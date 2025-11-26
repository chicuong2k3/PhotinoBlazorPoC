using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace TowelBorrowing.Services.Impls;

internal class ScreenServiceWindows : IScreenService
{
	private readonly ILogger<ScreenServiceWindows> _logger;

	public ScreenServiceWindows(ILogger<ScreenServiceWindows> logger)
	{
		_logger = logger;
	}

	public byte[] CaptureScreen()
	{
		try
		{
			int w = GetSystemMetrics(0);
			int h = GetSystemMetrics(1);
			using var bmp = new Bitmap(w, h, PixelFormat.Format32bppArgb);
			using var g = Graphics.FromImage(bmp);
			var dst = g.GetHdc();
			var src = GetWindowDC(IntPtr.Zero);
			BitBlt(dst, 0, 0, w, h, src, 0, 0, 0x00CC0020);
			g.ReleaseHdc(dst);
			ReleaseDC(IntPtr.Zero, src);

			using var ms = new MemoryStream();
			bmp.Save(ms, ImageFormat.Png);
			return ms.ToArray();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "CaptureScreen failed (Windows)");
			return Array.Empty<byte>();
		}
	}

	public byte[] CaptureProcessRegion(string processName, Rectangle region)
	{
		try
		{
			var proc = Process.GetProcessesByName(processName)
				.FirstOrDefault(p => p.MainWindowHandle != IntPtr.Zero);
			if (proc == null) return Array.Empty<byte>();

			var hWnd = proc.MainWindowHandle;

			if (!GetClientRect(hWnd, out RECT clientRect))
				return Array.Empty<byte>();

			// Clamp region
			region.X = Math.Max(0, Math.Min(region.X, clientRect.Width - 1));
			region.Y = Math.Max(0, Math.Min(region.Y, clientRect.Height - 1));
			region.Width = Math.Min(region.Width, clientRect.Width - region.X);
			region.Height = Math.Min(region.Height, clientRect.Height - region.Y);

			using var fullBmp = new Bitmap(clientRect.Width, clientRect.Height, PixelFormat.Format32bppArgb);
			using var g = Graphics.FromImage(fullBmp);
			var hdcDest = g.GetHdc();

			bool success = PrintWindow(hWnd, hdcDest, 2);
			g.ReleaseHdc(hdcDest);

			if (!success)
			{
				_logger.LogError("Failed to capture client area for {Process}", processName);
				return Array.Empty<byte>();
			}

			using var resultBmp = new Bitmap(region.Width, region.Height, PixelFormat.Format32bppArgb);
			using (var cropG = Graphics.FromImage(resultBmp))
			{
				cropG.DrawImage(
					fullBmp,
					new Rectangle(0, 0, region.Width, region.Height),
					region,
					GraphicsUnit.Pixel);
			}

			using var ms = new MemoryStream();
			resultBmp.Save(ms, ImageFormat.Png);
			return ms.ToArray();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "CaptureProcessRegion failed - {Process}", processName);
			return Array.Empty<byte>();
		}
	}

	public byte[] CaptureMessageBox(string processName)
	{
		try
		{
			var proc = Process.GetProcessesByName(processName).FirstOrDefault();
			if (proc == null)
			{
				_logger.LogError("Process {processName} not found", processName);
				return Array.Empty<byte>();
			}

			IntPtr messageBoxHwnd = IntPtr.Zero;

			EnumWindows((hWnd, lParam) =>
			{
				GetWindowThreadProcessId(hWnd, out uint pid);
				if (pid == proc.Id)
				{
					var className = new System.Text.StringBuilder(256);
					GetClassName(hWnd, className, className.Capacity);
					if (className.ToString() == "#32770") // Dialog class = MessageBox
					{
						messageBoxHwnd = hWnd;
						return false; // stop enumerate
					}
				}
				return true; // continue
			}, IntPtr.Zero);

			if (messageBoxHwnd == IntPtr.Zero)
			{
				_logger.LogWarning("MessageBox not found for process {processName}", processName);
				return Array.Empty<byte>();
			}

			// Lấy kích thước message box
			if (!GetWindowRect(messageBoxHwnd, out RECT messageBoxRect))
			{
				_logger.LogError("Failed to get MessageBox rect");
				return Array.Empty<byte>();
			}

			int width = messageBoxRect.Width;
			int height = messageBoxRect.Height;

			// Validate kích thước
			if (width <= 0 || height <= 0)
			{
				_logger.LogError("Invalid MessageBox size: {Width}x{Height}", width, height);
				return Array.Empty<byte>();
			}

			// Chụp ảnh message box
			using var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
			using var g = Graphics.FromImage(bmp);

			IntPtr hdcDest = g.GetHdc();
			IntPtr hdcSrc = GetWindowDC(messageBoxHwnd);

			bool success = BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, 0x00CC0020);

			g.ReleaseHdc(hdcDest);
			ReleaseDC(messageBoxHwnd, hdcSrc);

			if (!success)
			{
				_logger.LogError("BitBlt failed for MessageBox capture");
				return Array.Empty<byte>();
			}

			using var ms = new MemoryStream();
			bmp.Save(ms, ImageFormat.Png);
			return ms.ToArray();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "CaptureMessageBox failed for process {processName}", processName);
			return Array.Empty<byte>();
		}
	}
	public Point? GetMessageBoxCenter(string processName)
	{
		try
		{
			var proc = Process.GetProcessesByName(processName).FirstOrDefault();
			if (proc == null)
			{
				_logger.LogError("Process {processName} not found", processName);
				return null;
			}

			IntPtr messageBoxHwnd = IntPtr.Zero;

			EnumWindows((hWnd, lParam) =>
			{
				GetWindowThreadProcessId(hWnd, out uint pid);
				if (pid == proc.Id)
				{
					var className = new System.Text.StringBuilder(256);
					GetClassName(hWnd, className, className.Capacity);
					if (className.ToString() == "#32770") // Dialog class = MessageBox
					{
						messageBoxHwnd = hWnd;
						return false; // dừng enumerate
					}
				}
				return true; // tiếp tục
			}, IntPtr.Zero);

			if (messageBoxHwnd == IntPtr.Zero)
			{
				_logger.LogWarning("MessageBox not found for process {processName}", processName);
				return null;
			}

			GetWindowRect(messageBoxHwnd, out RECT r);
			return new Point(r.Left + r.Width / 2, r.Top + r.Height / 2);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "GetMessageBoxCenter failed for process {processName}", processName);
			return null;
		}
	}

	public Point? GetProcessCenter(string processName)
	{
		try
		{
			var proc = Process.GetProcessesByName(processName)
				.FirstOrDefault(p => p.MainWindowHandle != IntPtr.Zero);
			if (proc == null)
			{
				_logger.LogError("Process {processName} not found", processName);

				var processes = Process.GetProcesses();
				foreach (var pro in processes)
				{
					_logger.LogWarning("{processName} running", pro.ProcessName);
				}

				return null;
			}

			var hWnd = proc.MainWindowHandle;
			GetClientRect(hWnd, out RECT r);
			Point tl = new(0, 0);
			ClientToScreen(hWnd, ref tl);

			return new Point(tl.X + r.Width / 2, tl.Y + r.Height / 2);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "GetProcessCenter failed (Windows)");
			return null;
		}
	}

	// =============== P/Invoke Windows ===============
	[StructLayout(LayoutKind.Sequential)] private struct RECT { public int Left, Top, Right, Bottom; public int Width => Right - Left; public int Height => Bottom - Top; }

	[DllImport("user32.dll")] private static extern IntPtr GetWindowDC(IntPtr hWnd);
	[DllImport("user32.dll")] private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
	[DllImport("gdi32.dll")] private static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);
	[DllImport("user32.dll")] private static extern int GetSystemMetrics(int nIndex);
	[DllImport("user32.dll")] private static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);
	[DllImport("user32.dll")] private static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);
	[DllImport("user32.dll")] private static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, uint nFlags);
	[DllImport("user32.dll")] private static extern bool IsIconic(IntPtr hWnd);
	[DllImport("user32.dll")] private static extern bool IsWindow(IntPtr hWnd);
	[DllImport("user32.dll")] private static extern bool IsWindowVisible(IntPtr hWnd);

	// ================= P/Invoke 26/11 ====================
	[DllImport("user32.dll")]
	private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

	private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

	[DllImport("user32.dll")]
	private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

	[DllImport("user32.dll", SetLastError = true)]
	private static extern int GetClassName(IntPtr hWnd, System.Text.StringBuilder lpClassName, int nMaxCount);

	[DllImport("user32.dll")]
	private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
}

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

	public Point? GetProcessCenter(string processName)
	{
		try
		{
			var proc = Process.GetProcessesByName(processName)
				.FirstOrDefault(p => p.MainWindowHandle != IntPtr.Zero);
			if (proc == null) return null;

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
}

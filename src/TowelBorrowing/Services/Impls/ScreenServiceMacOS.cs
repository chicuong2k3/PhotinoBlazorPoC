using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace TowelBorrowing.Services.Impls;

internal class ScreenServiceMacOS : IScreenService
{
	private readonly ILogger<ScreenServiceMacOS> _logger;

	public ScreenServiceMacOS(ILogger<ScreenServiceMacOS> logger) => _logger = logger;

	public byte[] CaptureScreen()
	{
		try
		{
			var img = CGDisplayCreateImage(CGMainDisplayID());
			var png = ConvertCGImageToPng(img);
			CFRelease(img);
			return png;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "CaptureScreen failed (macOS)");
			return Array.Empty<byte>();
		}
	}

	public byte[] CaptureProcessRegion(string processName, Rectangle region)
	{
		try
		{
			var proc = Process.GetProcessesByName(processName).FirstOrDefault();
			if (proc == null) return Array.Empty<byte>();

			const uint listOptions = 1 | (1 << 3);
			const uint imageOptions = 1 << 2;

			var fullImg = CGWindowListCreateImage(
				new CGRect { X = double.NegativeInfinity, Y = double.NegativeInfinity, Width = 0, Height = 0 },
				listOptions,
				(uint) proc.Id,
				imageOptions);

			if (fullImg == IntPtr.Zero) return Array.Empty<byte>();

			int fullW = CGImageGetWidth(fullImg);
			int fullH = CGImageGetHeight(fullImg);

			if (region.X < 0 || region.Y < 0 || region.Right > fullW || region.Bottom > fullH)
			{
				CFRelease(fullImg);
				return Array.Empty<byte>();
			}

			var cropRect = new CGRect { X = region.X, Y = region.Y, Width = region.Width, Height = region.Height };
			var cropped = CGImageCreateWithImageInRect(fullImg, cropRect);
			var png = ConvertCGImageToPng(cropped);

			CFRelease(fullImg);
			if (cropped != IntPtr.Zero) CFRelease(cropped);

			return png;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "CaptureProcessRegion failed (macOS) - {Process}", processName);
			return Array.Empty<byte>();
		}
	}

	public Point? GetProcessCenter(string processName)
	{
		try
		{
			var proc = Process.GetProcessesByName(processName).FirstOrDefault();
			if (proc == null) return null;

			var list = CGWindowListCopyWindowInfo(1, 0);
			if (list == IntPtr.Zero) return null;

			var count = CFArrayGetCount(list);
			for (nint i = 0; i < count; i++)
			{
				var dict = CFArrayGetValueAtIndex(list, i);
				if (GetWindowPid(dict) == proc.Id && TryGetWindowBounds(dict, out var r))
				{
					CFRelease(list);
					return new Point((int) (r.X + r.Width / 2), (int) (r.Y + r.Height / 2));
				}
			}

			CFRelease(list);
			return null;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "GetProcessCenter failed (macOS)");
			return null;
		}
	}

	private byte[] ConvertCGImageToPng(IntPtr img)
	{
		if (img == IntPtr.Zero) return Array.Empty<byte>();

		int w = CGImageGetWidth(img);
		int h = CGImageGetHeight(img);
		var provider = CGImageGetDataProvider(img);
		var data = CGDataProviderCopyData(provider);
		var len = (int) CFDataGetLength(data);
		var ptr = CFDataGetBytePtr(data);

		var bytes = new byte[len];
		Marshal.Copy(ptr, bytes, 0, len);

		using var bmp = new Bitmap(w, h, PixelFormat.Format32bppArgb);
		var rect = new Rectangle(0, 0, w, h);
		var bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
		Marshal.Copy(bytes, 0, bmpData.Scan0, len);
		bmp.UnlockBits(bmpData);

		using var ms = new MemoryStream();
		bmp.Save(ms, ImageFormat.Png);
		return ms.ToArray();
	}

	private int GetWindowPid(IntPtr dict)
	{
		var ptr = CFDictionaryGetValue(dict, kCGWindowOwnerPID);
		return ptr != IntPtr.Zero && CFNumberGetValue(ptr, 3, out int pid) ? pid : -1;
	}

	private bool TryGetWindowBounds(IntPtr dict, out CGRect bounds)
	{
		bounds = default;
		var ptr = CFDictionaryGetValue(dict, kCGWindowBounds);
		if (ptr != IntPtr.Zero)
		{
			bounds = Marshal.PtrToStructure<CGRect>(ptr);
			return true;
		}
		return false;
	}

	[StructLayout(LayoutKind.Sequential)] private struct CGRect { public double X, Y, Width, Height; }

	[DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
	private static extern uint CGMainDisplayID();

	[DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
	private static extern IntPtr CGDisplayCreateImage(uint displayId);

	[DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
	private static extern IntPtr CGWindowListCreateImage(CGRect screenBounds, uint listOption, uint windowId, uint imageOption);

	[DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
	private static extern IntPtr CGImageCreateWithImageInRect(IntPtr image, CGRect rect);

	[DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
	private static extern int CGImageGetWidth(IntPtr image);

	[DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
	private static extern int CGImageGetHeight(IntPtr image);

	[DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
	private static extern IntPtr CGImageGetDataProvider(IntPtr image);

	[DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
	private static extern IntPtr CGDataProviderCopyData(IntPtr provider);

	[DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
	private static extern IntPtr CFDataGetBytePtr(IntPtr data);

	[DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
	private static extern nint CFDataGetLength(IntPtr data);

	[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
	private static extern void CFRelease(IntPtr cf);

	[DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
	private static extern IntPtr CGWindowListCopyWindowInfo(uint option, uint relativeToWindow);

	[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
	private static extern nint CFArrayGetCount(IntPtr array);

	[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
	private static extern IntPtr CFArrayGetValueAtIndex(IntPtr array, nint index);

	[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
	private static extern IntPtr CFDictionaryGetValue(IntPtr dict, IntPtr key);

	[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
	private static extern bool CFNumberGetValue(IntPtr number, int type, out int value);

	private static readonly IntPtr kCGWindowOwnerPID = CFStringCreate("kCGWindowOwnerPID");
	private static readonly IntPtr kCGWindowBounds = CFStringCreate("kCGWindowBounds");

	private static IntPtr CFStringCreate(string s)
	{
		return Marshal.StringToHGlobalUni(s);
	}
}

using System.Drawing;

namespace TowelBorrowing.Services;

public interface IScreenService
{
	byte[] CaptureScreen();
	byte[] CaptureProcessRegion(string processName, Rectangle region);
	Point? GetProcessCenter(string processName);
	Point? GetMessageBoxCenter(string processName);
}

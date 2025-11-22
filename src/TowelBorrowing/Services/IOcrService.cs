using System;
using System.Collections.Generic;
using System.Text;

namespace TowelBorrowing.Services;

public interface IOcrService
{
	Task<string> RecognizeAsync(string imagePath);
}

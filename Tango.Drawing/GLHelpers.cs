using SharpFont;
using System;

namespace Tango.Drawing
{
	public static class GLHelpers
    {
		public static byte[] GraysToRGBA(byte[] grays)
		{
			byte[] imgbmp = new byte[4 * grays.Length];
			var p = 0;
			for (int i = 0; i < grays.Length; i++)
			{
				imgbmp[p] = grays[i];
				imgbmp[p + 1] = grays[i];
				imgbmp[p + 2] = grays[i];
				imgbmp[p + 3] = 255;
				p += 4;
			}
			return imgbmp;
		}

		public static byte[] LcdToRGBA(byte[] data, int width, int height, int pitch)
		{
			byte[] imgbmp = new byte[4 * width * height];
			var p = 0;
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					int pos = (y * pitch) + (x * 3);

					imgbmp[p] = data[pos];
					imgbmp[p + 1] = data[pos + 1];
					imgbmp[p + 2] = data[pos + 2];
					if (data[pos] == 0 && data[pos + 1] == 0 && data[pos + 2] == 0)
						imgbmp[p + 3] = 0;
					else
					{
						var c = 0.2126 * data[pos] / 255f + 0.7152 * data[pos + 1] / 255f + 0.0722 * data[pos + 2] / 255f;
						c = c <= 0.0031308 ? 12.92 * c : 1.055 * Math.Pow(c, 1 / 2.4) - 0.055;
						c = c * 255;
						imgbmp[p + 3] = (byte)c;
					}
					p += 4;
				}
			}
			return imgbmp;
		}
	}
}

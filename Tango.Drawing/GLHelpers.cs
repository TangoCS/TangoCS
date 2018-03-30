using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.Drawing
{
    public static class GLHelpers
    {
		public static byte[] GraysToRGBA(byte[] grays, int width, int height)
		{
			byte[] imgbmp = new byte[4 * width * height];
			var p = 0;
			for (int row = 0; row < height; row++)
			{
				for (int col = 0; col < width; col++)
				{
					imgbmp[p] = grays[row * width + col];
					imgbmp[p + 1] = grays[row * width + col];
					imgbmp[p + 2] = grays[row * width + col];
					imgbmp[p + 3] = 255;
					p += 4;
				}
			}
			return imgbmp;
		}
    }
}

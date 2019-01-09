using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using System;

namespace Tango.Drawing.Test
{
	public partial class Program
	{
		public static void Main1(string[] args)
		{

			using (var ctx = new WglContext(800, 600))
			{
				unsafe
				{
					var s = new string((sbyte*)GL.glGetString(GL.GL_RENDERER));
					Console.WriteLine(s);
				}

				GL.glMatrixMode(GL.GL_PROJECTION);
				GL.glLoadIdentity();
				GL.glOrtho(0.0, 1.0f, 0.0, 1.0, 0.0, 1.0);
				GL.glMatrixMode(GL.GL_MODELVIEW);
				GL.glLoadIdentity();

				GL.glBegin(GL.GL_TRIANGLES);
				GL.glColor3f(1.0f, 0.0f, 0.0f); GL.glVertex2f(0.0f, 0.0f);
				GL.glColor3f(0.0f, 1.0f, 0.0f); GL.glVertex2f(0.5f, 1.0f);
				GL.glColor3f(0.0f, 0.0f, 1.0f); GL.glVertex2f(1.0f, 0.0f);
				GL.glEnd();

				using (Image<Rgba32> image = Image.LoadPixelData<Rgba32>(ctx.GetBytes(), ctx.Width, ctx.Height))
				{
					image.Save("test.jpg");
					image.Save("test.png", new PngEncoder());
					image.Save("test.gif", new GifEncoder());
					image.Save("test.bmp", new BmpEncoder());
				}
			}
		}
	}
}

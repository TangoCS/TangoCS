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
		public static void Main(string[] args)
		{
			using (var ctx = new OsMesaContext(800, 600))
			{
				unsafe
				{
					var s = new string((sbyte*)GL.glGetString(GL.GL_RENDERER));
					Console.WriteLine(s);
				}


				//ctx.Drawing.BeginDraw(Rgba32.Black);

				//GL.glColor4f(0, 0, 1, 0);

				//DrawRectangle(0, 0, 100, 100);
				//GL.glBegin(GL.GL_LINE_LOOP);

				//GL.glVertex2f(0, 0.5f);
				//GL.glVertex2f(0, 100f);
				//GL.glVertex2f(100f, 100f);
				//GL.glVertex2f(100f, 0.5f);

				//GL.glEnd();


				//FillRectangle(0, 0, 100, 100);

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

				//ctx.Drawing.SetFont(@"C:\Projects\TangoCS\Tango.Drawing.Test\bin\Debug\net47\Cousine-Regular-Latin.ttf");
				//ctx.Drawing.DrawString(100, 100, "test111 g", 14);
				//ctx.Drawing.DrawString(100, 130, "test111 g", 12);
				//ctx.Drawing.DrawString(100, 160, "test111 g", 11);
				//ctx.Drawing.DrawString(100, 180, "test111 g", 10);
				//ctx.Drawing.DrawString(100, 200, "test111 g", 8);

				using (Image<Rgba32> image = Image.LoadPixelData<Rgba32>(ctx.GetBytes(), ctx.Width, ctx.Height))
				{
					image.Save("test.jpg");
					image.Save("test.png", new PngEncoder());
					image.Save("test.gif", new GifEncoder());
					image.Save("test.bmp", new BmpEncoder());
				}
			}
		}

		static void DrawRectangle(int x, int y, int w, int h)
		{
			GL.glBegin(GL.GL_LINE_LOOP);
			GL.glVertex2i(x + w, y);
			GL.glVertex2i(x + w, y + h);
			GL.glVertex2i(x, y + h);
			GL.glVertex2i(x, y);
			GL.glEnd();
		}

		static void FillRectangle(int x, int y, int w, int h)
		{
			GL.glRecti(x, y, x + w, y + h);
		}

		
	}
}

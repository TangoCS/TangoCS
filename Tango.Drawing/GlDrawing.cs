using SixLabors.ImageSharp.PixelFormats;
using SixLabors.Primitives;
using System;

namespace Tango.Drawing
{
	public class GLDrawing : IDrawing, IDisposable
    {
		FontService _fontService;
		bool _blend = false;

		int _width, _height;

		public GLDrawing(int width, int height)
		{
			_width = width;
			_height = height;
			_fontService = new FontService(this);
		}

		void SetDashStyle(DashStyle style)
		{
			switch (style)
			{
				case DashStyle.Dash:
					GL.glLineStipple(3, 0xAAAA); /* dashed */
					break;
				case DashStyle.DashDot:
					//GL.glLineStipple(1, 0xF18F); 
					GL.glLineStipple(1, 0xE187);
					break;
				case DashStyle.DashDotDot:
					//GL.glLineStipple(1, 0xC447); // штрих-пунктир
					GL.glLineStipple(1, 0xE667); // штрих-пунктир
					break;
				case DashStyle.Dot:
					GL.glLineStipple(1, 0x0101); /* dotted */
					break;
				case DashStyle.Solid:
					GL.glLineStipple(1, 0xFFFF);
					break;
				case DashStyle.Custom:
					GL.glLineStipple(3, 0xAAAA); /* dashed */
					break;
				default:
					break;
			}
		}

		public bool BeginDraw(Rgba32 c)
		{		
			return BeginDraw(_width, _height, c);
		}

		public bool BeginDraw()
		{
			return BeginDraw(_width, _height);
		}

		bool BeginDraw(int width, int height)
		{
			GL.glViewport(0, 0, width, height);
			GL.glMatrixMode(GL.GL_PROJECTION);
			GL.glLoadIdentity();
			GL.glOrtho(0.0f, width, height, 0.0f, 0.0f, 1.0f);
			GL.glMatrixMode(GL.GL_MODELVIEW);
			GL.glDisable(GL.GL_DEPTH_TEST);

			return true;
		}

		bool BeginDraw(int width, int height, Rgba32 c)
		{
			BeginDraw(width, height);

			GL.glClearColor((float)c.R / 255, (float)c.G / 255, (float)c.B / 255, (float)c.A / 255);
			GL.glClear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT);
			GL.glShadeModel(GL.GL_FLAT);

			return true;
		}

		public void SetColor(Rgba32 c)
		{
			GL.glColor4ub(c.R, c.G, c.B, c.A);
		}

		public void SetColor(Rgba32 c, byte alpha)
		{
			GL.glColor4ub(c.R, c.G, c.B, alpha);
		}

		public void EnableBlend()
		{
			_blend = true;
		}

		public void DisableBlend()
		{
			_blend = false;
		}


		public void DrawLine(int x1, int y1, int x2, int y2)
		{
			GL.glBegin(GL.GL_LINES);
			GL.glVertex2f(x1 + 0.375f, y1 + 0.375f);
			GL.glVertex2f(x2 + 0.375f, y2 + 0.375f);
			GL.glEnd();
		}

		public void DrawLine(int x1, int y1, int x2, int y2, DashStyle style)
		{
			if (_blend)
			{
				GL.glEnable(GL.GL_BLEND);
				GL.glBlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);
			}

			GL.glEnable(GL.GL_LINE_STIPPLE);
			//GL.glLineWidth(p.Width);
			SetDashStyle(style);
			DrawLine(x1, y1, x2, y2);
			GL.glLineWidth(1);
			GL.glDisable(GL.GL_LINE_STIPPLE);

			if (_blend)
			{
				GL.glDisable(GL.GL_BLEND);
			}
		}

		public void DrawLineStrip(int[] p)
		{
			GL.glBegin(GL.GL_LINE_STRIP);
			for (int i = 0; i < p.Length; i += 2)
				GL.glVertex2f(p[i] + 0.375f, p[i + 1] + 0.375f);
			GL.glEnd();
		}

		public void DrawLineStrip(int[] p, DashStyle style)
		{
			GL.glEnable(GL.GL_LINE_STIPPLE);
			SetDashStyle(style);

			DrawLineStrip(p);
			GL.glDisable(GL.GL_LINE_STIPPLE);
		}

		public void DrawLines(int[] p)
		{
			GL.glBegin(GL.GL_LINES);
			for (int i = 0; i < p.Length; i += 2)
				GL.glVertex2f(p[i] + 0.375f, p[i + 1] + 0.375f);
			GL.glEnd();

			GL.glBegin(GL.GL_POINTS);
			for (int i = 0; i < p.Length; i += 2)
				GL.glVertex2f(p[i] + 0.375f, p[i + 1] + 0.375f);
			GL.glEnd();
		}

		public void DrawTriangleStrip(int[] p)
		{
			GL.glBegin(GL.GL_TRIANGLE_STRIP);
			for (int i = 0; i < p.Length; i += 2)
				GL.glVertex2f(p[i] + 0.375f, p[i + 1] + 0.375f);
			GL.glEnd();
		}

		public void DrawRectangle(int x, int y, int w, int h)
		{
			GL.glBegin(GL.GL_LINE_LOOP);
			GL.glVertex2f(x + w + 0.375f, y + 0.375f);
			GL.glVertex2f(x + w + 0.375f, y + h + 0.375f);
			GL.glVertex2f(x + 0.375f, y + h + 0.375f);
			GL.glVertex2f(x + 0.375f, y + 0.375f);
			GL.glEnd();
		}

		public void DrawRectangle(Rectangle r)
		{
			DrawRectangle(r.X, r.Y, r.Width, r.Height);
		}

		public void FillEllipse(int x, int y, int w, int h, double alpha)
		{
			double x1, y1, a, b;
			int t;
			a = (double)w / 2;
			b = (double)h / 2;

			if (_blend)
			{
				GL.glEnable(GL.GL_BLEND);
				GL.glBlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);
			}
			GL.glBegin(GL.GL_TRIANGLE_FAN);
			GL.glVertex2d(x, y);
			for (t = 0; t <= 360; t += 10)
			{
				x1 = x + a * Math.Cos(alpha * Math.PI / 180) * Math.Cos(t * Math.PI / 180) - b * Math.Sin(alpha * Math.PI / 180) * Math.Sin(t * Math.PI / 180);
				y1 = y + a * Math.Sin(alpha * Math.PI / 180) * Math.Cos(t * Math.PI / 180) + b * Math.Cos(alpha * Math.PI / 180) * Math.Sin(t * Math.PI / 180);

				GL.glVertex2d(x1, y1);
			}
			GL.glEnd();
			if (_blend)
			{
				GL.glDisable(GL.GL_BLEND);
			}
		}

		public void FillTriangle(Point p1, Point p2, Point p3)
		{
			if (_blend)
			{
				GL.glEnable(GL.GL_BLEND);
				GL.glBlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);
			}
			GL.glBegin(GL.GL_TRIANGLES);
			GL.glVertex2f(p1.X + 0.375f, p1.Y + 0.375f);
			GL.glVertex2f(p2.X + 0.375f, p2.Y + 0.375f);
			GL.glVertex2f(p3.X + 0.375f, p3.Y + 0.375f);
			GL.glEnd();
			if (_blend)
			{
				GL.glDisable(GL.GL_BLEND);
			}
		}

		public void DrawCircle(int x, int y, int r)
		{
			double x1, y1;
			int t;
			GL.glBegin(GL.GL_LINE_LOOP);
			for (t = 0; t <= 360; t += 30)
			{
				x1 = Math.Sin(t * Math.PI / 180);
				y1 = Math.Cos(t * Math.PI / 180);

				GL.glVertex2d(Math.Round(r * x1 + x, 0), Math.Round(r * y1 + y, 0));
			}
			GL.glEnd();
		}



		public void FillRectangle(int x, int y, int w, int h)
		{
			if (_blend)
			{
				GL.glEnable(GL.GL_BLEND);
				GL.glBlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);
			}

			GL.glBegin(GL.GL_TRIANGLE_STRIP);
			GL.glVertex2f(x + 0.375f, y + 0.375f);
			GL.glVertex2f(x + 0.375f, y + h + 0.375f);
			GL.glVertex2f(x + w + 0.375f, y + 0.375f);
			GL.glVertex2f(x + w + 0.375f, y + h + 0.375f);	
			GL.glEnd();

			if (_blend)
			{
				GL.glDisable(GL.GL_BLEND);
			}
		}

		public void FillRectangle(int x, int y, int w, int h, Rgba32 gradientBottomColor, Rgba32 gradientTopColor)
		{
			if(_blend)
			{
				GL.glEnable(GL.GL_BLEND);
				GL.glBlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);
			}

			GL.glShadeModel(GL.GL_SMOOTH);
			GL.glBegin(GL.GL_TRIANGLE_STRIP);
			SetColor(gradientBottomColor);
			GL.glVertex2f(x + 0.375f, y + 0.375f);
			GL.glVertex2f(x + w + 0.375f, y + 0.375f);			
			SetColor(gradientTopColor);
			GL.glVertex2f(x + 0.375f, y + h + 0.375f);
			GL.glVertex2f(x + w + 0.375f, y + h + 0.375f);
			GL.glEnd();
			GL.glShadeModel(GL.GL_FLAT);

			if (_blend)
			{
				GL.glDisable(GL.GL_BLEND);
			}
		}

		public void FillRectangle(Rectangle r)
		{
			FillRectangle(r.X, r.Y, r.Width, r.Height);
		}

		public void DrawString(int x, int y, string s, int size)
		{
			_fontService.RenderString(x, y, s, size);
		}

		public int MeasureString(string s, int size)
		{
			return _fontService.MeasureString(s, size);
		}

		public void SetFont(string fontFile)
		{
			_fontService.SetFont(fontFile);
		}

		public void Dispose()
		{
			_fontService.Dispose();
		}

		public void EndDraw()
		{
			
		}

		//public void FrameViewEnable(IFrame f)
		//{
			//GL.glScissor(f.X, _height - f.Height - f.Y, f.Width, f.Height);
			//GL.glEnable(GL.GL_SCISSOR_TEST);
		//}

		//public void FrameViewDisable()
		//{
			//GL.glDisable(GL.GL_SCISSOR_TEST);
		//}

		public void DrawTexture(uint textureno, int x, int y, int width, int height, int[] map = null)
		{
			if (map == null) map = new int[8] {
				0, 0,
				0, 1,
				1, 1,
				1, 0
			};

			GL.glEnable(GL.GL_TEXTURE_2D);

			//GL.glTexEnvf(GL.GL_TEXTURE_ENV, GL.GL_TEXTURE_ENV_MODE, GL.GL_COMBINE);
			GL.glBindTexture(GL.GL_TEXTURE_2D, textureno);
			//GL.glGenerateMipmap(GL.GL_TEXTURE_2D);

			GL.glEnable(GL.GL_BLEND);
			GL.glBlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);

			GL.glBegin(GL.GL_QUADS);
			GL.glTexCoord2d(map[0], map[1]);
			GL.glVertex2f(x, y + height);
			GL.glTexCoord2d(map[2], map[3]);
			GL.glVertex2f(x, y);
			GL.glTexCoord2d(map[4], map[5]);
			GL.glVertex2f(x + width, y);
			GL.glTexCoord2d(map[6], map[7]);
			GL.glVertex2f(x + width, y + height);
			GL.glEnd();

			GL.glDisable(GL.GL_BLEND);
			GL.glDisable(GL.GL_TEXTURE_2D);
		}

		public uint AddTexture(int width, int height, byte[] data)
		{
			var tex = new uint[1];

			GL.glGenTextures(1, tex);

			// Set up some texture parameters for opengl
			GL.glBindTexture(GL.GL_TEXTURE_2D, tex[0]);

			GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MAG_FILTER, GL.GL_LINEAR);
			GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MIN_FILTER, GL.GL_LINEAR);

			// Create the texture
			GL.glTexImage2D(GL.GL_TEXTURE_2D, 0, (int)GL.GL_RGBA, width, height, 0, GL.GL_RGBA, GL.GL_UNSIGNED_BYTE, data);

			return tex[0];
		}

		public uint DrawOnTexture(int width, int height, Action content)
		{
			var renderedTexture = new uint[1];
			GL.glGenTextures(1, renderedTexture);
			GL.glBindTexture(GL.GL_TEXTURE_2D, renderedTexture[0]);
			GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MAG_FILTER, GL.GL_LINEAR);
			GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MIN_FILTER, GL.GL_LINEAR);

			GL.glTexImage2D(GL.GL_TEXTURE_2D, 0, (int)GL.GL_RGBA, width, height, 0, GL.GL_RGBA, GL.GL_UNSIGNED_BYTE, IntPtr.Zero);
			GL.glBindTexture(GL.GL_TEXTURE_2D, 0);

			var FramebufferName = new uint[1];
			GL.glGenFramebuffers(1, FramebufferName);
			GL.glBindFramebuffer(GL.GL_FRAMEBUFFER, FramebufferName[0]);

			GL.glFramebufferTexture2D(GL.GL_FRAMEBUFFER, GL.GL_COLOR_ATTACHMENT0, GL.GL_TEXTURE_2D, renderedTexture[0], 0);

			var r = GL.glCheckFramebufferStatus(GL.GL_FRAMEBUFFER);

			BeginDraw(width, height, Rgba32.Black);

			content();

			GL.glBindFramebuffer(GL.GL_FRAMEBUFFER, 0);

			BeginDraw();
			SetColor(Rgba32.White);

			return renderedTexture[0];
		}
	}
}

using SixLabors.ImageSharp;
using SixLabors.Primitives;
using System;

namespace Tango.Drawing
{
	public class GLDrawing : IDrawing, IDisposable
    {
		FontService _fontService = new FontService();
		bool _blend = false;

		int _width, _height;

		public GLDrawing(int width, int height)
		{
			_width = width;
			_height = height;
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
			GL.glViewport(0, 0, _width, _height);
			GL.glMatrixMode(GL.GL_PROJECTION);
			GL.glLoadIdentity();
			GL.glOrtho(0.0f, _width, _height, 0.0f, 0.0f, 1.0f);

			GL.glClearColor((float)c.R / 255, (float)c.G / 255, (float)c.B / 255, (float)c.A / 255);
			GL.glClear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT);		
			GL.glShadeModel(GL.GL_FLAT);
			return true;
		}

		public void SetColor(Rgba32 c)
		{
			_blend = (c.A < 255);
			GL.glColor4f((float)c.R / 255, (float)c.G / 255, (float)c.B / 255, (float)c.A / 255);
		}

		public void SetColor(Rgba32 c, int alpha)
		{
			_blend = (alpha < 255);
			GL.glColor4f((float)c.R / 255, (float)c.G / 255, (float)c.B / 255, (float)alpha / 255);
		}

		public void DrawLine(int x1, int y1, int x2, int y2)
		{
			GL.glBegin(GL.GL_LINES);
			GL.glVertex2i(x1, y1);
			GL.glVertex2i(x2, y2);
			GL.glEnd();
		}

		public void DrawLine(int x1, int y1, int x2, int y2, DashStyle style)
		{
			GL.glEnable(GL.GL_LINE_STIPPLE);
			//GL.glLineWidth(p.Width);
			SetDashStyle(style);
			DrawLine(x1, y1, x2, y2);
			GL.glLineWidth(1);
			GL.glDisable(GL.GL_LINE_STIPPLE);
		}

		public void DrawLineStrip(Point[] p)
		{
			GL.glBegin(GL.GL_LINE_STRIP);
			for (int i = 0; i < p.Length; i++)
				GL.glVertex2i(p[i].X, p[i].Y);
			GL.glEnd();
		}

		public void DrawLineStrip(Point[] p, DashStyle style)
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
				GL.glVertex2i(p[i], p[i + 1]);

			GL.glEnd();
		}

		public void DrawLines(Point[] pb, Point[] pe)
		{
			GL.glBegin(GL.GL_LINES);
			for (int i = 0; i < pb.Length; i++)
			{
				GL.glVertex2i(pb[i].X, pb[i].Y);
				GL.glVertex2i(pe[i].X, pe[i].Y);
			}
			GL.glEnd();
		}

		public void DrawLines(Point[] pb, Point[] pe, DashStyle style)
		{
			GL.glEnable(GL.GL_LINE_STIPPLE);
			SetDashStyle(style);
			DrawLines(pb, pe);
			GL.glDisable(GL.GL_LINE_STIPPLE);
		}

		public void DrawRectangle(int x, int y, int w, int h)
		{
			GL.glBegin(GL.GL_LINE_LOOP);
			GL.glVertex2i(x + w, y);
			GL.glVertex2i(x + w, y + h);
			GL.glVertex2i(x, y + h);
			GL.glVertex2i(x, y);
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
			GL.glVertex2i(p1.X, p1.Y);
			GL.glVertex2i(p2.X, p2.Y);
			GL.glVertex2i(p3.X, p3.Y);
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

			GL.glRecti(x, y, x + w, y + h);

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
	}
}

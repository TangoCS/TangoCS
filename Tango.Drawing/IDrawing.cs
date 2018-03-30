using SixLabors.ImageSharp;
using SixLabors.Primitives;

namespace Tango.Drawing
{
	public interface IDrawing
	{
		bool BeginDraw(Rgba32 backcolor);
		void DrawCircle(int x, int y, int r);
		void DrawLine(int x1, int y1, int x2, int y2, DashStyle style);
		void DrawLine(int x1, int y1, int x2, int y2);
		void DrawLines(int[] p);
		void DrawLines(Point[] pb, Point[] pe);
		void DrawLines(Point[] pb, Point[] pe, DashStyle style);
		void DrawLineStrip(Point[] p);
		void DrawLineStrip(Point[] p, DashStyle style);
		void DrawRectangle(Rectangle r);
		void DrawRectangle(int x, int y, int w, int h);
		void DrawString(int x, int y, string s, int size);
		uint AddTexture(int width, int height, byte[] data);
		void DrawTexture(uint textureno, int x, int y, int width, int height);
		void EndDraw();
		void FillEllipse(int x, int y, int w, int h, double alpha);
		void FillRectangle(Rectangle r);
		void FillRectangle(int x, int y, int w, int h);
		void FillTriangle(Point p1, Point p2, Point p3);
		void FrameViewDisable();
		void FrameViewEnable(IFrame f);
		void SetColor(Rgba32 c);
		void SetColor(Rgba32 c, byte alpha);
		void SetFont(string fontFile);
	}

	public interface IFrame
	{
		int Height { get; set; }
		int Width { get; set; }
		int X { get; set; }
		int Y { get; set; }
	}

	public enum DashStyle
	{
		Solid = 0,
		Dash = 1,
		Dot = 2,
		DashDot = 3,
		DashDotDot = 4,
		Custom = 5
	}
}

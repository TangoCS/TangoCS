using SixLabors.ImageSharp;
using SixLabors.Primitives;

namespace Tango.Drawing
{
	public interface IDrawing
	{
		bool BeginDraw(Rgba32 backcolor);
		//void DeInitText();
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
		void EndDraw();
		void FillEllipse(int x, int y, int w, int h, double alpha);
		void FillRectangle(Rectangle r);
		void FillRectangle(int x, int y, int w, int h);
		void FillTriangle(Point p1, Point p2, Point p3);
		//void FrameViewDisable();
		//void FrameViewEnable(IFrame f);
		//void Init(Control owner);
		//void InitText(Font font, int index);
		void SetColor(Rgba32 c);
		void SetColor(Rgba32 c, int alpha);
		void SetFont(string fontFile);
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

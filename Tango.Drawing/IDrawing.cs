using SixLabors.ImageSharp.PixelFormats;
using SixLabors.Primitives;
using System;

namespace Tango.Drawing
{
	public interface IDrawing
	{
		bool BeginDraw(Rgba32 backcolor);
		bool BeginDraw();
		void DrawPoint(int x, int y);
		void DrawCircle(int x, int y, int r);
		void DrawLine(int x1, int y1, int x2, int y2, DashStyle style);
		void DrawLine(int x1, int y1, int x2, int y2);
		void DrawLines(int[] p);
		void DrawLineStrip(int[] p);
		void DrawLineStrip(int[] p, DashStyle style);
		void DrawTriangleStrip(int[] p);
		void DrawRectangle(Rectangle r);
		void DrawRectangle(int x, int y, int w, int h);
		void DrawString(int x, int y, string s, int size);
		int MeasureString(string s, int size);
		uint AddTexture(int width, int height, byte[] data);
		void DrawTexture(uint textureno, int x, int y, int width, int height, int[] map = null);
		uint DrawOnTexture(int width, int height, Action content);
		void EndDraw();
		void FillEllipse(int x, int y, int w, int h, double alpha);
		void FillRectangle(Rectangle r);
		void FillRectangle(int x, int y, int w, int h);
		void FillRectangle(int x, int y, int w, int h, Rgba32 gradientBottomColor, Rgba32 gradientTopColor);
		void FillTriangle(Point p1, Point p2, Point p3);
		//void FrameViewDisable();
		//void FrameViewEnable(IFrame f);
		void SetColor(Rgba32 c);
		void SetColor(Rgba32 c, byte alpha);
		void SetFont(string fontFile);
		void EnableBlend();
		void DisableBlend();
	}

	//public interface IFrame
	//{
	//	int Height { get; set; }
	//	int Width { get; set; }
	//	int X { get; set; }
	//	int Y { get; set; }
	//}

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

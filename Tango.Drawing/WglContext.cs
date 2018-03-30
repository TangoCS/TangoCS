using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Tango.Drawing
{
	public class WglContext : IDisposable
    {
		int _bitDepth;

		int _length;

		IntPtr _windowHandle = IntPtr.Zero;
		IntPtr _deviceContextHandle = IntPtr.Zero;
		IntPtr _renderContextHandle = IntPtr.Zero;

		public int Width { get; private set; }
		public int Height { get; private set; }

		GLDrawing _drawing;
		public IDrawing Drawing => _drawing;

		public WglContext(int width, int height, int bitDepth = 32)
		{
			Width = width;
			Height = height;
			_bitDepth = bitDepth;

			_length = Width * Height * 4;

			Create();

			_drawing = new GLDrawing(width, height);
		}

		void Create()
		{
			// Create a new window class, as basic as possible.                
			Win32.WNDCLASSEX wndClass = new Win32.WNDCLASSEX();
			wndClass.Init();
			wndClass.style = Win32.ClassStyles.HorizontalRedraw | Win32.ClassStyles.VerticalRedraw | Win32.ClassStyles.OwnDC;
			wndClass.lpfnWndProc = wndProcDelegate;
			wndClass.cbClsExtra = 0;
			wndClass.cbWndExtra = 0;
			wndClass.hInstance = IntPtr.Zero;
			wndClass.hIcon = IntPtr.Zero;
			wndClass.hCursor = IntPtr.Zero;
			wndClass.hbrBackground = IntPtr.Zero;
			wndClass.lpszMenuName = null;
			wndClass.lpszClassName = "SharpGLRenderWindow";
			wndClass.hIconSm = IntPtr.Zero;
			Win32.RegisterClassEx(ref wndClass);

			// Create the window. Position and size it.
			_windowHandle = Win32.CreateWindowEx(0,
						  "SharpGLRenderWindow",
						  "",
						  Win32.WindowStyles.WS_CLIPCHILDREN | Win32.WindowStyles.WS_CLIPSIBLINGS | Win32.WindowStyles.WS_POPUP,
						  0, 0, Width, Height,
						  IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

			// Get the window device context.
			_deviceContextHandle = Win32.GetDC(_windowHandle);

			// Setup a pixel format.
			Win32.PIXELFORMATDESCRIPTOR pfd = new Win32.PIXELFORMATDESCRIPTOR();
			pfd.Init();
			pfd.nVersion = 1;
			pfd.dwFlags = Win32.PFD_DRAW_TO_WINDOW | Win32.PFD_SUPPORT_OPENGL | Win32.PFD_DOUBLEBUFFER;
			pfd.iPixelType = Win32.PFD_TYPE_RGBA;
			pfd.cColorBits = (byte)_bitDepth;
			pfd.cDepthBits = 16;
			pfd.cStencilBits = 8;
			pfd.iLayerType = Win32.PFD_MAIN_PLANE;

			// Match an appropriate pixel format 
			int iPixelformat;
			if ((iPixelformat = Win32.ChoosePixelFormat(_deviceContextHandle, pfd)) == 0)
				return;

			// Sets the pixel format
			if (Win32.SetPixelFormat(_deviceContextHandle, iPixelformat, pfd) == 0)
			{
				return;
			}

			// Create the render context.
			_renderContextHandle = wglCreateContext(_deviceContextHandle);

			wglMakeCurrent(_deviceContextHandle, _renderContextHandle);
		}

		public byte[] GetBytes()
		{
			byte[] buffer = new byte[_length];
			GL.glReadPixels(0, 0, Width, Height, GL.GL_RGBA, GL.GL_UNSIGNED_BYTE, buffer);
			return buffer;
		}

		public byte[] ToGif()
		{
			using (Image<Rgba32> image = Image.LoadPixelData<Rgba32>(GetBytes(), Width, Height))
			{
				using (MemoryStream ms = new MemoryStream())
				{
					image.Save(ms, new GifEncoder());
					return ms.ToArray();
				}
			}
		}

		public void Dispose()
		{
			if (_renderContextHandle != IntPtr.Zero)
				wglDeleteContext(_renderContextHandle);
			if (_deviceContextHandle != IntPtr.Zero)
				Win32.ReleaseDC(_windowHandle, _deviceContextHandle);
			if (_windowHandle != IntPtr.Zero)
				Win32.DestroyWindow(_windowHandle);
		}

		static IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
		{
			return Win32.DefWindowProc(hWnd, msg, wParam, lParam);
		}
		static Win32.WndProc wndProcDelegate = new Win32.WndProc(WndProc);

		[DllImport("opengl32", SetLastError = true)]
		static extern System.IntPtr wglGetCurrentContext();
		[DllImport("opengl32", SetLastError = true)]
		static extern System.IntPtr wglGetCurrentDC();
		[DllImport("opengl32", SetLastError = true)]
		static extern System.IntPtr wglCreateContext(IntPtr hdc);
		[DllImport("opengl32", SetLastError = true)]
		static extern System.Int32 wglMakeCurrent(IntPtr hdc, IntPtr hglrc);
		[DllImport("opengl32", SetLastError = true)]
		static extern System.Int32 wglDeleteContext(IntPtr hglrc);
	}
}

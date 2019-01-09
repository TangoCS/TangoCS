using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Tango.Drawing
{
	public class OsMesaContext : IDisposable
	{
		int _bitDepth;
		int _length;
		IntPtr _ctx;
		IntPtr _bufferPnt;

		public int Width { get; private set; }
		public int Height { get; private set; }

		GLDrawing _drawing;
		public IDrawing Drawing => _drawing;

		public OsMesaContext(int width, int height, int bitDepth = 32)
		{
			Width = width;
			Height = height;
			_bitDepth = bitDepth;

			_length = Width * Height * 4;

			_ctx = OSMesaCreateContext(GL.GL_RGBA, IntPtr.Zero);

			_bufferPnt = Marshal.AllocHGlobal(Marshal.SizeOf<byte>() * _length);

			if (!OSMesaMakeCurrent(_ctx, _bufferPnt, GL.GL_UNSIGNED_BYTE, width, height))
			{
				throw new Exception("OSMesaMakeCurrent failed!");
			}

			_drawing = new GLDrawing(width, height);
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

		public byte[] ToPng()
		{
			using (Image<Rgba32> image = Image.LoadPixelData<Rgba32>(GetBytes(), Width, Height))
			{
				using (MemoryStream ms = new MemoryStream())
				{
					image.Save(ms, new PngEncoder());
					return ms.ToArray();
				}
			}
		}

		public void Dispose()
		{
			_drawing.Dispose();
			OSMesaDestroyContext(_ctx);
			Marshal.FreeHGlobal(_bufferPnt);
		}

		

		[DllImport(GL.Opengl)]
		public static extern IntPtr OSMesaCreateContext(uint format, IntPtr sharelist);
		[DllImport(GL.Opengl)]
		public static extern IntPtr OSMesaGetCurrentContext();
		[DllImport(GL.Opengl)]
		public static extern IntPtr OSMesaCreateContextExt(uint format, int depthBits, int stencilBits, int accumBits, IntPtr sharelist);
		[DllImport(GL.Opengl)]
		public static extern IntPtr OSMesaCreateContextAttribs(int[] attribList, IntPtr sharelist);

		[DllImport(GL.Opengl)]
		public static extern bool OSMesaMakeCurrent(IntPtr hdc, IntPtr buffer, uint type, int width, int height);
		[DllImport(GL.Opengl)]
		public static extern void OSMesaDestroyContext(IntPtr hdc);

		[DllImport(GL.Opengl)]
		public static extern void OSMesaGetIntegerv(int pname, out int value);

		public const int OSMESA_WIDTH = 0x20;
		public const int OSMESA_HEIGHT = 0x21;
		public const int OSMESA_FORMAT = 0x22;
		public const int OSMESA_TYPE = 0x23;
		public const int OSMESA_MAX_WIDTH = 0x24;  /* new in 4.0 */
		public const int OSMESA_MAX_HEIGHT = 0x25;  /* new in 4.0 */

		public const int OSMESA_PROFILE = 0x33;
		public const int OSMESA_CORE_PROFILE = 0x34;
	}
}

using System;
using System.Collections.Generic;
using System.IO;

using SharpFont;

namespace Tango.Drawing
{
	public class FontService : IDisposable
	{
		private Library _lib = new Library();
		private Face _fontFace;

		public FontFormatCollection SupportedFormats { get; } = new FontFormatCollection {
			{ "TrueType", "ttf" },
			{ "OpenType", "otf" }
		};

		/// <summary>
		/// If multithreading, each thread should have its own FontService.
		/// </summary>
		public FontService()
		{
			
		}

		public void SetFont(string filename)
		{
			_fontFace = new Face(_lib, filename);
		}

		#region RenderString

		/// <summary>
		/// Render the string into a bitmap with an opaque background.
		/// </summary>
		/// <param name="text">The string to render.</param>
		/// <param name="foreColor">The color of the text.</param>
		/// <param name="backColor">The color of the background behind the text.</param>
		/// <returns></returns>
		public virtual void RenderString(int x, int y, string text, int fontSize)
		{
			RenderString(_lib, _fontFace, x, y, text, fontSize);
		}

		static void RenderString(Library library, Face face, int x, int y, string text, int fontSize)
		{
			float penX = x;
			float overrun = 0;
			float underrun = 0;
			float kern = 0;

			face.SetCharSize(0, fontSize, 0, 96);

			var textures = new uint[128];
			GL.glEnable(GL.GL_TEXTURE_2D);
			GL.glDisable(GL.GL_DEPTH_TEST);
			GL.glGenTextures(128, textures);

			// Draw the string into the bitmap.
			// A lot of this is a repeat of the measuring steps, but this time we have
			// an actual bitmap to work with (both canvas and bitmaps in the glyph slot).
			for (int i = 0; i < text.Length; i++)
			{
				#region Load character
				char c = text[i];

				// Same as when we were measuring, except RenderGlyph() causes the glyph data
				// to be converted to a bitmap.
				uint glyphIndex = face.GetCharIndex(c);
				face.LoadGlyph(glyphIndex, LoadFlags.Default, LoadTarget.Normal);
				face.Glyph.RenderGlyph(RenderMode.Normal);
				FTBitmap ftbmp = face.Glyph.Bitmap;

				float gAdvanceX = (float)face.Glyph.Advance.X;
				float gBearingX = (float)face.Glyph.Metrics.HorizontalBearingX;
				float gWidth = (float)face.Glyph.Metrics.Width;

				#endregion

				#region Underrun
				// Underrun
				underrun += -(gBearingX);
				if (penX == x)
					penX += underrun;
				#endregion

				#region Draw glyph
				// Whitespace characters sometimes have a bitmap of zero size, but a non-zero advance.
				// We can't draw a 0-size bitmap, but the pen position will still get advanced (below).
				if ((ftbmp.Width > 0 && ftbmp.Rows > 0))
				{
					RenderChar(face, ftbmp, textures[glyphIndex], (int)Math.Round(penX + face.Glyph.BitmapLeft), y);
					// Check if we are aligned properly on the right edge (for debugging)
				}

				#endregion

				#region Overrun
				if (gBearingX + gWidth > 0 || gAdvanceX > 0)
				{
					overrun -= Math.Max(gBearingX + gWidth, gAdvanceX);
					if (overrun <= 0) overrun = 0;
				}
				overrun += (float)(gBearingX == 0 && gWidth == 0 ? 0 : gBearingX + gWidth - gAdvanceX);
				if (i == text.Length - 1) penX += overrun;
				#endregion

				// Advance pen positions for drawing the next character.
				penX += (float)face.Glyph.Advance.X; // same as Metrics.HorizontalAdvance?

				#region Kerning (for NEXT character)
				// Adjust for kerning between this character and the next.
				if (face.HasKerning && i < text.Length - 1)
				{
					char cNext = text[i + 1];
					kern = (float)face.GetKerning(glyphIndex, face.GetCharIndex(cNext), KerningMode.Default).X;
					if (kern > gAdvanceX * 5 || kern < -(gAdvanceX * 5))
						kern = 0;
					penX += (float)kern;
				}
				#endregion

			}

		}

		static void RenderChar(Face face, FTBitmap ftbmp, uint texture, int x, int y)
		{
			int bmpSize = ftbmp.Width * ftbmp.Rows;

			byte[] bmp = new byte[bmpSize];
			Array.Copy(ftbmp.BufferData, bmp, bmp.Length);

			// Set up some texture parameters for opengl
			GL.glBindTexture(GL.GL_TEXTURE_2D, texture);
			GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MAG_FILTER, GL.GL_LINEAR);
			GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MIN_FILTER, GL.GL_LINEAR);

			byte[] imgbmp = new byte[4 * ftbmp.Width * ftbmp.Rows];
			var p = 0;
			for (int row = 0; row < ftbmp.Rows; row++)
			{
				for (int col = 0; col < ftbmp.Width; col++)
				{
					imgbmp[p] = bmp[row * ftbmp.Width + col];
					imgbmp[p + 1] = bmp[row * ftbmp.Width + col];
					imgbmp[p + 2] = bmp[row * ftbmp.Width + col];
					imgbmp[p + 3] = 255;
					p += 4;
				}
			}

			// Create the texture
			GL.glTexImage2D(GL.GL_TEXTURE_2D, 0, (int)GL.GL_RGBA, ftbmp.Width, ftbmp.Rows,
				0, GL.GL_RGBA, GL.GL_UNSIGNED_BYTE, imgbmp);
			bmp = null;

			var bottomY = y + face.Glyph.Metrics.HorizontalBearingY.ToInt32() - ftbmp.Rows;

			//  Draw the quad
			GL.glBegin(GL.GL_QUADS);
			GL.glTexCoord2d(0, 0);
			GL.glVertex2f(x, bottomY + ftbmp.Rows);
			GL.glTexCoord2d(0, 1);
			GL.glVertex2f(x, bottomY);
			GL.glTexCoord2d(1, 1);
			GL.glVertex2f(x + ftbmp.Width, bottomY);
			GL.glTexCoord2d(1, 0);
			GL.glVertex2f(x + ftbmp.Width, bottomY + ftbmp.Rows);
			GL.glEnd();
		}



		#endregion // RenderString

		bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					if (_fontFace != null && !_fontFace.IsDisposed)
						_fontFace.Dispose();
				}

				disposedValue = true;
			}
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
		}
	}

	public class FontFormat
	{
		/// <summary>
		/// Gets the name for the format.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Gets the typical file extension for this format (lowercase).
		/// </summary>
		public string FileExtension { get; private set; }

		public FontFormat(string name, string ext)
		{
			if (!ext.StartsWith(".")) ext = "." + ext;
			Name = name; FileExtension = ext;
		}

	}

	public class FontFormatCollection : Dictionary<string, FontFormat>
	{

		public void Add(string name, string ext)
		{
			if (!ext.StartsWith(".")) ext = "." + ext;
			Add(ext, new FontFormat(name, ext));
		}

		public bool ContainsExt(string ext)
		{
			return ContainsKey(ext);
		}

	}
}
using System;
using System.Collections.Generic;
using SharpFont;

namespace Tango.Drawing
{
	public class FontService : IDisposable
	{
		Library _lib = new Library();
		Face _fontFace;

		IDrawing _d;
		Dictionary<int, uint[]> _texturesStore = new Dictionary<int, uint[]>();

		public FontFormatCollection SupportedFormats { get; } = new FontFormatCollection {
			{ "TrueType", "ttf" },
			{ "OpenType", "otf" }
		};

		/// <summary>
		/// If multithreading, each thread should have its own FontService.
		/// </summary>
		public FontService(IDrawing d)
		{
			_d = d;
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

		void RenderString(Library library, Face face, int x, int y, string text, int fontSize)
		{
			float penX = x;
			float overrun = 0;
			float underrun = 0;
			float kern = 0;

			face.SetCharSize(0, fontSize, 0, 96);

			if (!_texturesStore.TryGetValue(fontSize, out var textures))
			{
				textures = new uint[128];
				_texturesStore.Add(fontSize, textures);
			}

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
				face.LoadGlyph(glyphIndex, LoadFlags.Default, LoadTarget.Lcd);
				face.Glyph.RenderGlyph(RenderMode.Lcd);
				FTBitmap ftbmp = face.Glyph.Bitmap;
				int gWidthi = face.Glyph.Metrics.Width.ToInt32();

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
				if (ftbmp.Width > 0 && ftbmp.Rows > 0)
				{
					var t = textures[glyphIndex];
					if (t == 0)
					{
						var imgbmp = GLHelpers.LcdToRGBA(ftbmp.BufferData, gWidthi, ftbmp.Rows, ftbmp.Pitch);
						//var imgbmp = GLHelpers.GraysToRGBA(ftbmp.BufferData);
						
						t = _d.AddTexture(gWidthi, ftbmp.Rows, imgbmp);
						textures[glyphIndex] = t;
					}
					var bottomY = y + face.Glyph.Metrics.HorizontalBearingY.ToInt32() - ftbmp.Rows;
					_d.DrawTexture(t, (int)Math.Round(penX + face.Glyph.BitmapLeft), bottomY, gWidthi, ftbmp.Rows);
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
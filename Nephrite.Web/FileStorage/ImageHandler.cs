using System;
using System.Web;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using System.Drawing.Drawing2D;


namespace Nephrite.FileStorage
{
    public class ImageHandler : IHttpHandler
    {
        //IImageProvider provider = null;

        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
			//if (provider == null)
			//{
			//    provider = (IImageProvider)Activator.CreateInstance(Settings.ImageProviderAssembly, Settings.ImageProviderClass).Unwrap();
			//}

            byte[] image;
            ImageFormat format;
			string fileName;
			string contentType;
            //int heigth = Query.GetInt("height") ?? 0;
			//int width = Query.GetInt("width") ?? 0;
			bool flag = FileProvider.GetFile(context, out image, out fileName, out contentType);
			format = GetFormat(fileName);
            if (!flag)
            {
                OutEmpty(context);
                context.Response.End();
            }

            int w = context.Request.Params["width"].ToInt32(0);
            int h = context.Request.Params["height"].ToInt32(0);
            //int mw = context.Request.Params["maxwidth"].ToInt32(0);
            //int mh = context.Request.Params["maxheight"].ToInt32(0);
            if ((w > 0 /*&& w != width*/) ||
                (h > 0 /*&& h != heigth*/))
            {
                image = GetResizedImage(image, w/* > 0 ? w : width*/, h/* > 0 ? h : heigth*/, format);
            }
            /*else if ((mw > 0 && width > mw) || (mh > 0 && heigth > mh))
            {
                image = GetResizedImage(image, mw > 0 ? mw : width, mh > 0 ? mh : heigth, format);
            }*/

            context.Response.OutputStream.Write(image, 0, image.Length);

            context.Response.End();
        }

        private static void OutEmpty(HttpContext context)
        {
            Bitmap emptyImage = new Bitmap(1, 1);

            MemoryStream outstream = new MemoryStream();
            emptyImage.Save(outstream, ImageFormat.Gif);
            byte[] data = outstream.ToArray();
            context.Response.OutputStream.Write(data, 0, data.Length);
        }

        static byte[] GetResizedImage(byte[] image, int newWidth, int newHeight, ImageFormat format)
        {
            using (Bitmap imgIn = new Bitmap(new MemoryStream(image)))
            {
                int rw = imgIn.Width;
                int rh = imgIn.Height;
                int fw = 0;
                int fh = 0;

                if (newWidth == rw && newHeight == rh)
                    return image;

                if (newWidth > rw && newHeight > rh)
                {
                    fw = rw;
                    fh = rh;
                }

                if (newWidth <= rw && newHeight > rh)
                {
                    fw = newWidth;
                    fh = Convert.ToInt32((rh * newWidth) / rw);
                }

                if (newWidth > rw && newHeight <= rh)
                {
                    fw = rw;
                    fh = rh;
                }
                if (newWidth <= rw && newHeight <= rh)
                {
                    fw = newWidth;
                    fh = Convert.ToInt32((rh * newWidth) / rw);
                }
                if (newWidth <= rw && newHeight <= rh && fh > newHeight)
                {
                    fh = newHeight;
                    fw = Convert.ToInt32((rw * newHeight) / rh);
                }

                MemoryStream outStream = new MemoryStream();

                Rectangle dest = new Rectangle(0, 0, fw, fh);
                Rectangle src = new Rectangle(0, 0, rw, rh);

                Bitmap imgOut = new Bitmap(fw, fh);
                Graphics g = Graphics.FromImage(imgOut);
                g.CompositingQuality = CompositingQuality.HighQuality;

                g.DrawImage(imgIn, dest, src, GraphicsUnit.Pixel);

                imgOut.Save(outStream, format);

                return outStream.ToArray();
            }
        }

		public ImageFormat GetFormat(string fileName)
		{
			string ext = Path.GetExtension(fileName);
			switch (ext)
			{
				case ".bmp":
					return ImageFormat.Bmp;
				case ".jpg":
					return ImageFormat.Jpeg;
				case ".gif":
					return ImageFormat.Gif;
				case ".png":
					return ImageFormat.Png;
				default:
					return null;
			}
		}
    }
}

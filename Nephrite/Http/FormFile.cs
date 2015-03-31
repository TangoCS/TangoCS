using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nephrite.Http
{
	public class FormFile : IFormFile
	{
		private Stream _baseStream;
		private long _baseStreamOffset;
		private long _length;

		public FormFile(Stream baseStream, long baseStreamOffset, long length)
		{
			_baseStream = baseStream;
			_baseStreamOffset = baseStreamOffset;
			_length = length;
		}

		public string ContentDisposition
		{
			get { return Headers["Content-Disposition"]; }
			set { Headers["Content-Disposition"] = value; }
		}

		public string ContentType
		{
			get { return Headers["Content-Type"]; }
			set { Headers["Content-Type"] = value; }
		}

		public IHeaderDictionary Headers { get; set; }

		public long Length
		{
			get { return _length; }
		}

		public Stream OpenReadStream()
		{
			return new ReferenceReadStream(_baseStream, _baseStreamOffset, _length);
		}
	}
}

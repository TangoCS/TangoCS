using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.IO;
using Nephrite.Web;
using Nephrite.Web.SPM;
using Nephrite.Web.FileStorage;
using System.Text;
using System.Xml.Linq;

namespace Solution.Model
{
	partial class DbFile : IDbFile
	{
		private int _IsValid;

		#region Validation
		bool IsNew = true;
		List<ValidationMessage> _validationMessages;
		string writeErrorMessage;

		public bool CheckValid()
		{
			if (_validationMessages == null)
			{
				_validationMessages = new List<ValidationMessage>();
				// Проверка на отсутствие файла с таким же именем
				//if (IsNew && FileStorageManager.DbFiles.Any(o => o.Path == _Path && o.Title == _Title))
				//	_validationMessages.Add(new ValidationMessage("Файл " + (_Path ?? "") + "/" + _Title + " уже существует", ValidationMessageSeverity.Error));
				// Проверка на отсутствие кривых символов
				if (_Title.ToCharArray().Intersect(System.IO.Path.GetInvalidPathChars()).Count() > 0)
					_validationMessages.Add(new ValidationMessage("Имя файла содержит недопустимый символ", ValidationMessageSeverity.Error));
				if (_Title.IsEmpty())
					_validationMessages.Add(new ValidationMessage("Имя файла не может быть пустым", ValidationMessageSeverity.Error));
				if (_Title != null && _Title.Length > 300)
					_validationMessages.Add(new ValidationMessage("Имя файла не может превышать 300 символов", ValidationMessageSeverity.Error));
				if (writeErrorMessage != null)
					_validationMessages.Add(new ValidationMessage(writeErrorMessage, ValidationMessageSeverity.Error));
			}
			IsValid = _validationMessages.Count == 0 ? 1 : 0;
			return _validationMessages.Count == 0;
		}

		public void OnLoaded()
		{
			IsNew = false;
		}

		public List<ValidationMessage> GetValidationMessages()
		{
			CheckValid();
			return _validationMessages;
		}
		#endregion

		byte[] bytes;
		bool dataChanged = false;

		public byte[] GetBytes()
		{
			if (!IsNew)
			{
				bytes = FileStorageManager.GetBytes(this);
			}
			if (bytes == null)
				return new byte[0];
			return bytes;
		}

		public FileStorageType GetStorageType()
		{
			var f = GetParentFolder();
			if (f != null)
				return f.GetStorageType();
			return FileStorageType.LocalDatabase;
		}

		public string GetStorageParameter()
		{
			var f = GetParentFolder();
			if (f != null)
				return f.GetStorageParameter();
			return null;
		}

		public void Write(byte[] bytes)
		{
			SaveFileResult vc = VirusChecker.Check(Title, bytes);
			if (vc != SaveFileResult.OK)
			{
				writeErrorMessage = vc.ToText();
				return;
			}
			Size = bytes.Length;
			Changed();
			this.bytes = bytes;
			if (!dataChanged)
				App.DataContext.AfterSaveActions.Add(() =>
					{
						FileStorageManager.StoreData(this, this.bytes);
					});
			dataChanged = true;
		}

		public void WriteText(string text)
		{
			WriteText(text, Encoding.UTF8);
		}

		public void WriteText(string text, Encoding encoding)
		{
			MemoryStream ms = new MemoryStream();
			BinaryWriter br = new BinaryWriter(ms);
			br.Write(encoding.GetPreamble());
			br.Write(encoding.GetBytes(text));
			br.Close();
			Write(ms.ToArray());
		}

		public string GetText()
		{
			var bytes = FileStorageManager.GetBytes(this);
			if (bytes.Length>=3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
				return Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3);

			if (bytes.Length>=2 && bytes[0] == 0xFE && bytes[1] == 0xFF)
				return Encoding.BigEndianUnicode.GetString(bytes, 2, bytes.Length - 2);

			if (bytes.Length>=2 && bytes[0] == 0xFF && bytes[1] == 0xFE)
				return Encoding.Unicode.GetString(bytes, 2, bytes.Length - 2);
			
			return Encoding.UTF8.GetString(bytes);
		}

		public IDbFolder GetParentFolder()
		{
			return ParentFolder;
		}
		public void SetParentFolder(IDbFolder parent)
		{
			ParentFolder = (DbFolder)parent;
			if (parent != null)
			{
				ParentFolderID = parent.ID;
				Path = ParentFolder.FullPath;
			}
			else
			{
				Path = "";
				ParentFolderID = null;
			}
		}

		public bool CheckOut()
		{
			if (CheckedOutByID.HasValue)
				return false;
			CheckedOutByID = AppSPM.GetCurrentSubjectID();
			return true;
		}

		public bool CheckIn()
		{
			if (!CheckedOutByID.HasValue || CheckedOutByID.Value != AppSPM.GetCurrentSubjectID())
				return false;
			CheckedOutByID = null;
			return true;
		}

		public void WriteXML(XDocument document)
		{
			StringWriter sw = new StringWriter();
			document.Save(sw);
			WriteText(sw.ToString());
		}

		public XDocument GetXML()
		{
			return XDocument.Parse(GetText());
		}
	}
}
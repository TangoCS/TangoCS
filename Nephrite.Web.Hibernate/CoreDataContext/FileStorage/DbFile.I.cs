using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;
using Nephrite.Identity;
using Nephrite.Web.FileStorage;

namespace Nephrite.Web.CoreDataContext
{
	partial class V_DbFile : IDbFile
	{
		#region Validation
		bool IsNew = true;
		List<ValidationMessage> _validationMessages;
		string writeErrorMessage;

		public virtual bool CheckValid()
		{
			if (_validationMessages == null)
			{
				_validationMessages = new List<ValidationMessage>();
				// Проверка на отсутствие файла с таким же именем
				if (IsNew && FileStorageManager.DbFiles.Any(o => o.Path == Path && o.Title == Title))
					_validationMessages.Add(new ValidationMessage("Файл " + (Path ?? "") + "/" + Title + " уже существует", ValidationMessageSeverity.Error));
				// Проверка на отсутствие кривых символов
				if (Title.ToCharArray().Intersect(System.IO.Path.GetInvalidPathChars()).Count() > 0)
					_validationMessages.Add(new ValidationMessage("Имя файла содержит недопустимый символ", ValidationMessageSeverity.Error));
				if (Title.IsEmpty())
					_validationMessages.Add(new ValidationMessage("Имя файла не может быть пустым", ValidationMessageSeverity.Error));
				if (Title != null && Title.Length > 300)
					_validationMessages.Add(new ValidationMessage("Имя файла не может превышать 300 символов", ValidationMessageSeverity.Error));
				if (writeErrorMessage != null)
					_validationMessages.Add(new ValidationMessage(writeErrorMessage, ValidationMessageSeverity.Error));
			}
			IsValid = _validationMessages.Count == 0 ? 1 : 0;
			return _validationMessages.Count == 0;
		}

		public virtual void OnLoaded()
		{
			IsNew = false;
		}

		public virtual List<ValidationMessage> GetValidationMessages()
		{
			CheckValid();
			return _validationMessages;
		}
		#endregion

		byte[] bytes;
		bool dataChanged = false;

		public virtual byte[] GetBytes()
		{
			if (!IsNew)
			{
				bytes = FileStorageManager.GetBytes(this);
			}
			if (bytes == null)
				return new byte[0];
			return bytes;
		}

		public virtual FileStorageType GetStorageType()
		{
			var f = GetParentFolder();
			if (f != null)
				return f.GetStorageType();
			return FileStorageType.LocalDatabase;
		}

		public virtual string GetStorageParameter()
		{
			var f = GetParentFolder();
			if (f != null)
				return f.GetStorageParameter();
			return null;
		}

		public virtual void Write(byte[] bytes)
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
				A.Model.AfterSaveActions.Add(() =>
				{
					FileStorageManager.StoreData(this, this.bytes);
				});
			dataChanged = true;
		}

		public virtual void WriteText(string text)
		{
			WriteText(text, Encoding.UTF8);
		}

		public virtual void WriteText(string text, Encoding encoding)
		{
			MemoryStream ms = new MemoryStream();
			BinaryWriter br = new BinaryWriter(ms);
			br.Write(encoding.GetPreamble());
			br.Write(encoding.GetBytes(text));
			br.Close();
			Write(ms.ToArray());
		}

		public virtual string GetText()
		{
			var bytes = FileStorageManager.GetBytes(this);
			if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
				return Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3);

			if (bytes.Length >= 2 && bytes[0] == 0xFE && bytes[1] == 0xFF)
				return Encoding.BigEndianUnicode.GetString(bytes, 2, bytes.Length - 2);

			if (bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE)
				return Encoding.Unicode.GetString(bytes, 2, bytes.Length - 2);

			return Encoding.UTF8.GetString(bytes);
		}

		public virtual IDbFolder GetParentFolder()
		{
			return FileStorageManager.DbFolders.Where(o => o.ID == ParentFolderID).FirstOrDefault();
		}
		public virtual void SetParentFolder(IDbFolder parent)
		{
			if (parent != null)
			{
				ParentFolderID = parent.ID;
				Path = parent.FullPath;
			}
			else
			{
				Path = "";
				ParentFolderID = null;
			}
		}

		public virtual bool CheckOut()
		{
			if (CheckedOutByID.HasValue)
				return false;
			CheckedOutByID = Subject.Current.ID;
			return true;
		}

		public virtual bool CheckIn()
		{
			if (!CheckedOutByID.HasValue || CheckedOutByID.Value != Subject.Current.ID)
				return false;
			CheckedOutByID = null;
			return true;
		}

		public virtual void WriteXML(XDocument document)
		{
			StringWriter sw = new StringWriter();
			document.Save(sw);
			WriteText(sw.ToString());
		}

		public virtual XDocument GetXML()
		{
			return XDocument.Parse(GetText());
		}

		void Changed()
		{
			if (CheckedOutByID.HasValue && CheckedOutByID.Value != Subject.Current.ID)
				throw new Exception("Файл " + Title + " извлечен другим пользователем!");
			DateTime dt = DateTime.Now;
			this.LastModifiedDate = dt;
			var sid = Subject.Current.ID;
			if (sid != LastModifiedUserID)
			{
				this.LastModifiedUserID = sid;
			}
		}
	}
}
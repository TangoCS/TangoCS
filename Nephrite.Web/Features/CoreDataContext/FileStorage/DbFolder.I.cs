using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Web.FileStorage;

namespace Nephrite.Web.CoreDataContext
{
	partial class V_DbFolder : IDbFolder
	{
		#region Validation
		internal bool IsNew = true;
		List<ValidationMessage> _validationMessages;

		public void OnLoaded()
		{
			IsNew = false;
		}

		public bool CheckValid()
		{
			if (_validationMessages == null)
			{
				_validationMessages = new List<ValidationMessage>();
				// Проверка на отсутствие папки с таким же именем
				if (IsNew && FileStorageManager.DbFolders.Any(o => o.Path == Path && o.Title == Title))
					_validationMessages.Add(new ValidationMessage("Папка " + (Path ?? "") + "/" + Title + " уже существует", ValidationMessageSeverity.Error));
				// Проверка на отсутствие кривых символов
				if (Title.ToCharArray().Intersect(System.IO.Path.GetInvalidPathChars()).Count() > 0)
					_validationMessages.Add(new ValidationMessage("Имя папки содержит недопустимый символ", ValidationMessageSeverity.Error));
				if (Title.IsEmpty())
					_validationMessages.Add(new ValidationMessage("Имя папки не может быть пустым", ValidationMessageSeverity.Error));
			}
			IsValid = _validationMessages.Count == 0 ? 1 : 0;
			return _validationMessages.Count == 0;
		}

		public List<ValidationMessage> GetValidationMessages()
		{
			CheckValid();
			return _validationMessages;
		}
		#endregion

		public IDbFolder GetParentFolder()
		{
			return FileStorageManager.DbFolders.Where(o => o.ID == ParentFolderID).FirstOrDefault();
		}

		public void SetParentFolder(IDbFolder parent)
		{
			ParentFolderID = parent.ID;
		}

		public FileStorageType GetStorageType()
		{
			switch (StorageType)
			{
				case "B":
					return FileStorageType.LocalDatabase;
				case "D":
					return FileStorageType.FileSystem;
				case "R":
					return FileStorageType.RemoteDatabase;
				case "E":
					return FileStorageType.External;
				default:
					return FileStorageType.LocalDatabase;
			}
		}

		public string GetStorageParameter()
		{
			return StorageParameter;
		}

		public void SetStorageInfo(FileStorageType fileStorageType, string fileStorageParameter)
		{
			if (StorageParameter == fileStorageParameter && StorageType == GetStorageType(fileStorageType))
				return;

			StorageParameter = fileStorageParameter;
			StorageType = GetStorageType(fileStorageType);
		}

		string GetStorageType(FileStorageType fileStorageType)
		{
			switch (fileStorageType)
			{
				case FileStorageType.LocalDatabase:
					return "B";
				case FileStorageType.RemoteDatabase:
					return "R";
				case FileStorageType.FileSystem:
					return "D";
				case FileStorageType.External:
					return "E";
				default:
					return "B";
			}
		}
	}
}
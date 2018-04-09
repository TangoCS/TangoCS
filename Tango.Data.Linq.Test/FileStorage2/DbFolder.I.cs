using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Data.Linq.Mapping;
using System.Data.Linq;
using Nephrite.Web;
using Nephrite.Web.SPM;
using Nephrite.Web.FileStorage;

namespace Solution.Model
{
	partial class DbFolder : IDbFolder
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
				/*
				// Проверка на отсутствие папки с таким же именем
				if (IsNew && FileStorageManager.DbFolders.Any(o => o.Path == _Path && o.Title == _Title))
					_validationMessages.Add(new ValidationMessage("Папка " + (_Path ?? "") + "/" + _Title + " уже существует", ValidationMessageSeverity.Error));
				// Проверка на отсутствие кривых символов
				if (_Title.ToCharArray().Intersect(System.IO.Path.GetInvalidPathChars()).Count() > 0)
					_validationMessages.Add(new ValidationMessage("Имя папки содержит недопустимый символ", ValidationMessageSeverity.Error));
				if (_Title.IsEmpty())
					_validationMessages.Add(new ValidationMessage("Имя папки не может быть пустым", ValidationMessageSeverity.Error));
				 */
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
			return ParentFolder;
		}

		public void SetParentFolder(IDbFolder parent)
		{
			ParentFolder = (DbFolder)parent;
		}

		public FileStorageType GetStorageType()
		{
			switch (_StorageType)
			{
				case 'B':
					return FileStorageType.LocalDatabase;
				case 'D':
					return FileStorageType.FileSystem;
				case 'R':
					return FileStorageType.RemoteDatabase;
				case 'E':
					return FileStorageType.External;
				default:
					return FileStorageType.LocalDatabase;
			}
		}

		public string GetStorageParameter()
		{
			return _StorageParameter;
		}

		public void SetStorageInfo(FileStorageType fileStorageType, string fileStorageParameter)
		{
			if (StorageParameter == fileStorageParameter && StorageType == GetStorageType(fileStorageType))
				return;

			StorageParameter = fileStorageParameter;
			StorageType = GetStorageType(fileStorageType);
		}

		char GetStorageType(FileStorageType fileStorageType)
		{
			switch (fileStorageType)
			{
				case FileStorageType.LocalDatabase:
					return 'B';
				case FileStorageType.RemoteDatabase:
					return 'R';
				case FileStorageType.FileSystem:
					return 'D';
				case FileStorageType.External:
					return 'E';
				default:
					return 'B';
			}
		}
	}
}
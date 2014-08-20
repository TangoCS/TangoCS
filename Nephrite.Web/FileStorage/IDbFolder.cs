using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Web.FileStorage
{
	/// <summary>
	/// Папка в файловом хранилище
	/// </summary>
	public interface IDbFolder : IValidatable, IEntity
	{
		/// <summary>
		/// Уникальный идентификатор
		/// </summary>
		Guid ID { get; set; }

		/// <summary>
		/// Имя папки
		/// </summary>
		string Title { get; set; }

		/// <summary>
		/// Путь
		/// </summary>
		string Path { get; set; }

		/// <summary>
		/// Размер файлов
		/// </summary>
		long Size { get; set; }

		/// <summary>
		/// Признак Удалено
		/// </summary>
		bool IsDeleted { get; set; }

		/// <summary>
		/// Идентификатор родительской папки
		/// </summary>
		Guid? ParentFolderID { get; set; }

		/// <summary>
		/// Дата и время последнего изменения
		/// </summary>
		DateTime LastModifiedDate { get; set; }

		/// <summary>
		/// Имя пользователя, сделавшего последние изменения
		/// </summary>
		string LastModifiedUserName { get; set; }
		int LastModifiedUserID { get; set; }

		/// <summary>
		/// Получить родительскую папку
		/// </summary>
		/// <returns></returns>
		IDbFolder GetParentFolder();

		/// <summary>
		/// Получить тип хранения файлов
		/// </summary>
		/// <returns></returns>
		FileStorageType GetStorageType();
		string StorageType { get; set; }
		string StorageParameter { get; set; }

		/// <summary>
		/// Получить параметр хранения файлов
		/// </summary>
		/// <returns></returns>
		string GetStorageParameter();

		/// <summary>
		/// Задать параметры хранения файла
		/// </summary>
		/// <param name="fileStorageType"></param>
		/// <param name="fileStorageParameter"></param>
		void SetStorageInfo(FileStorageType fileStorageType, string fileStorageParameter);

		/// <summary>
		/// Задать папку
		/// </summary>
		/// <param name="folder"></param>
		void SetParentFolder(IDbFolder folder);

		Guid SPMActionItemGUID { get; set; }

		/// <summary>
		/// Полный путь
		/// </summary>
		string FullPath { get; set; }

		/// <summary>
		/// Включена версионность
		/// </summary>
		bool EnableVersioning { get; set; }

		/// <summary>
		/// Создавший пользователь
		/// </summary>
		string Creator { get; set; }

		/// <summary>
		/// Создавший пользователь
		/// </summary>
		int CreatorID { get; set; }

		/// <summary>
		/// Тэг
		/// </summary>
		string Tag { get; set; }

		/// <summary>
		/// Опубликован
		/// </summary>
		DateTime? PublishDate { get; set; }	
	}
}
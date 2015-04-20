using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nephrite.Data;

namespace Nephrite.Web.FileStorage
{
	/// <summary>
	/// Элемент файлового хранилища (папка, файл)
	/// </summary>
	public interface IDbItem : IEntity
	{
		DateTime LastModifiedDate { get; set; }
		int LastModifiedUserID { get; set; }
		/// <summary>
		/// Тип элемента
		/// </summary>
		DbItemType Type { get; set; }

		/// <summary>
		/// Название
		/// </summary>
		string Title { get; set; }

		/// <summary>
		/// Идентификатор
		/// </summary>
		Guid ID { get; set; }

		/// <summary>
		/// Размер
		/// </summary>
		long Size { get; set; }

		/// <summary>
		/// Признак Удалено
		/// </summary>
		bool IsDeleted { get; set; }

		/// <summary>
		/// Идентификатор родителя
		/// </summary>
		Guid? ParentID { get; set; }

		/// <summary>
		/// Путь
		/// </summary>
		string Path { get; set; }

		/// <summary>
		/// Расширение
		/// </summary>
		string Extension { get; set; }

		/// <summary>
		/// Имя пользователя, сделавшего последние изменения
		/// </summary>
		string LastModifiedUserName { get; set; }

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
		/// Кем извлечено
		/// </summary>
		int? CheckedOutByID { get; set; }

		/// <summary>
		/// Кем извлечено
		/// </summary>
		string CheckedOutBy { get; set; }

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
		/// Дата публикации
		/// </summary>
		DateTime? PublishDate { get; set; }
	}
}

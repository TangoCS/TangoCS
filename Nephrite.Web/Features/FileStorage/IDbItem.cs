using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nephrite.Web.FileStorage
{
	/// <summary>
	/// Элемент файлового хранилища (папка, файл)
	/// </summary>
	public interface IDbItem : IEntity
	{
		/// <summary>
		/// Тип элемента
		/// </summary>
		DbItemType Type { get; }

		/// <summary>
		/// Название
		/// </summary>
		string Title { get; }

		/// <summary>
		/// Идентификатор
		/// </summary>
		Guid ID { get; }

		/// <summary>
		/// Размер
		/// </summary>
		long Size { get; }

		/// <summary>
		/// Признак Удалено
		/// </summary>
		bool IsDeleted { get; }

		/// <summary>
		/// Идентификатор родителя
		/// </summary>
		Guid? ParentID { get; }

		/// <summary>
		/// Путь
		/// </summary>
		string Path { get; }

		/// <summary>
		/// Расширение
		/// </summary>
		string Extension { get; }

		/// <summary>
		/// Дата и время последнего изменения
		/// </summary>
		DateTime LastModifiedDate { get; }

		/// <summary>
		/// Имя пользователя, сделавшего последние изменения
		/// </summary>
		string LastModifiedUserName { get; }

		Guid SPMActionItemGUID { get; }

		/// <summary>
		/// Полный путь
		/// </summary>
		string FullPath { get; }

		/// <summary>
		/// Включена версионность
		/// </summary>
		bool EnableVersioning { get; }

		/// <summary>
		/// Кем извлечено
		/// </summary>
		int? CheckedOutByID { get; }

		/// <summary>
		/// Кем извлечено
		/// </summary>
		string CheckedOutBy { get; }

		/// <summary>
		/// Создавший пользователь
		/// </summary>
		string Creator { get; }

		/// <summary>
		/// Создавший пользователь
		/// </summary>
		int CreatorID { get; }
		
		/// <summary>
		/// Тэг
		/// </summary>
		string Tag { get; }

		/// <summary>
		/// Дата публикации
		/// </summary>
		DateTime? PublishDate { get; }
	}
}

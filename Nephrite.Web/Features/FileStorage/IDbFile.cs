using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Xml.Linq;
using Nephrite.Web.SPM;

namespace Nephrite.Web.FileStorage
{
	/// <summary>
	/// Файл из файлового хранилища
	/// </summary>
	public interface IDbFile : IValidatable, IEntity
	{
		/// <summary>
		/// Уникальный идентификатор
		/// </summary>
		Guid ID { get; }

		/// <summary>
		/// Имя файла (включая расширение)
		/// </summary>
		string Title { get; set; }

		/// <summary>
		/// Расширение (включая точку)
		/// </summary>
		string Extension { get; }

		/// <summary>
		/// Размер содержимого
		/// </summary>
		long Size { get; }

		/// <summary>
		/// Путь
		/// </summary>
		string Path { get; }

		/// <summary>
		/// Признак Удалено
		/// </summary>
		bool IsDeleted { get; }

		/// <summary>
		/// Идентификатор родительской папки
		/// </summary>
		Guid? ParentFolderID { get; }

		/// <summary>
		/// Дата и время последнего изменения
		/// </summary>
		DateTime LastModifiedDate { get; }

		/// <summary>
		/// Имя пользователя, сделавшего последние изменения
		/// </summary>
		string LastModifiedUserName { get; }

		/// <summary>
		/// Получить родительскую папку
		/// </summary>
		/// <returns></returns>
		IDbFolder GetParentFolder();
		void SetParentFolder(IDbFolder parent);

		/// <summary>
		/// Получить данные
		/// </summary>
		/// <returns></returns>
		byte[] GetBytes();

		/// <summary>
		/// Записать данные
		/// </summary>
		/// <param name="bytes"></param>
		void Write(byte[] bytes);

		/// <summary>
		/// Получить тип хранения файлов
		/// </summary>
		/// <returns></returns>
		FileStorageType GetStorageType();

		/// <summary>
		/// Получить параметр хранения файлов
		/// </summary>
		/// <returns></returns>
		string GetStorageParameter();

		Guid SPMActionItemGUID { get; }

		/// <summary>
		/// Полный путь
		/// </summary>
		string FullPath { get; }

		/// <summary>
		/// Guid пакета или класса
		/// </summary>
		Guid? FeatureGUID { get; set;}

		/// <summary>
		/// Ид извлекшего пользователя
		/// </summary>
		int? CheckedOutByID { get; }

		/// <summary>
		/// Имя извлекшего пользователя
		/// </summary>
		string CheckedOutBy { get; }

		/// <summary>
		/// Ид основной версии файла
		/// </summary>
		Guid? MainID { get; }

		/// <summary>
		/// Номер версии
		/// </summary>
		int VersionNumber { get; }

		/// <summary>
		/// Тэг
		/// </summary>
		string Tag { get; set; }

		/// <summary>
		/// Извлечь
		/// </summary>
		/// <returns></returns>
		bool CheckOut();

		/// <summary>
		/// Вернуть
		/// </summary>
		/// <returns></returns>
		bool CheckIn();

		/// <summary>
		/// Записать текст
		/// </summary>
		/// <param name="text">Текст</param>
		void WriteText(string text);

		/// <summary>
		/// Записать текст
		/// </summary>
		/// <param name="text">Текст</param>
		/// <param name="encoding">Кодировка</param>
		void WriteText(string text, Encoding encoding);

		/// <summary>
		/// Получить текст
		/// </summary>
		/// <returns>Текст</returns>
		string GetText();

		/// <summary>
		/// Записать XML
		/// </summary>
		/// <param name="element">XML</param>
		void WriteXML(XDocument document);

		/// <summary>
		/// Получить XML
		/// </summary>
		/// <returns>XML</returns>
		XDocument GetXML();

		/// <summary>
		/// Создавший пользователь
		/// </summary>
		string Creator { get; }

		/// <summary>
		/// Создавший пользователь
		/// </summary>
		int CreatorID { get; }

		/// <summary>
		/// Опубликован
		/// </summary>
		DateTime? PublishDate { get; set; }
	}
}
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
	public interface IDbFile : IValidatable, IEntity, IWithTimeStamp
	{
		/// <summary>
		/// Уникальный идентификатор
		/// </summary>
		Guid ID { get; set; }

		/// <summary>
		/// Имя файла (включая расширение)
		/// </summary>
		string Title { get; set; }

		/// <summary>
		/// Расширение (включая точку)
		/// </summary>
		string Extension { get; set; }

		/// <summary>
		/// Размер содержимого
		/// </summary>
		long Size { get; set; }

		/// <summary>
		/// Путь
		/// </summary>
		string Path { get; set; }

		/// <summary>
		/// Признак Удалено
		/// </summary>
		bool IsDeleted { get; set; }

		/// <summary>
		/// Идентификатор родительской папки
		/// </summary>
		Guid? ParentFolderID { get; set; }

		/// <summary>
		/// Имя пользователя, сделавшего последние изменения
		/// </summary>
		string LastModifiedUserName { get; set; }

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

		Guid SPMActionItemGUID { get; set; }

		/// <summary>
		/// Полный путь
		/// </summary>
		string FullPath { get; set; }

		/// <summary>
		/// Guid пакета или класса
		/// </summary>
		Guid? FeatureGUID { get; set;}

		/// <summary>
		/// Ид извлекшего пользователя
		/// </summary>
		int? CheckedOutByID { get; set; }

		/// <summary>
		/// Имя извлекшего пользователя
		/// </summary>
		string CheckedOutBy { get; set; }

		/// <summary>
		/// Ид основной версии файла
		/// </summary>
		Guid? MainID { get; set; }

		/// <summary>
		/// Номер версии
		/// </summary>
		int VersionNumber { get; set; }

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
		string Creator { get; set; }

		/// <summary>
		/// Создавший пользователь
		/// </summary>
		int CreatorID { get; set; }

		/// <summary>
		/// Опубликован
		/// </summary>
		DateTime? PublishDate { get; set; }
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace Nephrite.Web.FileStorage
{
	public class Samples
	{
		void Run()
		{
			Guid folder_id = Guid.Empty;

// Получение списка папок
string rootpath = "SolutionSources";
var allCs = FileStorageManager.DbFolders.Where(o => o.Path.StartsWith(rootpath));
var ss = FileStorageManager.GetFolder(rootpath);
var allCs2 = FileStorageManager.DbFolders.Where(o => o.ParentFolderID == ss.ID);
Guid folderID = folder_id;
var myFolder = FileStorageManager.GetFolder(folder_id);

// Создание папки
var newFolder = FileStorageManager.CreateFolder("temp", "SolutionSources/Autogenerate");
if (!newFolder.CheckValid())
	throw new Exception(String.Join(Environment.NewLine, newFolder.GetValidationMessages().Select(o => o.Message).ToArray()));
Base.Model.SubmitChanges();

// Удаление папки
FileStorageManager.DeleteFolder(ss.ID);
Base.Model.SubmitChanges();

// Получение списка файлов
var allCs3 = FileStorageManager.DbFiles.Where(o => o.ParentFolderID == ss.ID && o.Extension == ".cs");
var allCs4 = FileStorageManager.DbFiles.Where(o => o.Path.StartsWith(rootpath) && o.Extension == ".cs");
var cs1 = FileStorageManager.GetFile(rootpath+"/Autogenerate/app.cs");

// Получение данных файла
var data = cs1.GetBytes();
var str = Encoding.UTF8.GetString(data);

// Создание файла
var file = FileStorageManager.CreateFile("my.xml", "Xml/Income");
// Обновление данных файла
file.Write(data);
if (!file.CheckValid())
	throw new Exception(String.Join(Environment.NewLine, file.GetValidationMessages().Select(o => o.Message).ToArray()));
Base.Model.SubmitChanges();

FileStorageManager.DeleteFile(file.ID);
Base.Model.SubmitChanges();

		}
	}
}
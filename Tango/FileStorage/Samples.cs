using Tango.Data;
using System;

namespace Tango.FileStorage.Std
{
	public class Samples
	{
		// работа с файлом на диске через виртуальный каталог
		public void Test1()
		{			
			IStorageFolder folder = new LocalDiskFolder("~/Temp");
			var file = folder.GetFile("test.txt");
			file.WriteAllText("abcdef");
			var data = file.ReadAllBytes();
			file.Delete();
		}

		// работа с файлом в базе через виртуальный каталог
		//public void Test2(IDataContext dataContext)
		//{	
		//	IStorageFolder folder = new DatabaseFolder(dataContext);

		//	folder.CreateFile("db0e2a2f-43f1-4b00-bbee-940483c3e35b").WriteAllText("abcdef");
		//	dataContext.SubmitChanges();

		//	var file = folder.GetFile("db0e2a2f-43f1-4b00-bbee-940483c3e35b");
		//	var data = file.ReadAllBytes();
		//	file.Delete();
		//	dataContext.SubmitChanges();
		//}

		public void Test3()
		{
			//var storage = DI.GetService<IStorage<string>>();
			//var file = storage.GetFile("test/test.txt");
			//var data = file.ReadAllBytes();
		}


			//Guid folder_id = Guid.Empty;

//// Получение списка папок
//string rootpath = "SolutionSources";
//var allCs = FileStorageManager.DbFolders.Where(o => o.Path.StartsWith(rootpath));
//var ss = FileStorageManager.GetFolder(rootpath);
//var allCs2 = FileStorageManager.DbFolders.Where(o => o.ParentFolderID == ss.ID);
//Guid folderID = folder_id;
//var myFolder = FileStorageManager.GetFolder(folder_id);

//// Создание папки
//var newFolder = FileStorageManager.CreateFolder("temp", "SolutionSources/Autogenerate");
//if (!newFolder.CheckValid())
//	throw new Exception(String.Join(Environment.NewLine, newFolder.GetValidationMessages().Select(o => o.Message).ToArray()));
//A.Model.SubmitChanges();

//// Удаление папки
//FileStorageManager.DeleteFolder(ss.ID);
//A.Model.SubmitChanges();

//// Получение списка файлов
//var allCs3 = FileStorageManager.DbFiles.Where(o => o.ParentFolderID == ss.ID && o.Extension == ".cs");
//var allCs4 = FileStorageManager.DbFiles.Where(o => o.Path.StartsWith(rootpath) && o.Extension == ".cs");
//var cs1 = FileStorageManager.GetFile(rootpath+"/Autogenerate/app.cs");

//// Получение данных файла
//var data = cs1.GetBytes();
//var str = Encoding.UTF8.GetString(data);

//// Создание файла
//var file = FileStorageManager.CreateFile("my.xml", "Xml/Income");
//// Обновление данных файла
//file.Write(data);
//if (!file.CheckValid())
//	throw new Exception(String.Join(Environment.NewLine, file.GetValidationMessages().Select(o => o.Message).ToArray()));
//A.Model.SubmitChanges();

//FileStorageManager.DeleteFile(file.ID);
//A.Model.SubmitChanges();

		
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Nephrite.Web.FileStorage;
using System.IO;
using Nephrite.TextResources;

namespace Nephrite.Web.Controls
{
	public class FileList : Control
	{
		public Guid? FolderGUID { get; set; }
		public bool Enabled { get; set; }

		public FileList()
		{
			Enabled = true;
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			var parent = this.Parent;
			while (parent != null && parent.GetType() != typeof(HtmlForm) && parent.GetType() != typeof(Page))
				parent = parent.Parent;

			System.Web.UI.HtmlControls.HtmlForm myForm = parent as System.Web.UI.HtmlControls.HtmlForm;
			if (parent is HtmlForm)
				((HtmlForm)parent).Enctype = "multipart/form-data";

		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			if (Enabled)
			{
				Page.ClientScript.RegisterClientScriptBlock(GetType(), "FileListScript", @"function addFileInput(ClientID)
{
	var num = parseInt($('#reFileCount_' + ClientID).val());
	num++;
	$('#reFileCount_' + ClientID).val(num);
	$('#addMoreButton_' + ClientID).before(""<div><input type='file' name='reFile"" + num + ""_"" + ClientID + ""' id='reFile"" + num + ""_"" + ClientID + ""' style='width:100%;' /></div>"");
	$('#addMoreButton_' + ClientID).val(""" + TextResource.Get("Common.Controls.FileList.AttachMore", "Прикрепить ещё") + @""");
	$('#btnUpload_' + ClientID).show();
}
function deleteFile(guid, ClientID)
{
	$('#Delete_' + ClientID).val($('#Delete_' + ClientID).val() + ',' + guid);
	$('#s_' + guid).remove();
}
", true);
			}
		}

		public static string RenderLinks(Guid? guid)
		{
			string links = "";
			var files = FileStorageManager.DbFiles.Where(o => o.ParentFolderID == guid).OrderBy(o => o.Title);
			foreach (var file in files)
			{
				links += "<span id='s_" + file.ID + "'>";
				links += "<a href='/file.ashx?guid=" + file.ID + "'>" + file.Title + "</a></span><br />";
			}
			return links;
		}

		protected override void Render(HtmlTextWriter writer)
		{
			base.Render(writer);

			var files = FileStorageManager.DbFiles.Where(o => o.ParentFolderID == FolderGUID).OrderBy(o => o.Title);
			foreach (var file in files)
			{
				writer.Write("<span id='s_" + file.ID + "'>");
				writer.Write("<a href='/file.ashx?guid=" + file.ID + "'>" + file.Title + "</a>");
				if (Enabled)
				{
					writer.Write(" <a href='#' onclick='deleteFile(\"" + file.ID + "\", \"" + ClientID + "\")'><img style='vertical-align:middle' src='/i/n/delete.gif' /></a>");
				}
				writer.WriteBreak();
				writer.Write("</span>");
			}

			if (Enabled)
			{
				writer.Write("<input type='hidden' value='0' id='reFileCount_" + ClientID + "' />");
				writer.Write("<input type='hidden' value='' name='Delete_" + ClientID + "' id='Delete_" + ClientID + "' />");
				writer.Write("<input type='button' value='" + TextResource.Get("Common.Controls.FileList.AttachFile", "Прикрепить файл") +
					"' onclick='addFileInput(\"" + ClientID + "\");' id='addMoreButton_" + ClientID + "' style='margin-left:0px'/>");
				writer.Write("<span style='display:none' id='btnUpload" + ClientID + "'></span>");
			}
		}

		public string Save(Meta.MetaProperty property, IModelObject owner)
		{//@Sad
			IDbFolder objFolder;
			string folderName = property.Parent.Name + "." + property.Name;
			if (FolderGUID.HasValue)
				objFolder = FileStorageManager.GetFolder(FolderGUID.Value);
			else
			{
				var rootFolder = FileStorageManager.GetFolder("ObjectFiles/" + folderName);
				if (rootFolder == null)
				{
					rootFolder = FileStorageManager.CreateFolder(folderName, "ObjectFiles");
					rootFolder.CheckValid();
				}
				A.Model.SubmitChanges();
				objFolder = FileStorageManager.CreateFolder(owner.ObjectID.ToString(), "ObjectFiles/" + folderName);
				objFolder.CheckValid();
				FolderGUID = objFolder.ID;
				A.Model.SubmitChanges();
			}
			var f = owner.GetType().GetField("propertyChanges", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			Dictionary<string, ObjectPropertyChange> changes = null;
			if (f != null)
				changes = f.GetValue(owner) as Dictionary<string, ObjectPropertyChange>;

			string oldFileList = FileStorageManager.DbFiles.Where(o => o.ParentFolderID == FolderGUID).Select(o => o.Title).Join("\n");

			string deleteGuids = Page.Request.Form["Delete_" + ClientID];
			if (deleteGuids != null)
			{
				var guids = deleteGuids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(o => new Guid(o));
				foreach (var g in guids)
					FileStorageManager.DeleteFile(g);
				A.Model.SubmitChanges();
			}
			for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
			{
				HttpPostedFile file = HttpContext.Current.Request.Files[i];
				if (file.ContentLength == 0)
					continue;
				string fileName = Path.GetFileName(file.FileName);
				var dbFile = FileStorageManager.GetFile("ObjectFiles/" + folderName + "/" + objFolder.Title + "/" + fileName);
				if (dbFile == null)
					dbFile = FileStorageManager.CreateFile(fileName, "ObjectFiles/" + folderName + "/" + objFolder.Title);
				dbFile.Write((new BinaryReader(file.InputStream)).ReadBytes(file.ContentLength));
				dbFile.CheckValid();
			}
			A.Model.SubmitChanges();
			string newFileList = FileStorageManager.DbFiles.Where(o => o.ParentFolderID == FolderGUID).Select(o => o.Title).Join("\n");
			changes[property.Name] = new ObjectPropertyChange(property.Name, property.Caption, "", oldFileList);
			changes[property.Name].NewValueTitle = newFileList;
			return "";
		}
	}
}
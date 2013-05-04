using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Nephrite.Web;
using Nephrite.Core;
using System.Configuration;
using System.IO;
using System.Diagnostics;
using Nephrite.Metamodel.Model;
using Nephrite.Web.SPM;
using Nephrite.Metamodel.Controllers;
using Nephrite.Web.FileStorage;

namespace Nephrite.Metamodel.View.Utils
{
    public partial class generatewsdl : ViewControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SetTitle("Генерация Web Service Reference");

            if (!IsPostBack)
                tbNamespace.Text = AppMM.DBName() + "Service";

            //BackButton.Url = Html.ActionUrl<UtilsController>(c => c.Utils());
        }

        protected void bOK_Click(object sender, EventArgs e)
        {
            string wsdlname = ConfigurationManager.AppSettings["WsdlPath"];

            if (String.IsNullOrEmpty(wsdlname))
            {
                lMsg.Text = "В web.config не задана настройка WsdlPath - полный путь к файлу wsdl.exe";
                return;
            }

            if (String.IsNullOrEmpty(tbUrl.Text))
            {
                lMsg.Text = "Необходимо задать URL веб-сервиса";
                return;
            }

            if (String.IsNullOrEmpty(tbNamespace.Text))
            {
                lMsg.Text = "Необходимо задать пространство имен";
                return;
            }

            using (new Impersonation(ConfigurationManager.AppSettings["ElevatedAccessAccountDomain"],
                ConfigurationManager.AppSettings["ElevatedAccessAccountUsername"],
                ConfigurationManager.AppSettings["ElevatedAccessAccountPassword"]))
            {
                // Создать временный каталог
                string tempdir = AppDomain.CurrentDomain.BaseDirectory + "_temp";

                Directory.CreateDirectory(tempdir);

                string wsdlargs = String.Format("/nologo /namespace:{0} /o:\"{1}\" /urlkey:{0}Url \"{2}\"",
                tbNamespace.Text, tempdir + "\\" + tbNamespace.Text + ".cs", tbUrl.Text);

                Process wsdl = new Process();
                wsdl.StartInfo = new ProcessStartInfo(wsdlname, wsdlargs);
                wsdl.Start();
                wsdl.WaitForExit();
                string output = String.Format("{0} {1}\r\n", wsdlname, wsdlargs);
                lMsg.Text = output.Replace("\n", "<br />");
				if (!File.Exists(tempdir + "\\" + tbNamespace.Text + ".cs"))
					return;

				var folder = FileStorageManager.GetFolder(ModelAssemblyGenerator.SourceFolder);
				if (folder == null)
					lMsg.Text += "<br /><br />Папка " + ModelAssemblyGenerator.SourceFolder + " не существует, некуда сохранить файл!";
                else
                {
                    byte[] data = File.ReadAllBytes(tempdir + "\\" + tbNamespace.Text + ".cs");
                    var file = FileStorageManager.GetFile(ModelAssemblyGenerator.SourceFolder + "/" + tbNamespace.Text + ".cs");
                    if (file != null)
                    {
						file.Write(data);
						if (!file.CheckValid())
						{
							lMsg.Text += "<br /><br />Файл " + file.GetValidationMessages().Select(o => o.Message).Join("; ");
						}
						else
						{
							Nephrite.Web.Base.Model.SubmitChanges();
							lMsg.Text += "<br /><br />Файл " + file.Title + " обновлен.";
						}
                    }
                    else
                    {
						file = FileStorageManager.CreateFile(tbNamespace.Text + ".cs", ModelAssemblyGenerator.SourceFolder);
						file.Write(data);
						if (!file.CheckValid())
						{
							lMsg.Text += "<br /><br />Файл " + file.GetValidationMessages().Select(o => o.Message).Join("; ");
						}
						else
						{
							Nephrite.Web.Base.Model.SubmitChanges();
							lMsg.Text += "<br /><br />Файл " + file.Title + " создан.";
						}
                    }
                }
            }
        }
    }
}
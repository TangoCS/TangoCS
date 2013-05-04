using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.VisualStudio.TextTemplating;
using System.Reflection;
using System.IO;
using Nephrite.Web;
using System.Configuration;

namespace Nephrite.Metamodel.TextTransform
{
    public class Host : ITextTemplatingEngineHost
    {
        public System.CodeDom.Compiler.CompilerErrorCollection Errors = new System.CodeDom.Compiler.CompilerErrorCollection();

        public string ObjectTypeSysName { get; set; }
		public string CompilerVersion { get; set; }

        #region ITextTemplatingEngineHost Members

        public object GetHostOption(string optionName)
        {
            return null;
        }

        public bool LoadIncludeText(string requestFileName, out string content, out string location)
        {
            content = System.String.Empty;
            location = System.String.Empty;

			content = GetTemplateContent(requestFileName);
            return true;
        }

        public void LogErrors(System.CodeDom.Compiler.CompilerErrorCollection errors)
        {
            Errors.AddRange(errors);
        }

        public AppDomain ProvideTemplatingAppDomain(string content)
        {
            var d = AppDomain.CreateDomain("Nephrite.Metamodel Generation App Domain");
            d.SetData("ConnectionString", ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
            d.SetData("ObjectTypeSysName", ObjectTypeSysName);
            d.SetData("DbName", AppMM.DBName());
			d.SetData("ModelPath", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "_temp\\model.xml"));
            return d;
        }

        public string ResolveAssemblyReference(string assemblyReference)
        {
            //If the argument is the fully qualified path of an existing file,
            //then we are done. (This does not do any work.)
            //----------------------------------------------------------------
            //if (File.Exists(assemblyReference))
            //{
            string candidate = Path.GetFileNameWithoutExtension(assemblyReference);
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (a.GetName().Name == candidate)
                    return a.Location;
            }
            return assemblyReference;
            //}

            //Maybe the assembly is in the same folder as the text template that 
            //called the directive.
            //----------------------------------------------------------------
            //string candidate = Path.GetFileNameWithoutExtension(assemblyReference);
            //var a = Assembly.LoadWithPartialName(candidate);
            //if (a != null)
            //    return a.Location;
            
            //If we cannot do better - return the original file name.
            //return "";
        }

        public Type ResolveDirectiveProcessor(string processorName)
        {
            throw new NotImplementedException();
        }

        public string ResolveParameterValue(string directiveId, string processorName, string parameterName)
        {
            return String.Empty;
        }

        public string ResolvePath(string path)
        {
            return path;
        }

        public void SetFileExtension(string extension)
        {
        }

        public void SetOutputEncoding(System.Text.Encoding encoding, bool fromOutputDirective)
        {
            
        }

        public IList<string> StandardAssemblyReferences
		{
			get { return new String[1] { "System.dll" }; }
        }

        public IList<string> StandardImports
        {
            get { return new String[0]; }
        }

        string templateFile;
        public string TemplateFile
        {
            get { return templateFile; }
            set { templateFile = value; }
        }

        #endregion

        public string GetTemplateContent(string p)
        {
            var s = Assembly.GetCallingAssembly().GetManifestResourceStream("Nephrite.Metamodel.CodeTemplates." + p);
            if (s != null)
                using (var sr = new StreamReader(s))
                    return sr.ReadToEnd().Replace("<#@ template language=\"C#v3.5\" debug=\"True\" #>",
						"<#@ template language=\"C#" + CompilerVersion + "\" debug=\"True\" #>");
            return String.Empty;
        }
    }
}

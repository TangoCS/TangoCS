using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Reflection;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Text;
using Nephrite.Web.FileStorage;

namespace Nephrite.Metamodel
{
	public static class DynamicClassActivator
	{
		public static T CreateInstance<T>() where T : class
		{
			var t = typeof(T);
			if (table.ContainsKey(t))
				return Activator.CreateInstance(table[t]) as T;

			var ca = t.GetCustomAttributes(typeof(DynamicClassAttribute), false);
			if (ca.Length == 0)
				throw new Exception("У интерфейса " + t.FullName + " не задан атрибут DynamicClass");

			var dca = ca[0] as DynamicClassAttribute;

			var f = FileStorageManager.DbFiles.Where(o => o.Path.StartsWith(ModelAssemblyGenerator.SourceFolder) && o.Title == dca.CodeFile).FirstOrDefault();
			if (f == null)
			{
				table.Add(t, dca.TargetType);
			}
			else
			{
				string dllName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin\\" + ModelAssemblyGenerator.DllName);

				if (File.GetLastWriteTime(dllName) > f.LastModifiedDate)
				{
					table.Add(t, dca.TargetType);
				}
				else
				{
					var a = CompileDynamicClassDll(Encoding.UTF8.GetString(f.GetBytes()));
					Type t1 = a.GetType(dca.TargetType.FullName);
					table.Add(t, t1);
				}
			}

			return Activator.CreateInstance(table[t]) as T;
		}

		static Dictionary<Type, Type> table = new Dictionary<Type, Type>();

		static Assembly CompileDynamicClassDll(string code)
		{
			CSharpCodeProvider csCompiler = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", ModelAssemblyGenerator.Config.CompilerVersion } });

			// input params for the compiler
			CompilerParameters cp = new CompilerParameters();
			cp.ReferencedAssemblies.Add("mscorlib.dll");
			cp.ReferencedAssemblies.Add("System.dll");
			cp.ReferencedAssemblies.Add("System.Core.dll");
			cp.ReferencedAssemblies.Add("System.Configuration.dll");
			cp.ReferencedAssemblies.Add("System.Data.dll");
			cp.ReferencedAssemblies.Add("System.Data.Linq.dll");
			cp.ReferencedAssemblies.Add("System.Xml.dll");
			cp.ReferencedAssemblies.Add("System.Xml.Linq.dll");
			cp.ReferencedAssemblies.Add("System.Web.dll");
			cp.ReferencedAssemblies.Add("System.Web.Extensions.dll");
			cp.ReferencedAssemblies.Add("System.Web.Services.dll");
			cp.ReferencedAssemblies.Add("System.Transactions.dll");

			string binPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin");
			DirectoryInfo binDir = new DirectoryInfo(binPath);
			foreach (var dllFile in binDir.GetFiles("*.dll"))
				cp.ReferencedAssemblies.Add(dllFile.FullName);

			cp.ReferencedAssemblies.AddRange(ModelAssemblyGenerator.Config.ReferencedAssembly.ToArray());

			cp.GenerateExecutable = false;
			cp.GenerateInMemory = true;

			cp.CompilerOptions = "/target:library";

			cp.IncludeDebugInformation = true;

			List<string> otherFiles = new List<string>();
			otherFiles.Add(code);

			// Run the compiler and build the assembly
			CompilerResults cr = null;

			cr = csCompiler.CompileAssemblyFromSource(cp, otherFiles.ToArray());

			if (cr.Errors.HasErrors)
			{
				string error = "Всего ошибок и предупреждений: " + cr.Errors.Count.ToString() + "<br />";
				foreach (var errinfo in cr.Errors.Cast<CompilerError>().OrderBy(o => o.IsWarning))
				{
					int line = errinfo.Line;
					error += "<br /><br /><b>" + (errinfo.IsWarning ? "WARNING" : "ERROR") + " " + errinfo.ErrorText + " at " + errinfo.FileName + " line " + line.ToString() + " column " + errinfo.Column.ToString() + "</b><br /><br />";
					int minline = Math.Max(1, line - 30);
				}
				throw new Exception(error);
			}

			return cr.CompiledAssembly;
		}

		public static void Clear()
		{
			table.Clear();
		}
	}
}
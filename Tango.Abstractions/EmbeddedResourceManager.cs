using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Tango
{
	public interface IHasEmbeddedResources { }

	public static class EmbeddedResourceManager
	{
		public static string GetString(this IHasEmbeddedResources obj, string name, List<string> filters = null)
		{
			return GetString(obj.GetType(), name, filters);
		}

		public static string GetString(Type t, string name, List<string> filters = null)
		{
			return GetString(t.Assembly, name, filters);
		}

		public static string GetString(Assembly assembly, string name, List<string> filters = null)
		{	
			return GetString(assembly, name, filters, DefaultLineParsers);
		}

		static string GetString(Assembly assembly, string name, List<string> filters, params Func<State, bool>[] lineParsers)
		{
			if (filters == null) filters = new List<string>();
			if (filters.Count == 0) filters.Add("null");

			var resourceName = assembly.GetName().Name + "." + name;
			using (Stream stream = assembly.GetManifestResourceStream(resourceName))
			using (StreamReader reader = new StreamReader(stream))
			{
				if (!name.ToLower().EndsWith(".sql"))
					return reader.ReadToEnd();

				State state = new State { assembly = assembly, filters = filters };
				while ((state.line = reader.ReadLine()) != null)
				{
					foreach (var lineParser in lineParsers)
						if (lineParser(state)) break;

					if (state.skip == Skip.Default) state.sb.AppendLine(state.line);
					if (state.skip == Skip.SkipOnce) state.skip = Skip.Default;
				}
				return state.sb.ToString();
			}
		}

		static readonly Func<State, bool>[] DefaultLineParsers = new Func<State, bool>[] { ProcessInclude, ProcessFilter };

		static bool ProcessInclude(State state)
		{
			if (state.skip == Skip.Default && state.line.StartsWith("--#include"))
			{
				var s = GetString(state.assembly, state.line.Substring(10).Trim(), state.filters, DefaultLineParsers);
				state.sb.AppendLine(s);
				state.skip = Skip.SkipOnce;
				return true;
			}
			return false;
		}

		static bool ProcessFilter(State state)
		{
			var line = state.line;
			if (line.StartsWith("--#filter"))
			{
				line = line.Trim().Substring(9);
				line = line.Remove(line.Length - 1);
				var blockNames = line.Split(',').Select(s => s.Trim());
				if (state.filters == null || blockNames.Intersect(state.filters).Count() == 0)
					state.skip = Skip.Skip;
				return true;
			}
			else if (line.StartsWith("--}"))
			{
				state.skip = Skip.SkipOnce;
				return true;
			}
			return false;
		}

		class State
		{
			public Assembly assembly;
			public StringBuilder sb = new StringBuilder();
			public string line;
			public Skip skip = Skip.Default;
			public List<string> filters;
		}
		
		enum Skip { SkipOnce, Skip, Default }
	}
}

using Tango.Data;
using System.IO;
using Tango.AccessControl;
using Tango.Localization;
using Tango.UI.Navigation;

namespace Tango.FileStorage.Std
{
	public class FileStorageMenuDataLoader : DefaultMenuDataLoader
	{
        IDataContext _dc;

        public FileStorageMenuDataLoader(IHostingEnvironment env, IMenuItemResolverCollection resolvers,
			IAccessControl accessControl, IResourceManager resource, IDataContext dc) : base(env, resolvers, resource)
        {
            _dc = dc;
        }

		protected override string GetData(string name)
		{
			var fileName = name + ".json";
			var folder = new DatabaseFolder(_dc);
			var file = folder.GetFileByName(fileName);
			if (file == null)
			{
				var path = _env.MapPath("data" + Path.DirectorySeparatorChar + fileName);
				if (File.Exists(path)) return File.ReadAllText(path);
				return null;
			}
			return file.ReadAllText();
		}
	}
}

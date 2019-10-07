using Tango.FileStorage.Std.Model;
using Tango.UI;
using Tango.UI.Std;
using System;

namespace Tango.FileStorage.Std.Views
{
    [OnAction(typeof(N_FileData), "delete")]
    public class n_filedata_delete : default_delete<N_FileData, Guid> { }
}

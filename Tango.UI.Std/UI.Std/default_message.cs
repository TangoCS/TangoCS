namespace Tango.UI.Std
{
	public class default_message : ViewPagePart
	{
		string _title;
		string _message;

		public default_message(string title, string message)
		{
			_title = title;
			_message = message;
		}

		public override void OnLoad(ApiResponse response)
		{
			response.AddWidget("contentbody", w => w.Write(_message));
			response.AddWidget("contenttitle", w => w.Write(_title));
			response.AddWidget("#title", w => w.Write(_title));
		}
	}
}

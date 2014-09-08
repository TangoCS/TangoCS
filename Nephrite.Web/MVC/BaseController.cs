using System;
using System.Web.UI;
using System.Linq.Expressions;
using System.Web;
using System.IO;
using System.Xml.Linq;
using System.Reflection;
using System.Configuration;
using Nephrite.Web.SPM;
using Nephrite.Web.Controls;
using Nephrite.Web.SettingsManager;
using Nephrite.Meta;
using Nephrite.Web.Office;
using Nephrite.Web.Layout;

namespace Nephrite.Web
{
    public abstract class BaseController
    {
		Control webPart = null;
        protected HttpRequest Request;
		protected HttpResponse Response;
		public BaseController()
		{
			if (HttpContext.Current != null)
			{
				Request = HttpContext.Current.Request;
				Response = HttpContext.Current.Response;
			}
		}
		public Control WebPart
        {
            get { return webPart; }
            set
            {
                webPart = value;
            }
        }

		//public DataContext DataContext { get; set; }
        //protected string ViewPath = "";

		public static void RedirectTo<TController>(Expression<Action<TController>> action) where TController : BaseController, new()
		{
		    RedirectTo<TController>(action, null);
		}

		public static void RedirectTo<TController>(Expression<Action<TController>> action, string anchor) where TController : BaseController, new()
		{
		    string url = HtmlHelperBase.Instance.ActionUrl<TController>(action);
		    if (anchor != null)
		        url += "&anchorlocation=" + anchor;

			/// HttpContext.Current.Request.Url.AbsoluteUri может не соответствовать реальному URL, если сервер сидит за прокси, которая меняет порт
			string newUrl = "";// HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "");
			if (!url.StartsWith("/"))
				url = "/" + url;
            newUrl = newUrl + url;
			HttpContext.Current.Response.Redirect(newUrl);
		}

        public void RenderMessage(string message)
        {
            string path = Settings.BaseControlsPath + "Message/show.ascx";
			ViewControl control = (ViewControl)webPart.Page.LoadControl(path);

			control.SetViewData(new MessageViewData { Title = "Внимание!", Text = message });

			webPart.Controls.Add(control);
        }
		public void RenderMessage(string title, string message)
		{
			string path = Settings.BaseControlsPath + "Message/show.ascx";
			ViewControl control = (ViewControl)webPart.Page.LoadControl(path);

			control.SetViewData(new MessageViewData { Title = title, Text = message });

			webPart.Controls.Add(control);
		}


		public static Result Run(Control control, string mode, string action, bool disableScriptManager, bool skipCreateMdm)
		{
			TraceContext tc = HttpContext.Current.Trace;

			if (!disableScriptManager)
			{
				ScriptManager sm = new ScriptManager();
				sm.ScriptMode = ScriptMode.Release;
				control.Controls.Add(sm);
			}

			if (!skipCreateMdm)
			{
				ModalDialogManager mdm = new ModalDialogManager();
				control.Controls.Add(mdm);
			}

			control.Page.ClientScript.RegisterStartupScript(control.GetType(), "Error handling", @"
<script type='text/javascript' language='javascript'>
    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);
                
    function EndRequestHandler(sender, args)
    {
        if (args.get_error() != undefined)
        {
            var errorMessage = args.get_error().message;
            var rd = args.get_response().get_responseData();
			args.set_errorHandled(true);
            if (args._error.httpStatusCode == 0) return;
            alert('Ошибка при обращении к серверу: ' + errorMessage);
			hideModalPopup();
        }
    }
</script>");

			Type controllerType = ControllerFactory.GetControllerType(mode);
			if (controllerType == null)
			{
				return new Result(-1, String.Format(Resources.Common.ControllerNotFound, mode + "controller"));
			}
			BaseController controller = (BaseController)Activator.CreateInstance(controllerType);
			controller.WebPart = control;

			MemberInfo[] methodArray = controllerType.GetMember(action, MemberTypes.Method,
				BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
			if (methodArray.Length != 1)
			{
				return new Result(-1, String.Format(Resources.Common.ControllerMethodNotFound, action, controllerType.FullName));
			}
			MethodInfo method = (MethodInfo)methodArray[0];

			if (ConfigurationManager.AppSettings["DisableSPM"] == null && action.ToLower() != "logoff")
			{
				string checkaction = action;
				object[] ca = method.GetCustomAttributes(typeof(SpmActionNameAttribute), true);
				if (ca != null && ca.Length == 1)
				{
					var san = ca[0] as SpmActionNameAttribute;
					checkaction = san.Name;
				}

				//MetaOperation mo = Base.Meta.GetOperation(mode, checkaction);
				if (!String.IsNullOrEmpty(checkaction) && !ActionSPMContext.Current.Check(mode + "." + checkaction, 1))
				{
					if (AppSettings.Get("loginurl").IsEmpty())
						return new Result( -2, "Недостаточно полномочий для выполнения операции.");
					else
						HttpContext.Current.Response.Redirect(AppSettings.Get("loginurl").AddQueryParameter("returnurl", Query.CreateReturnUrl()));
					return new Result(0, "");
				}
			}
			
			ParameterInfo[] mp = method.GetParameters();
			object[] p = new object[mp.Length];
			for (int i = 0; i < mp.Length; i++)
			{
				string val = mp[i].Name.ToLower() == "id" ? Url.Current.GetString("o" + mp[i].Name) : Url.Current.GetString(mp[i].Name);
				val = HttpUtility.UrlDecode(val);
				try
				{
					if (mp[i].ParameterType == typeof(Guid))
						p[i] = val.ToGuid();
					else
						p[i] = Convert.ChangeType(val, mp[i].ParameterType);
				}
				catch
				{
					switch (mp[i].ParameterType.Name)
					{
						case "DateTime":
							p[i] = DateTime.Today;
							break;
						case "Int32":
							p[i] = 0;
							break;
						case "Boolean":
							p[i] = false;
							break;
						default:
							throw;
					}
				}
			}

			
			method.Invoke(controller, p);
			return new Result(0, "");
		}
    }

	public class MessageViewData
	{
		public string Title { get; set; }
		public string Text { get; set; }
	}
}

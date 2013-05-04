using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Web;

namespace Nephrite.Metamodel.Model
{
    partial class MM_FormView : BaseModelObject, IMM_ModelElement
    {
        public string Icon
        {
            get { return "icon_formview.gif"; }
        }
		public string ElementSysName
		{
			get { return SysName + " : " + BaseClass; }
		}
        public int ID
        {
            get { return FormViewID; }
        }

		public Guid ObjectGUID
		{
			get { return Guid; }
		}

        public int Level { get; set; }

        public string ClassName { get { return "Форма"; } }

        public string FullSysName
        {
            get
            {
                if (PackageID.HasValue)
                    return MM_Package.FullSysName + "." + SysName + " (" + Title + ")";
                else
                    return MM_ObjectType.FullSysName + "." + SysName + " (" + Title + ")";
            }
        }

		public string FullTitle
		{
			get
			{
				if (PackageID.HasValue)
					return MM_Package.Title + "." + Title;
				else
					return MM_ObjectType.Title + "." + Title;
			}
		}

		string _controlPath = null;
        public string ControlPath
        {
            get
            {
				if (_controlPath == null)
				{
					string type = "ascx";
					if (TemplateTypeCode == TemplateType.Aspx)
						type = "aspx";
					else if (TemplateTypeCode == TemplateType.Ashx)
						type = "ashx";
					else if (TemplateTypeCode == TemplateType.Asmx)
						type = "asmx";
					else if (TemplateTypeCode == TemplateType.Svc)
						type = "svc";
					
					_controlPath = Settings.ControlsPath + "/" + (ObjectTypeID.HasValue ? MM_ObjectType.ControlPath : MM_Package.ControlPath) + "/" + SysName + "." + type;
				}
				return _controlPath;
            }
        }

		public override void WriteDeleteObjectHistory()
		{
			
		}

		public override void WriteInsertObjectHistory()
		{
			AppMM.DataContext.MM_FormView_CreateHistoryVersion(FormViewID);
		}

		public override void WriteUpdateObjectHistory()
		{
			
		}

		public string BC
		{
			get { return BaseClass.Replace("Nephrite.Web.", ""); }
		}

		public bool IsSingleObjectView
		{
			get { return BC == "ViewControl<{0}>" || BC == "ViewControl<V_{0}>" || BC == "ViewControl<HST_{0}>"; }
		}
	}
}

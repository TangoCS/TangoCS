using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Web.SPM;
using Nephrite.Web;

namespace Nephrite.Metamodel.Model
{
    partial class MM_ObjectType : IMM_ModelElement
    {
        bool? isSpmEnabled;

        public bool IsSPMEnabled
        {
            get
            {
                if (isSpmEnabled == null)
                    isSpmEnabled = AppMM.DataContext.SPM_Actions.Any(a => a.SystemName == _SysName);
                return isSpmEnabled.Value;
            }
        }

        SPM_Action[] actions;
        public SPM_Action[] SPM_Actions
        {
            get
            {
                if (actions == null)
                    actions = (from b in AppMM.DataContext.SPM_Actions
                               join asso in AppMM.DataContext.SPM_ActionAssos on b.ActionID equals asso.ParentActionID
                               where b.SystemName == SysName
                               orderby asso.SPM_Action.Title
                               select asso.SPM_Action).ToArray();
                return actions;
            }
        }

        public string Icon
        {
            get { return IsTemplate ? "icon_templateobjecttype.gif" : "icon_objecttype.gif"; }
        }

        public int ID
        {
            get { return ObjectTypeID; }
        }

        public int Level { get; set; }

        public string ClassName { get { return "Тип объекта"; } }

        public MM_ObjectProperty[] PrimaryKey
        {
            get { return MM_ObjectProperties.Where(o => o.IsPrimaryKey).OrderBy(o => o.SeqNo).ToArray(); }
        }

        public string FullSysName
        {
            get { return MM_Package.FullSysName + "." + SysName + " (" + Title + ")"; }
        }

		public string FullTitle
		{
			get { return Title + " [" + SysName + "]"; }
		}

		public string ElementSysName
		{
			get { return SysName; }
		}

        /// <summary>
        /// Отслеживается ли история изменений объекта
        /// </summary>
        bool? trackHistory;
        public bool TrackHistory
        {
            get
            {
                if (trackHistory.HasValue)
                    return trackHistory.Value;

                if (IsEnableObjectHistory)
                {
                    trackHistory = true;
                    return true;
                }
                foreach (var op in MM_ObjectProperties.Where(o => o.RefObjectPropertyID.HasValue &&
                    o.RefObjectProperty.IsAggregate && o.RefObjectTypeID != o.ObjectTypeID).ToList())
                {
                    if (op.RefObjectProperty.RefObjectPropertyID.HasValue && op.RefObjectProperty.RefObjectPropertyID == op.ObjectPropertyID &&
                        op.IsAggregate)
                        continue;
                    trackHistory = op.RefObjectType.TrackHistory;
                    return trackHistory.Value;
                }
                trackHistory = false;
                return false;
            }
        }

        MM_ObjectType historyParentClass;
        public MM_ObjectType HistoryParentClass
        {
            get
            {
                if (historyParentClass == null)
                {
                    foreach (var op in MM_ObjectProperties.Where(o => o.RefObjectPropertyID.HasValue &&
                        o.RefObjectProperty.IsAggregate && o.RefObjectTypeID != o.ObjectTypeID))
                    {
                        historyParentClass = op.RefObjectType.HistoryParentClass;
                        return historyParentClass;
                    }
                    historyParentClass = this;
                }
                return historyParentClass;
            }
        }

        public bool IsMultiLingual
        {
            get { return MM_ObjectProperties.Any(o => o.IsMultilingual); }
        }

        public MM_ObjectProperty ParentProperty
        {
            get { return AllProperties.FirstOrDefault(o => o.SysName == "Parent" && o.RefObjectTypeID.HasValue && o.UpperBound == 1); }
        }

        public bool IsMovable
        {
            get { return AllProperties.Any(o => o.SysName == "SeqNo" && o.LowerBound == 1 && o.UpperBound == 1 && o.TypeCode == ObjectPropertyType.Number); }
        }

        public IEnumerable<MM_ObjectProperty> AllProperties
        {
            get { return BaseObjectTypeID.HasValue ? BaseObjectType.MM_ObjectProperties.Union(MM_ObjectProperties) : MM_ObjectProperties; }
        }

        public string ControlPath
        {
            get
            {
                return MM_Package.ControlPath + "/" + SysName;
            }
        }

		public bool HasTableFilter
		{
			get
			{

				return ControllerFactory.GetControllerType(SysName).GetMethods().Any(o => o.Name == "FilterTable");
			}
		}

		//public SPM_Subject LastModifiedUser
		//{
		//	get { return AppSPM.DataContext.SPM_Subjects.SingleOrDefault(o => o.SubjectID == LastModifiedUserID); }
		//}
    }
}

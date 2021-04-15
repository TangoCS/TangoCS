using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dapper;
using Newtonsoft.Json;
using Tango.Data;
using Tango.Html;
using Tango.Identity;
using Tango.UI;
using Tango.UI.Controls;
using Tango.UI.Std;

namespace Tango.Mail
{
    public static class MailSettingsHelper
    {
        public static IEnumerable<MethodSettings> GetMethodSettingsByTypesKey(string typesKey)
        {
            var cache = new TypeCache();
            var types = cache.Get(typesKey).SelectMany(t =>
            {
                return t.GetMethods()
                    .Where(m => m.GetCustomAttribute<DescriptionAttribute>() != null)
                    .Select(m => new MethodSettings
                    {
                        ClassName = t.FullName,
                        MethodName = m.Name,
                        Params = new Dictionary<string, object>()
                    });
            });

            return types;
        }

        public static Dictionary<string, string> GetFullNameDictionary(string typesKey)
        {
            var cache = new TypeCache();
            var list = cache.Get(typesKey).SelectMany(t => {
                var clsName = t.GetCustomAttribute<DescriptionAttribute>()?.Description ?? t.Name;
                return t.GetMethods()
                    .Where(m => m.GetCustomAttribute<DescriptionAttribute>() != null)
                    .Select(m => {
                        var mName = m.GetCustomAttribute<DescriptionAttribute>().Description;
                        return ($"{t.FullName}|{m.Name}", $"{clsName}.{mName}");
                    });
            });

            var dict = list.ToDictionary(i => i.Item1, i => i.Item2);

            return dict;
        }

        public static string GetMethodName(string typesKey, string methodJson)
        {
            var methods = MailSettingsHelper.GetFullNameDictionary(typesKey);
            var result = string.Empty;
            var methodSettingsCollection = JsonConvert.DeserializeObject<MethodSettingsCollection>(methodJson);
            foreach (var ms in methodSettingsCollection.MethodSettings)
            {
                var msKey = $"{ms.ClassName}|{ms.MethodName}";
                if (methods.TryGetValue(msKey, out var r))
                    result += $"{r}{Environment.NewLine}";
            }

            return result;
        }
    }
    
    [OnAction(typeof(MailSettings), "viewlist")]
    public class MailSettings_viewlist : default_list_rep<MailSettings>
    {
        protected override Func<string, Expression<Func<MailSettings, bool>>> SearchExpression => s => 
            o => o.Title.ToLower().Contains(s.ToLower());

        protected override IEnumerable<MailSettings> GetPageData()
        {
            var items = base.GetPageData();
            foreach (var item in items)
            {
                if (!string.IsNullOrEmpty(item.PreProcessingMethod))
                {
                    item.PreProcessingMethod = MailSettingsHelper.GetMethodName(MailTypeCacheKeys.PreProcessingMethod,
                        item.PreProcessingMethod);
                }
                if (!string.IsNullOrEmpty(item.PostProcessingMethod))
                {
                    item.PostProcessingMethod = MailSettingsHelper.GetMethodName(MailTypeCacheKeys.PostProcessingMethod,
                        item.PostProcessingMethod);
                }
            }

            return items;
        }

        protected override void FieldsInit(FieldCollection<MailSettings> fields)
        {
            fields.AddCellWithSortAndFilter(o => o.ID, o => o.ID);
            fields.AddCellWithSortAndFilter(o => o.Title, o => o.Title);
            fields.AddCellWithSortAndFilter(o => o.MailTemplateTitle, o=>o.MailTemplateTitle);
            fields.AddCellWithSortAndFilter(o => o.MailCategoryTitle, o=>o.MailCategoryTitle);
            fields.AddCellWithSortAndFilter(o => o.AttemptsToSendCount, o=>o.AttemptsToSendCount);
            fields.AddCellWithSortAndFilter(o => o.TimeoutValue, o=>o.TimeoutValue);
            fields.AddCellWithSortAndFilter(o => o.PreProcessingMethod, o => o.PreProcessingMethod);
            fields.AddCellWithSortAndFilter(o => o.PostProcessingMethod, o => o.PostProcessingMethod);
            fields.AddCellWithSortAndFilter(o => o.SystemName, o => o.SystemName);
            fields.AddCell(o => o.SendMailDayInterval, o => o.SendMailDayInterval);
            fields.AddCell(o => o.SendMailStartInterval, o => o.SendMailStartInterval);
            fields.AddCell(o => o.SendMailFinishInterval, o => o.SendMailFinishInterval);
            fields.AddActionsCell(
                o => al => al.ToView<MailSettings>(AccessControl, o.ID)
                    .WithImage("mail").WithTitle("Тема и Текст письма"),
                o => al => al.ToEdit<MailSettings>(AccessControl, o.ID)
                    .WithImage("edit").WithTitle("Редактировать"),
                o => al => al.ToDelete<MailSettings>(AccessControl, o.ID)
                    .WithImage("delete").WithTitle("Удалить"),
                o =>
                {
                    if (!o.HasTemplate)
                    {
                        return al =>
                        {
                            al.ToView<MailSettings>(AccessControl, o.ID)
                                .WithImage("warning").WithTitle("На текущий момент для письма не определен шаблон");
                        };
                    }

                    return null;
                });
        }
    }
    
    [OnAction(typeof(MailSettings), "view")]
    public class MailSettings_view : default_view_rep<MailSettings, int, IRepository<MailSettings>>
    {
        private MailSettingsTemplate_list _mailSettingsTemplateList;
        private string _preProcessingMethods;
        private string _postProcessingMethods;

        public override void OnInit()
        {
            base.OnInit();
            _mailSettingsTemplateList = CreateControl<MailSettingsTemplate_list>("mstlst", c => {
                c.MailSettingsID = ViewData.MailSettingsID;
                c.Sections.RenderContentTitle = false;
            });

            if (!string.IsNullOrEmpty(ViewData.PreProcessingMethod))
            {
                ViewData.PreProcessingMethod = MailSettingsHelper.GetMethodName(MailTypeCacheKeys.PreProcessingMethod,
                    ViewData.PreProcessingMethod);
            }
            
            if (!string.IsNullOrEmpty(ViewData.PostProcessingMethod))
            {
                ViewData.PostProcessingMethod = MailSettingsHelper.GetMethodName(MailTypeCacheKeys.PostProcessingMethod,
                    ViewData.PostProcessingMethod);
            }
        }

        protected MailSettingsFields.DefaultGroup Group { get; set; }
        protected override void Form(LayoutWriter w)
        {
            w.FieldsBlockStd(() =>
            {
                w.PlainText(Group.Title);
                w.PlainText(Group.MailCategoryTitle);
                w.PlainText("Методы предварительной обработки", () =>
                {
                    w.Write(ViewData.PreProcessingMethod);
                });
                w.PlainText("Методы постобработки", () =>
                {
                    w.Write(ViewData.PostProcessingMethod);
                });
                // w.PlainText(Group.PreProcessingMethod);
                // w.PlainText(Group.PostProcessingMethod);
                w.PlainText(Group.TimeoutValue);
                w.PlainText(Group.SystemName);
                w.PlainText(Group.SendMailDayInterval);
                w.PlainText(Group.SendMailStartInterval);
                w.PlainText(Group.SendMailFinishInterval);
                w.PlainText(Group.AttemptsToSendCount);
                w.PlainText(Group.LastModifiedDate);
            });
        }

        protected override void LinkedData(LayoutWriter w)
        {
            w.GroupTitle(() =>
            {
                w.Write("Шаблон письма");
                if (!ViewData.HasTemplate)
                {
                    w.Write("&nbsp;");
                    w.ActionImage(al => al.ToCreateNew<MailSettingsTemplate>(AccessControl)
                        .WithArg("s", ViewData.ID)
                        .WithTitle("Добавить шаблон")
                        .WithImage("new2"));
                }
            });
            _mailSettingsTemplateList.Render(w);
        }
    }
    
    [OnAction(typeof(MailSettings), "createnew")]
    [OnAction(typeof(MailSettings), "edit")]
    public class MailSettings_edit : default_edit_rep<MailSettings, int, IMailSettingsRepository>
    {
        [Inject] protected IUserIdAccessor<object> UserIdAccessor { get; set; }

        private const string PreProcessMethodID = "preprocessmethod";
        private const string PostProcessMethodID = "postprocessmethod";
        
        private IEnumerable<SelectListItem> _selectMailTemplate;
        private IEnumerable<SelectListItem> _selectMailCategory;
        private List<(MethodSettingsField, MethodSettings)> _preProcessMethodFields;
        private List<(MethodSettingsField, MethodSettings)> _postProcessMethodFields;
        
        public override void OnInit()
        {
            base.OnInit();

            _preProcessMethodFields = GenetateFields(MailTypeCacheKeys.PreProcessingMethod, PreProcessMethodID, ViewData.PreProcessingMethod);
            _postProcessMethodFields = GenetateFields(MailTypeCacheKeys.PostProcessingMethod, PostProcessMethodID, ViewData.PostProcessingMethod);

            _selectMailTemplate = Database.Connection.Query<MailTemplate>(Repository.GetMailTemplateSql()).ToList()
                .OrderBy(x => x.MailTemplateID)
                .Select(o => new SelectListItem(o.Title, o.MailTemplateID));
            _selectMailCategory = Database.Connection.Query<C_MailCategory>(Repository.GetMailCategorySql()).ToList()
                .OrderBy(x => x.MailCategoryID)
                .Select(o => new SelectListItem(o.Title, o.MailCategoryID));
        }

        private List<(MethodSettingsField, MethodSettings)> GenetateFields(string typeCacheKey, string id, string json)
        {
            var fields = new List<(MethodSettingsField, MethodSettings)>();
            
            var methodSettings = MailSettingsHelper.GetMethodSettingsByTypesKey(typeCacheKey);
            
            if (!string.IsNullOrEmpty(json))
            {
                var methodSettingsCollection = JsonConvert.DeserializeObject<MethodSettingsCollection>(json);
                
                var cnt = 0;
                foreach (var ms in methodSettingsCollection.MethodSettings)
                {
                    var cntr = CreateControl<MethodSettingsField>($"{id}{cnt + 1}",
                        c => { c.TypesKey = typeCacheKey; });
                    fields.Add((cntr, ms));
                    cnt++;
                }

                var excl = methodSettings.Except(methodSettingsCollection.MethodSettings);
                foreach (var ms in excl)
                {
                    CreateControl<MethodSettingsField>($"{id}{cnt + 1}",
                        c => { c.TypesKey = typeCacheKey; });
                    cnt++;
                }
            }
            else
            {
                var cntr = CreateControl<MethodSettingsField>($"{id}1", c => { c.TypesKey = typeCacheKey; });
                
                // TODO: нужно решить что-то с умолчательными значениями. Сейчас падает (14.04.2021)
                
                if (methodSettings.Any())
                {
                    fields.Add((cntr, methodSettings.First()));
                    if (methodSettings.Count() > 1)
                    {
                        for (var i = 1; i < methodSettings.Count(); i++)
                        {
                            CreateControl<MethodSettingsField>($"{id}{i + 1}",
                                c => { c.TypesKey = typeCacheKey; });
                        }
                    }
                }
            }

            return fields;
        }

        protected MailSettingsFields.DefaultGroup Group { get; set; }
        
        protected override void Form(LayoutWriter w)
        { 
            w.FieldsBlockStd(() =>
            {
                w.TextBox(Group.Title);
                if(CreateObjectMode)
                    w.DropDownList(Group.MailTemplateID, _selectMailTemplate);
                w.DropDownList(Group.MailCategoryID, _selectMailCategory);

                w.FormField(Group.PreProcessingMethod, () =>
                {
                    var cnt = 0;
                    foreach (var (field, ms) in _preProcessMethodFields)
                    {
                        field.Render(w, new MethodSettings
                        {
                            ClassName = ms.ClassName,
                            MethodName = ms.MethodName,
                            Params = ms.Params
                        });
                        // if (cnt > 0)
                        // {
                        //     w.Span(a => a.ID($"{field.ID}_deletebtn").Class("cal-openbtn").Title("Удалить")
                        //         .OnClick("this.removeChild(this.parentNode);"), () => w.Icon("delete"));
                        // }
                        cnt++;
                    }
                    
                    w.A(a => a.OnClickPostEvent(OnPreMethodsAdd), "добавить");
                });
                
                w.FormField(Group.PostProcessingMethod, () =>
                {
                    var cnt = 0;
                    foreach (var (field, ms) in _postProcessMethodFields)
                    {
                        field.Render(w, new MethodSettings
                        {
                            ClassName = ms.ClassName,
                            MethodName = ms.MethodName,
                            Params = ms.Params
                        });
                        // if (cnt > 0)
                        // {
                        //     w.Span(a => a.ID($"{field.ID}_deletebtn").Class("cal-openbtn").Title("Удалить")
                        //         .OnClick("this.removeChild(this.parentNode);"), () => w.Icon("delete"));
                        // }
                        cnt++;
                    }
                    
                    w.A(a => a.OnClickPostEvent(OnPostMethodsAdd), "добавить");
                });
                w.TextBox(Group.TimeoutValue);
                w.TextBox(Group.SystemName);
                w.TextBox(Group.SendMailDayInterval);
                w.TextBox(Group.SendMailStartInterval);
                w.TextBox(Group.SendMailFinishInterval);
                w.TextBox(Group.AttemptsToSendCount);
            });
        }

        protected override void PostProcessFormData(ApiResponse response, ValidationMessageCollection val)
        {
            var preProcessingJson = CreateMethodSettingsColletionJson(PreProcessMethodID);
            ViewData.PreProcessingMethod = preProcessingJson;
            
            var postProcessingJson = CreateMethodSettingsColletionJson(PostProcessMethodID);
            ViewData.PostProcessingMethod = postProcessingJson;
        }

        private string CreateMethodSettingsColletionJson(string key)
        {
            var processings = Context.AllArgs
                .Where(i => i.Key.StartsWith(key) && !string.IsNullOrEmpty(i.Value?.ToString()))
                .GroupBy(x => x.Key.Split('_')[0]);

            if (processings.Any())
            {
                var cnt = 0;
                var processingsColl = new MethodSettingsCollection
                {
                    MethodSettings = new MethodSettings[processings.Count()]
                };
                foreach (var preProcessing in processings)
                {
                    var className = string.Empty;
                    var methodName = string.Empty;
                    var parms = new Dictionary<string, object>();
                    foreach (var pr in preProcessing)
                    {
                        if (pr.Key.EndsWith("_ddl"))
                        {
                            var spl = pr.Value.ToString().Split('|');
                            className = spl[0];
                            methodName = spl[1];
                        }
                        else
                        {
                            var spl = pr.Key.Split(new[] {"_parm_"}, StringSplitOptions.RemoveEmptyEntries);
                            if (!parms.ContainsKey(spl[1]))
                            {
                                parms.Add(spl[1], pr.Value);
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(className) || string.IsNullOrEmpty(methodName))
                        continue;

                    processingsColl.MethodSettings[cnt] = new MethodSettings
                    {
                        ClassName = className,
                        MethodName = methodName,
                        Params = parms
                    };
                    cnt++;
                }

                var json = JsonConvert.SerializeObject(processingsColl);
                return json;
            }

            return null;
        }

        public void OnPreMethodsAdd(ApiResponse response)
        {
            AddMethodField(response, PreProcessMethodID);
        }
        
        public void OnPostMethodsAdd(ApiResponse response)
        {
            AddMethodField(response, PostProcessMethodID);
        }

        public void AddMethodField(ApiResponse response, string id)
        {
            var cnt = Context.AllArgs.Count(x => x.Key.StartsWith(id) && x.Key.EndsWith("_ddl"));

            var (lastPm, methodSettings) = _preProcessMethodFields.Last();
            lastPm.ID = $"{id}{cnt + 1}";
            
            response.WithNamesFor(this);
            response.AddAdjacentWidget($"{id}{cnt}_fld", lastPm.ID, AdjacentHTMLPosition.AfterEnd, w => {
                lastPm.Render(w, new MethodSettings{
                    ClassName = methodSettings.ClassName,
                    MethodName = methodSettings.MethodName,
                    Params = methodSettings.Params
                });
            });
            response.AddChildWidget($"{id}{cnt+1}_fld", $"{lastPm.ID}_deletebtn", w => {
                w.Span(a => a.ID($"{lastPm.ID}_deletebtn").Class("cal-openbtn").Title("Удалить")
                    .OnClick("this.parentNode.parentNode.removeChild(this.parentNode);"), () => w.Icon("delete"));
            });
        }

        protected override void FieldsPreInit()
        {
            base.FieldsPreInit();
            Group.MailTemplateID.CanRequired = CreateObjectMode;
            //Group.PreProcessingMethod.SetValueProvider(_preProcessMethod);
            //Group.PostProcessingMethod.SetValueProvider(_postProcessMethod);
        }

        protected override void AfterSaveEntity()
        {
            base.AfterSaveEntity();
            
            if (CreateObjectMode)
            {
                var rep = Database.Repository<MailSettingsTemplate>();
                var mailSettingsTemplate = new MailSettingsTemplate
                {
                    MailTemplateID = Group.MailTemplateID.Value,
                    MailSettingsID = ViewData.MailSettingsID,
                    StartDate = MailConstants.StartDate,
                    FinishDate = MailConstants.FinishDate
                };
                rep.Create(mailSettingsTemplate);
            }
        }

        protected override MailSettings GetNewEntity()
        {
            var obj = new MailSettings();
            SetDefaultValues(obj);
            return obj;
        }

        protected override void SetDefaultValues(MailSettings obj)
        {
            obj.CreateDate = DateTime.Now;
            obj.LastModifiedDate = DateTime.Now;
            obj.LastModifiedUserID = UserIdAccessor.CurrentUserID;
        }

        protected override MailSettings GetExistingEntity()
        {
            var obj = base.GetExistingEntity();
            obj.LastModifiedDate = DateTime.Now;
            obj.LastModifiedUserID = UserIdAccessor.CurrentUserID;

            return obj;
        }
    }
    
    [OnAction(typeof(MailSettings), "delete")]
    public class MailSettings_delete : default_delete<MailSettings, int>
    {
    }
}
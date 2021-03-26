using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Tango.AccessControl;
using Tango.Data;
using Tango.UI;
using Tango.UI.Controls;
using Tango.UI.Std;

namespace Tango.Mail
{
    [OnAction("mailCategory", "viewlist")]
    public class dto_c_mailCategory_allviews : default_list_rep<DTO_C_MailCategory>
    {
        [Inject] protected IMailRepository MailRepository { get; set; }
        protected override Func<string, Expression<Func<DTO_C_MailCategory, bool>>> SearchExpression => s => 
            o => o.Title.ToLower().Contains(s.ToLower());
        
        protected override IRepository<DTO_C_MailCategory> GetRepository() => MailRepository.GetMailCategories();
        
        protected override void FieldsInit(FieldCollection<DTO_C_MailCategory> fields)
        {
            fields.AddCellWithSortAndFilter(o => o.ID, o => o.ID);
            fields.AddCellWithSortAndFilter(o => o.Title, (w, o) => 
                w.ActionLink(al => al.To("mailCategory", "view", AccessControl).WithArg(Constants.Id, o.ID).WithTitle(o.Title)));
            fields.AddCellWithSortAndFilter(o => o.SystemName, o=>o.SystemName);
            fields.AddCellWithSortAndFilter(o => o.MailType, o => o.MailType.Icon());
            fields.AddActionsCell(
                o => al => al.To("mailCategory", "edit", AccessControl).WithArg(Constants.Id, o.ID)
                    .WithImage("edit").WithTitle("Редактировать"),
                o => al => al.To("mailCategory", "delete", AccessControl).WithArg(Constants.Id, o.ID)
                    .WithImage("delete").WithTitle("Удалить")
            );
        }
        
        protected override void ToolbarLeft(MenuBuilder t)
        {
            t.ItemFilter(Filter);
            t.ToCreateNew("mailCategory", "createnew");
            ToDeleteBulk(t);
        }
    }

    [OnAction("mailCategory", "createnew")]
    [OnAction("mailCategory", "edit")]
    public class dto_c_mailCategory_edit : default_edit_rep<DTO_C_MailCategory, int>
    {
        [Inject] protected IMailRepository MailRepository { get; set; }
        private DTO_C_MailCategoryFields.DefaultGroup _group;

        private IEnumerable<SelectListItem> GetSystemNames() => MailRepository.GetSystemNames().OrderBy(x => x.Item1)
            .Select(o => new SelectListItem(o.Item1, o.Item2));
        
        protected override void Form(LayoutWriter w)
        {
            w.FieldsBlockStd(() =>
            {
                w.TextBox(_group.Title);
                w.DropDownList(_group.SystemID, GetSystemNames());
                w.DropDownList(_group.MailType, ViewData.GetMailTypes());
            });
        }
        
        public override void OnInit()
        {
            _group = AddFieldGroup(new DTO_C_MailCategoryFields.DefaultGroup());
        }

        protected override DTO_C_MailCategory GetExistingEntity()
        {
            var id = Context.GetArg<int>(Constants.Id);
            var obj = MailRepository.GetMailCategories().GetById(id);
            return obj;
        }
        
        protected override void Submit(ApiResponse response)
        {
            if (EntityAudit != null && ViewData != null)
            {
                if (!CreateObjectMode)
                {
                    if (EntityAudit != null)
                        EntityAudit.PrimaryObject.PropertyChanges = Tracker?.GetChanges(ViewData);
                }
            }

            if (CreateObjectMode)
                InTransaction(() =>
                {
                    MailRepository.CreateMailCategory(ViewData);
                });
            else
            {
                InTransaction(() =>
                {
                    MailRepository.UpdateMailCategory(ViewData);
                });
            }
        }
    }

    [OnAction("mailCategory", "view")]
    public class dto_c_mailCategory_view : default_view_rep<DTO_C_MailCategory, int>
    {
        [Inject] protected IMailRepository MailRepository { get; set; }
        private DTO_C_MailCategoryFields.DefaultGroup _group;
        protected override void Form(LayoutWriter w)
        {
            w.FieldsBlockStd(() =>
            {
                w.PlainText(_group.Title);
                w.PlainText(_group.SystemName);
                w.PlainText(_group.MailType);
            });
        }
        
        protected override void ToolbarLeft(MenuBuilder t)
        {
            t.ItemBack();
            t.ItemActionImageText(x => x.To("mailCategory", "edit", AccessControl, null, Context.ReturnUrl.Get(1))
                .WithImage("edit")
                .WithArg(Constants.Id, ViewData.ID));
            t.ItemSeparator();
            t.ItemActionImageText(x => x.To("mailCategory", "delete", AccessControl, null, Context.ReturnUrl.Get(1))
                .WithImage("delete")
                .WithArg(Constants.Id, ViewData.ID));
        }

        public override void OnInit()
        {
            _group = AddFieldGroup(new DTO_C_MailCategoryFields.DefaultGroup());
        }

        protected override DTO_C_MailCategory GetExistingEntity()
        {
            var id = Context.GetArg<int>(Constants.Id);
            var obj = MailRepository.GetMailCategories().GetById(id);
            return obj;
        }
    }

    [OnAction("mailCategory", "delete")]
    public class dto_c_mailCategory_delete : default_delete<DTO_C_MailCategory, int>
    {
        [Inject] protected IMailRepository MailRepository { get; set; }
        protected override void Delete(IEnumerable<int> ids)
        {
            MailRepository.DeleteMailCategory(ids);
        }
    }
}
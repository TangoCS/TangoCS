using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Data.Linq;
using System.Collections;
using System.Transactions;
using Nephrite.Web.Layout;
using Nephrite.Web;
using Nephrite.Meta;
using System.Web.UI.WebControls;
using System.Globalization;

namespace Nephrite.Web.Controls
{
	[ParseChildren(true), PersistChildren(false), ControlBuilder(typeof(CartControlBuilder))]
	public class Cart : Control, INamingContainer
	{
		[TemplateInstance(TemplateInstance.Single)]
		public ITemplate CartTemplate { get; set; }
		public DataContext DataContext { get; set; }
		protected Control templateInstance;
		public string Title { get; set; }
		public string Type { get; set; }
		public string Culture { get; set; }
		public int UserID { get; set; }

	}

	[ParseChildren(true), PersistChildren(false)]
	public class Cart<T> : Cart where T : class, ICartItem, new()
	{
		HiddenField hfItemToDelete = new HiddenField { ID = "hfItemToDelete" };
		LinkButton lbDelete = new LinkButton { ID = "lbDelete" };
		Button btnCreateOrder = new Button { ID = "btnCreateOrder", Text = "Оформить заказ", Width = Unit.Percentage(100) };
		int cartItemToDelete = 0;
		List<T> cartItemsAll = new List<T>();
		T obj = new T();

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			Controls.Add(hfItemToDelete);
			Controls.Add(lbDelete);
			Controls.Add(btnCreateOrder);

			btnCreateOrder.Click += new EventHandler(btnCreateOrder_Click);

			if (CartTemplate != null)
			{
				templateInstance = new Control();
				CartTemplate.InstantiateIn(templateInstance);
				Controls.Add(templateInstance);
			}
		}

		void btnCreateOrder_Click(object sender, EventArgs e)
		{
			Guid billGUID = Guid.NewGuid();

			using (TransactionScope transaction = new TransactionScope())
			{
				DataContext.ExecuteCommand("INSERT INTO Bill (BillGUID, UserID, IsPaid, CreateDate, OperationType) VALUES ({0}, {1}, {2}, {3}, 'B')", billGUID, UserID, 0, DateTime.Now);
				foreach (var item in cartItemsAll)
				{
					Guid billItemGUID = Guid.NewGuid();
					DataContext.ExecuteCommand("INSERT INTO BillItem (Price, Number, BillGUID, ObjectKey, ClassName, Title) Values ({0}, 1, {1}, {2}, {3}, {4})", item.Price, billGUID, item.ObjectID, "EfdDescription", item.Title);
				}

				DataContext.ExecuteCommand("EXECUTE('DELETE FROM Cart WHERE UserID = ' + {0} + ' AND ObjectKey in (' + {1} +')')", UserID.ToString(), cartItemsAll.Select(o => o.ObjectID.ToString()).ToArray().Join(","));
				transaction.Complete();
			}

			Page.Response.Redirect(String.Format("/Web.aspx?mode=Bill&action=view&oid={0}", billGUID));
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			cartItemToDelete = hfItemToDelete.Value.ToInt32(0);

			if (cartItemToDelete > 0)
				DataContext.ExecuteCommand("DELETE FROM Cart WHERE ObjectKey = {0} AND UserID = {1}", cartItemToDelete, UserID);

			hfItemToDelete.Value = "";

			var cartItemIds = DataContext.ExecuteQuery<int>("Select c.ObjectKey from Cart as \"c\" where c.UserID = {0}", UserID).Where(o => o != cartItemToDelete).ToList();
			if (cartItemIds.Count() > 0)
				cartItemsAll = DataContext.GetTable<T>().Where<T>(obj.FindByIDs<T>(cartItemIds)).ToList();
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if (templateInstance != null)
				templateInstance.RenderControl(writer);
			else
			{
				var cultureInfo = new CultureInfo(Culture);
				if (Culture.Contains("ru"))
					cultureInfo.NumberFormat.CurrencySymbol = " руб.";

				if (cartItemsAll.Count > 0)
				{
					writer.Write("Для удобства, выбранные вами произведения сведены в таблицу:<br/><br/>");
					writer.Write(AppWeb.Layout.ListTableBegin(new { width = "100%" }));
					writer.Write(AppWeb.Layout.ListHeaderBegin(new { style = "background-color:#D9D9D9" }));
					writer.Write(AppWeb.Layout.TH("№", new { width = "2%" }));
					writer.Write(AppWeb.Layout.TH("", new { width = "2%" }));
					writer.Write(AppWeb.Layout.TH("Наименование", new { width = "84%", style = "text-align:left;" }));
					writer.Write(AppWeb.Layout.TH("Стоимость", new { width = "12%" }));
					writer.Write(AppWeb.Layout.ListHeaderEnd());

					var cartItems = new List<T>();
					cartItems.AddRange(cartItemsAll);

					var parentIds = cartItemsAll.Where(o => o.ParentObjectID.HasValue).Select(o => o.ParentObjectID.Value).ToList();
					var cartItemsParent = new List<T>();
					if (parentIds.Count > 0)
					{
						cartItemsParent = DataContext.GetTable<T>().Where<T>(obj.FindByIDs<T>(parentIds)).ToList();
						cartItems.AddRange(cartItemsParent);
					}
					cartItemToDelete = 0;

					var cartItemsFirstLevel = cartItems.Where(o => !o.ParentObjectID.HasValue).ToList();
					var cartItemsLevel2 = cartItems.Where(o => o.ParentObjectID.HasValue).ToList();

					int i = 0;

					foreach (var cartItem in cartItemsFirstLevel)
					{
						i++;
						var childItems = cartItemsLevel2.Where(o => o.ParentObjectID == cartItem.ObjectID).ToList();
						writer.Write(AppWeb.Layout.ListRowBegin(""));
						writer.Write(AppWeb.Layout.TD(i.ToString() + ".", new { style = "text-align:center;" }));
						writer.Write(AppWeb.Layout.TDBegin());

						if (childItems.Count == 0)
							writer.Write("<a href=\"javascript:void(0)\" onclick=\"if (confirm('Удалить ЭПД из корзины?')) {document.getElementById('" + hfItemToDelete.ClientID + "').value = '" + cartItem.ObjectID.ToString() + "'; " + Page.ClientScript.GetPostBackEventReference(lbDelete, "") + ";}\"><img src=\"/i/n/delete.gif\" alt=\"\" /></a>");

						writer.Write(AppWeb.Layout.TDEnd());
						writer.Write(AppWeb.Layout.TD(cartItem.Title));
						writer.Write(AppWeb.Layout.TDBegin(new { style = "text-align:right; font-weight:bold" }));

						if (childItems.Count == 0)
							writer.Write(cartItem.Price.HasValue ? cartItem.Price.Value.ToString("C", cultureInfo.NumberFormat) : "");
						else
							writer.Write(childItems.Where(o => o.Price.HasValue).Select(o => o.Price.Value).Sum().ToString("C", cultureInfo.NumberFormat));

						writer.Write(AppWeb.Layout.TDEnd());
						writer.Write(AppWeb.Layout.ListRowEnd());

						if (childItems.Count > 0)
						{
							int i2 = 0;
							foreach (var efdL2Item in childItems)
							{
								i2++;
								writer.Write(AppWeb.Layout.ListRowBegin(""));
								writer.Write(AppWeb.Layout.TD(""));
								writer.Write(AppWeb.Layout.TD(i.ToString() + "." + i2.ToString()));
								writer.Write(AppWeb.Layout.TDBegin());
								writer.Write("<a href=\"javascript:void(0)\" onclick=\"if (confirm('Удалить ЭПД из корзины?')) {document.getElementById('" + hfItemToDelete.ClientID + "').value = '" + efdL2Item.ObjectID.ToString() + "'; " + Page.ClientScript.GetPostBackEventReference(lbDelete, "") + ";}\"><img src=\"/i/n/delete.gif\" alt=\"\" /></a>");
								writer.Write("&nbsp;&nbsp;&nbsp;" + efdL2Item.Title);
								writer.Write(AppWeb.Layout.TDEnd());
								writer.Write(AppWeb.Layout.TD(efdL2Item.Price.HasValue ? efdL2Item.Price.Value.ToString("C", cultureInfo.NumberFormat) : "", new { style = "text-align:left;" }));
								writer.Write(AppWeb.Layout.ListRowEnd());
							}
						}
					}

					writer.Write(AppWeb.Layout.ListTableEnd());
					writer.Write("<div style=\"background-color:#C6D8F0; padding:6px; padding-bottom:6px;\">");
					writer.Write("<span style=\"text-align:right; display:block;font-weight:bold; font-size:15pt; padding-top:10px; padding-bottom:20px;\">Итого с вас: ");
					writer.Write("<span style=\"padding-left:50px\">");
					writer.Write(cartItems.Where(o => (cartItemsLevel2.Select(o1 => o1.ObjectID).Contains(o.ObjectID) || cartItemsFirstLevel.Select(o1 => o1.ObjectID).Contains(o.ObjectID) && cartItemsLevel2.Where(o1 => o1.ParentObjectID == o.ObjectID).Count() == 0) && o.Price.HasValue).Select(o => o.Price.Value).Sum().ToString("C", cultureInfo.NumberFormat) + "&nbsp;");
					writer.Write("</span></span>");
					btnCreateOrder.RenderControl(writer);
					writer.Write("</div><br/>");
					hfItemToDelete.RenderControl(writer);
					lbDelete.RenderControl(writer);
				}
				else
					writer.Write("На данный момент ваша корзина пуста.<br/>");
			}
		}
	}

	public class CartControlBuilder : ControlBuilder
	{
		public override void Init(TemplateParser parser, ControlBuilder parentBuilder, Type type, string tagName, string id, IDictionary attribs)
		{
			string typeName = (string)attribs["Type"];
			Type t = Type.GetType(typeName);
			Type genericType = typeof(Cart<>);
			base.Init(parser, parentBuilder, genericType.MakeGenericType(new Type[] { t }), tagName, id, attribs);
		}


	}


}

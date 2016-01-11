using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Nephrite.Data;

namespace Nephrite.Controls
{
	public class PersistentFilter
	{
		IDC_ListFilter _dc;
		//AbstractQueryString _query;
		IN_Filter _filter;
		List<FilterItem> _items;
		public bool editMode = false;


		//public PersistentFilter(IDC_ListFilter dc, AbstractQueryString query, string action = null)
		//{
		//	_dc = dc;
		//	//_query = query;
		//	_filter = _dc.IN_Filter.SingleOrDefault(o => o.FilterID == _query.GetInt("filterid", 0));
		//	_items = _filter != null && _filter.FilterValue != null ? XmlHelper.Deserialize<List<FilterItem>>(_filter.FilterValue.Root) : new List<FilterItem>();
		//}

		public PersistentFilter(IDC_ListFilter dc, int? filterID)
		{
			_dc = dc;
			//_query = query;
			if (filterID.HasValue) _filter = _dc.IN_Filter.SingleOrDefault(o => o.FilterID == filterID);
			_items = _filter != null && _filter.FilterValue != null ? XmlHelper.Deserialize<List<FilterItem>>(_filter.FilterValue.Root) : new List<FilterItem>();
		}

		public List<FilterItem> Criteria
		{
			get { return _items; }
		}

		public void NewFilter()
		{
			_filter = _dc.NewIN_Filter();
			_filter.FilterValue = _filter != null ? _filter.FilterValue :
					new XDocument(XmlHelper.Serialize<List<FilterItem>>(new List<FilterItem>()));
			_dc.IN_Filter.InsertOnSubmit(_filter);
		}

		IN_Filter Filter
		{
			get
			{
				if (_filter == null)
				{
					_filter = _dc.NewIN_Filter();
				}
				return _filter;
			}
		}

		public string Name
		{
			get { return Filter.FilterName; }
			set { Filter.FilterName = value; }
		}

		public string ListParms
		{
			get { return Filter.ListParms; }
			set { Filter.ListParms = value; }
		}

		public string Columns
		{
			get { return Filter.Columns; }
			set { Filter.Columns = value; }
		}
		public string Sort
		{
			get { return Filter.Sort; }
			set { Filter.Sort = value; }
		}
		public int ItemsOnPage
		{
			get { return Filter.ItemsOnPage < 1 ? 50 : Filter.ItemsOnPage; }
			set { Filter.ItemsOnPage = value; }
		}


		public string ListName
		{
			get { return Filter.ListName; }
			set { Filter.ListName = value; }
		}

		public bool IsDefault
		{
			get { return Filter.IsDefault; }
			set { Filter.IsDefault = value; }
		}

		public int FilterID
		{
			get { return Filter.FilterID; }
		}

		public int? SubjectID
		{
			get { return Filter.SubjectID; }
			set { Filter.SubjectID = value; }
		}

		#region Группировки
		public string Group1Sort
		{
			get { return Filter.Group1Sort; }
			set { Filter.Group1Sort = value; }
		}

		public string Group2Sort
		{
			get { return Filter.Group2Sort; }
			set { Filter.Group2Sort = value; }
		}

		public int? Group1Column
		{
			get { return Filter.Group1Column; }
			set { Filter.Group1Column = value; }
		}

		public int? Group2Column
		{
			get { return Filter.Group2Column; }
			set { Filter.Group2Column = value; }
		}
		#endregion

		public void Save(List<FilterItem> items)
		{
			_items = items;
			Filter.FilterValue = new XDocument(XmlHelper.Serialize<List<FilterItem>>(_items));
			if (Filter.FilterID == 0) _dc.IN_Filter.InsertOnSubmit(Filter);
			_dc.SubmitChanges();
		}
	}

	public interface IN_Filter : IEntity
	{
		int FilterID { get; set; }
		int? SubjectID { get; set; }
		string ListName { get; set; }
		XDocument FilterValue { get; set; }
		string FilterName { get; set; }
		bool IsDefault { get; set; }
		System.Nullable<int> Group1Column { get; set; }
		string Group1Sort { get; set; }
		System.Nullable<int> Group2Column { get; set; }
		string Group2Sort { get; set; }
		string ListParms { get; set; }
		string Columns { get; set; }
		string Sort { get; set; }
		int ItemsOnPage { get; set; }
	}

	public interface IDC_ListFilter : IDataContext
	{
		ITable<IN_Filter> IN_Filter { get; }
		IN_Filter NewIN_Filter();
	}
}

using System;
using System.Collections.Generic;
using System.Text;

namespace RetailERP.Core.Common.KendoGridManager
{
    public class GridPopUp
    {
        public string searchKey { get; set; }
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
    }
    public class GridOptions
    {
        public int? skip { get; set; } = 0;
        public int? take { get; set; } = 0;
        public int? page { get; set; } = 0;
        public int pageSize { get; set; }
        public List<AzFilter.GridSort> sort { get; set; }
        public AzFilter.GridFilters filter { get; set; }
    }

    public class GridColumns
    {
        public string field { get; set; }
        public string title { get; set; }
        public string width { get; set; }
        //public string footerTemplate { get; set; }
        //public string template { get; set; }
        public bool filterable { get; set; }
        public bool sortable { get; set; }
        public bool hidden { get; set; }
    }
    public class GridResult<T>
    {

        public GridEntity<T> Data(List<T> list, int totalCount)
        {
            var dEntity = new GridEntity<T>();
            dEntity.Items = list ?? new List<T>();
            dEntity.TotalCount = totalCount;
            dEntity.Columnses = new List<GridColumns>();
            return dEntity;
        }
        public GridEntity<T> Data1(List<T> list, int totalCount)
        {
            var dEntity = new GridEntity<T>();
            dEntity.Items = list ?? new List<T>();
            dEntity.TotalCount = totalCount;
            return dEntity;
        }

    }
    public class GridEntity<T>
    {
        public IList<T> Items { get; set; }
        public int TotalCount { get; set; }
        public IList<GridColumns> Columnses { get; set; }

    }
    public class ComboEntity<T>
    {
        public IList<T> data { get; set; }
        public int total { get; set; }

    }
    public class ComboResult<T>
    {
        public ComboEntity<T> Data(List<T> list, int totalCount)
        {
            var dEntity = new ComboEntity<T>();
            dEntity.data = list ?? new List<T>();
            dEntity.total = totalCount;
            return dEntity;
        }

    }
}

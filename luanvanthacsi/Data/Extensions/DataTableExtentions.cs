using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace luanvanthacsi.Data.Extentions
{
    public static class DataTableExtentions
    {
        public static List<DataColumn> GetDataColumns(this DataTable dataTable)
        {
            DataColumn[] columns = new DataColumn[dataTable.Columns.Count];
            dataTable.Columns.CopyTo(columns, 0);
            return columns.ToList();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace luanvanthacsi.Excel
{
    public class ExcelSheetObject
    {
        public string Name { get; set; }
        public List<KeyValuePair<int, int>> ErrorCells { get; set; }
        public List<KeyValuePair<int, int>> ErrorCellsTV { get; set; }
        public string TemplateKey { get; set; }
        public int StartRowIndex { get; set; }
        public int TemplateRowIndex { get; set; }
        public List<DataColumn> InputColumns { get; set; }
        public List<string> SkipColumns { get; set; }
        public List<string> ReadColumns
        {
            get
            {
                return InputColumns.Where(c => !SkipColumns.Contains(c.ColumnName)).Select(c => c.ColumnName).ToList();
            }
            private set { }
        }

        public ExcelSheetObject()
        {
            Name = "default";
            ErrorCells = new List<KeyValuePair<int, int>>();
            TemplateKey = string.Empty;
            StartRowIndex = 1;
            TemplateRowIndex = 1;
            SkipColumns = new List<string>();
            InputColumns = new List<DataColumn>();
            ErrorCellsTV = new List<KeyValuePair<int, int>>();
        }

        public ExcelSheetObject(string name,
            string templateKey, int startRowIndex, List<string> skipColumns, List<DataColumn> inputColumns, int templateRowIndex)
        {
            this.Name = name;
            this.ErrorCells = new List<KeyValuePair<int, int>>();
            this.ErrorCellsTV = new List<KeyValuePair<int, int>>();
            this.TemplateKey = templateKey;
            this.StartRowIndex = startRowIndex;
            this.TemplateRowIndex = templateRowIndex;
            if (skipColumns != null)
            {
                this.SkipColumns = skipColumns;
            }
            else
            {
                this.SkipColumns = new List<string>();
            }
            if (inputColumns != null)
            {
                this.InputColumns = inputColumns;
            }
            else
            {
                this.InputColumns = new List<DataColumn>();
            }

            this.ReadColumns = new List<string>();
        }
    }
}

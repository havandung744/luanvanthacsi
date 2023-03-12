using AutoMapper;
using luanvanthacsi.Data.Extentions;
using luanvanthacsi.Excel;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace luanvanthacsi.Excel
{
    public class ExcelExporter
    {
        public readonly IMapper _mapper;

        public ExcelExporter(IMapper mapper)
        {
            _mapper = mapper;
        }

        List<DictonaryWriteExcel> GetTemplateExportExcel(ExcelWorksheet wSheet, int headerRow)
        {
            List<DictonaryWriteExcel> templates = new List<DictonaryWriteExcel>();
            var startCol = wSheet.Dimension.Start.Column;
            var endCol = wSheet.Dimension.End.Column;
            for (int column = startCol; column <= endCol; column++)
            {
                templates.Add(new DictonaryWriteExcel(wSheet.Cells[headerRow, column].GetValue<string>(), (CellExcelEpplus)column));
            }
            templates.RemoveAll(c => c.NameProperty.IsNullOrEmpty());
            return templates;
        }
        public DataTable GetTable(Type type)
        {
            var table = new DataTable();
            var properties = type.GetProperties();
            table.Columns.AddRange(properties.Select(p => new DataColumn(p.Name, Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType)).ToArray());
            return table;
        }
        public void WriteToSheet<T>(List<T> data, ExcelWorksheet wSheet, ExcelSheetObject config)
        {
            WriteToSheet(data, wSheet, config.StartRowIndex, config.TemplateRowIndex);
        }

        public void WriteToSheet<T>(List<T> data, ExcelWorksheet wSheet, int startDataRow, int headerRow)
        {
            try
            {
                int rowIndex = startDataRow;
                int templateRow = rowIndex;
                wSheet.InsertRow(templateRow + 1, data.Count, templateRow);
                rowIndex += 1;
                var template = GetTemplateExportExcel(wSheet, headerRow);
                foreach (var item in data)
                {
                    foreach (var itemPosition in template)
                    {
                        int column = (int)itemPosition.CellEpplus;
                        string nameProperty = itemPosition.NameProperty;
                        var cellValue = item.GetValue(nameProperty);
                        if (cellValue.IsNotNullOrEmpty())
                        {
                            wSheet.SetValue(rowIndex, column, cellValue.ToString());
                        }
                    }
                    rowIndex++;
                }
                int startFormulaRow = templateRow + 1;
                int endFormulaRow = templateRow + data.Count + 1;
                foreach (var itemPosition in template)
                {
                    int column = (int)itemPosition.CellEpplus;
                    if (wSheet.Cells[templateRow, column].Formula.IsNotNullOrEmpty())
                    {
                        wSheet.Cells[startFormulaRow, column, endFormulaRow, column].FormulaR1C1
                            = wSheet.Cells[templateRow, column].FormulaR1C1;
                    }
                }
                wSheet.DeleteRow(templateRow);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}

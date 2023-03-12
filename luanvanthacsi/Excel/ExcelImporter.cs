using luanvanthacsi.Data.Extentions;
using luanvanthacsi.Excel;
using luanvanthacsi.Ultils;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace luanvanthacsi.Data
{
    public class ExcelImporter
    {
        public string FileName { get; set; }
        public bool IsTCVN3 = false;
        public string TemplateKey;

        Regex regexHoTen = new Regex(@"[~`#|^]+");
        Regex regexMaSoBHXH = new Regex(@"^[\d]{10}$");
        Regex regexDateTime = new Regex(@"^[\d]?[\d]?/[\d]?[\d]?/[12][0-9][0-9][0-9]$");
        CultureInfo culture = CultureInfo.CreateSpecificCulture("vi-VN");

        public ExcelImporter(string fileName, string TemplateKey)
        {
            FileName = fileName;
            this.TemplateKey = TemplateKey;
        }
        public class ReadExcelEventArgs : EventArgs
        {
            public double Percent { get; set; }
        }
        public event Action<object, EventArgs> Complete = null;
        public event Action<object, ReadExcelEventArgs> Reading = null;

        public DataSet Read_Excel_HSNS_xls(IList<ExcelSheetObject> ListSheet, int posHeader, List<KeyValuePair<int, int>> errorCells, List<KeyValuePair<int, int>> errorCells_tv, MemoryStream memoryStream, params string[] skipColumn)
        {
            var listColumn = ListSheet[0].InputColumns;
            var listColumNV = ListSheet[1].InputColumns;
            DataColumn[] columns = new DataColumn[listColumn.Count];
            listColumn.CopyTo(columns, 0);
            DataColumn[] columnsNV = new DataColumn[listColumNV.Count];
            listColumNV.CopyTo(columnsNV, 0);
            DataSet ds = Read_Excel_HSNS_2003(new List<string> { "NhanSu", "ThanNhan" }, new List<int> { posHeader, posHeader },
                new List<DataColumn[]> { columns, columnsNV }, new List<string[]> { skipColumn, skipColumn }, errorCells, errorCells_tv, memoryStream, TemplateKey);
            return ds;
        }

        private DataSet Read_Excel_HSNS_2003(IList<string> listSheet, IList<int> listPositionHeader,
        IList<DataColumn[]> listInputColumn, IList<string[]> skipColumn, List<KeyValuePair<int, int>> errorCells, List<KeyValuePair<int, int>> errorCells_tv, MemoryStream memoryStream, string TemplateKey)
        {
            DataRow destRow;
            IRow row;
            int j;
            ICell cell;
            string cellValue;
            string stringCellValue;
            CultureInfo culture = CultureInfo.CreateSpecificCulture("vi-VN");
            #region Khởi tạo danh sách check dữ liệu

            bool trangthai = true;
            #endregion
            Regex regexHoTen = new Regex(@"[~`#|^]+");
            Regex regexMaSoBHXH = new Regex(@"^[\d]{10}$");
            Regex regexDateTime = new Regex(@"^[\d]?[\d]?/[\d]?[\d]?/[12][0-9][0-9][0-9]$");
            double maximum = listSheet.Count > 10000 ? listSheet.Count : 10000;
            if (Reading != null)
                Reading(this, new ReadExcelEventArgs { Percent = 0 });
            double unitSheet = maximum / listSheet.Count;
            var dataSet = new DataSet();
            var dataSetNV = new DataSet();
            var excel = new HSSFWorkbook(memoryStream);
            for (int isheet = 0; isheet < listSheet.Count; isheet++)
            {
                string nameSheet = listSheet[isheet];
                ISheet sheet;
                //sheet = nameSheet != "default" ? ExcelImpSupport.GetSheet_2003(nameSheet, FileName) : ExcelImpSupport.GetSheet_2003(1, memoryStream);
                sheet = excel.GetSheet(nameSheet);
                if (sheet == null)
                    return null;
                var startRows = listPositionHeader[isheet];
                var endRows = sheet.LastRowNum;
                int startColumn = 0, endColumn = 0;

                #region Khởi tạo datatable
                var result = new DataTable(listSheet[isheet]);
                foreach (var item in listInputColumn[isheet])
                {
                    //result.Columns.Add(item.CopyTo());
                    result.Columns.Add(item.ColumnName, item.DataType);
                }
                #endregion

                var listHeader = new List<string>();
                double unitRow = (endRows - startRows + 1) == 0 ? unitSheet : unitSheet / (endRows - startRows + 1);
                var headRow = sheet.GetRow(startRows - 1);
                startColumn = sheet.GetRow(startRows - 1).FirstCellNum;
                endColumn = sheet.GetRow(startRows - 1).LastCellNum;
                // lấy danh sách tên cột dòng dòng tiêu đề trong file mẫu
                for (int i = startColumn; i < endColumn; i++)
                {
                    var headCell = headRow.GetCell(i);
                    if (headCell != null)
                    {
                        listHeader.Add(headCell.ToString());
                    }
                }
                if (listHeader == null || !listHeader.Any(c => c.ToLower().Equals(TemplateKey.ToLower())))
                {
                    throw new InvalidDataException("FileMauKhongHopLe");
                }
                Dictionary<int, string> mapColumnCell = new Dictionary<int, string>();
                for (int i = startColumn; i < endColumn; i++)
                {
                    var column = listInputColumn[isheet]
                                 .FirstOrDefault(col => col.ColumnName.Equals(listHeader[i]));
                    if (column != null && !skipColumn[isheet].Contains(column.ColumnName))
                    {
                        mapColumnCell.Add(i, column.ColumnName);
                    }
                }
                bool hasData;
                HSSFFormulaEvaluator formula = new HSSFFormulaEvaluator(excel);
                for (int i = startRows; i <= endRows; i++)
                {
                    hasData = false;
                    row = sheet.GetRow(i);
                    if (row == null)
                    {
                        continue;
                    }
                    destRow = result.NewRow();
                    foreach (var map in mapColumnCell)
                    {
                        j = map.Key;
                        cell = row.GetCell(j);
                        if (cell == null || cell.ToString().Trim().Length == 0)
                        {
                            continue;
                        }
                        formula.EvaluateInCell(cell);
                        cellValue = cell.ToString().Trim();
                        if (cell.CellType == CellType.String && cell.StringCellValue.IsNotNullOrEmpty())
                        {
                            stringCellValue = cell.StringCellValue.Trim();
                        }
                        else
                        {
                            stringCellValue = string.Empty;
                        }
                        hasData = true;
                        try
                        {
                            if (result.Columns[map.Value].DataType == typeof(DateTime))
                            {
                                if (cell.CellType == CellType.Numeric)
                                    destRow[map.Value] = cell.DateCellValue;
                                else
                                {
                                    destRow[map.Value] = stringCellValue.ConvertCultureVN();
                                    Convert.ToDateTime(stringCellValue, culture).ToString("dd/MM/yyyy");
                                }
                            }
                            else if (result.Columns[map.Value].DataType == typeof(bool))
                            {
                                if (cell.CellType == CellType.Boolean)
                                {
                                    destRow[map.Value] = cell.BooleanCellValue;
                                }
                                else
                                {
                                    destRow[map.Value] = Convert.ToBoolean(cell.StringCellValue.Trim().ToLower());
                                }
                            }
                            else if (result.Columns[map.Value].DataType.IsEnum)
                            {
                                var nameEnum = result.Columns[map.Value].DataType;
                                destRow[map.Value] = Enum.Parse(nameEnum, cellValue);
                            }
                            else
                            {
                                string value;
                                switch (map.Value.ToLower())
                                {
                                    case "hoten":
                                    case "hovaten":
                                    case "ct02":
                                        if (cellValue.IsNullOrEmpty())
                                        {
                                            hasData = false;
                                            break;
                                        }
                                        if (regexHoTen.IsMatch(cellValue))
                                        {
                                            errorCells.Add(i, j);
                                        }
                                        else
                                        {
                                            destRow[map.Value] = cellValue;
                                        }
                                        break;
                                    case "dateofbirth":
                                    case "dayforidentitycard":
                                    case "validdateofpassport":
                                    case "expiredateofpassport":
                                    case "validdateofvisa":
                                    case "expiredateofvisa":
                                    case "validdateofworkpermit":
                                    case "expiredateofworkpermit":
                                    case "startworkdate":
                                    case "oficialstartdate":
                                    case "dateofbirth_tv":
                                    case "dayforidentitycard_tv":
                                    case "applicationstartdate_tv":
                                    case "applicationenddate_tv":
                                        if (cellValue.IsNullOrEmpty())
                                        {
                                            break;
                                        }
                                        if (regexDateTime.IsMatch(stringCellValue))
                                        {
                                            destRow[map.Value] = stringCellValue.ConvertCultureVN();
                                            Convert.ToDateTime(stringCellValue, culture).ToString("dd/MM/yyyy");
                                        }
                                        else
                                        {
                                            errorCells.Add(i, j);
                                        }
                                        break;
                                    case "cmnd_tv":
                                        if (cellValue.IsNullOrEmpty())
                                        {
                                            break;
                                        }
                                        if (cellValue.Length >= 20)
                                        {
                                            errorCells_tv.Add(new KeyValuePair<int, int>(i, j));
                                        }
                                        else
                                        {
                                            destRow[map.Value] = cellValue;
                                        }
                                        break;


                                    case "masobhxh_tv":
                                        if (cellValue.IsNullOrEmpty())
                                        {
                                            break;
                                        }
                                        if (cellValue.Length != 10)
                                        {
                                            errorCells_tv.Add(new KeyValuePair<int, int>(i, j));
                                        }
                                        else if (regexMaSoBHXH.IsMatch(cellValue))
                                        {
                                            destRow[map.Value] = cellValue;
                                        }
                                        else
                                        {
                                            errorCells_tv.Add(new KeyValuePair<int, int>(i, j));
                                        }
                                        break;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            if (!errorCells.Contains(new KeyValuePair<int, int>(i, j)))
                            {
                                errorCells.Add(i, j);
                            }
                        }

                    }

                    if ((errorCells.Count == 0 || errorCells.Last().Key != i) && hasData)
                    {
                        result.Rows.Add(destRow);
                    }

                    if (Reading != null)
                        Reading(this, new ReadExcelEventArgs { Percent = (i - startRows + 1) * unitRow * (isheet + 1) / maximum });
                }
                if (trangthai)
                {
                    dataSet.Tables.Add(result);
                    trangthai = false;
                }
                else
                {
                    dataSetNV.Tables.Add(result);
                    trangthai = true;
                }
            }
            dataSet.Merge(dataSetNV);
            if (Reading != null)
                Reading(this, new ReadExcelEventArgs { Percent = 100 });
            if (Complete != null)
                Complete(this, new EventArgs());
            return dataSet;
        }

        public DataSet ReadHsnsXlsx(List<ExcelSheetObject> sheets, MemoryStream memoryStream, Dictionary<string, List<KeyValuePair<int, int>>> errors)
        {
            List<string> sheetNames = sheets.Select(c => c.Name).ToList();
            List<int> positionHeaders = sheets.Select(c => c.TemplateRowIndex).ToList();
            List<int> positionDatas = sheets.Select(c => c.StartRowIndex).ToList();
            List<List<DataColumn>> inputColumns = sheets.Select(c => c.InputColumns ?? new List<DataColumn>()).ToList();
            List<List<string>> skipColumns = sheets.Select(c => c.SkipColumns ?? new List<string>()).ToList();
            double maximum = 10000;
            if (Reading != null)
                Reading(this, new ReadExcelEventArgs { Percent = 0 });
            double unitSheet = maximum / sheetNames.Count;
            var dataSet = new DataSet();
            ExcelPackage excel = new ExcelPackage(memoryStream);
            List<KeyValuePair<int, int>> errorCells = null;

            for (int isheet = 0; isheet < sheetNames.Count; isheet++)
            {
                string templateKey = sheets[isheet].TemplateKey;
                errorCells = new List<KeyValuePair<int, int>>();
                string nameSheet = sheetNames[isheet];
                ExcelWorksheet oSheet;
                oSheet = excel.Workbook.Worksheets[nameSheet];
                oSheet.Calculate();
                if (oSheet == null || oSheet.Dimension == null)
                    return null;
                var startRow = positionDatas[isheet];
                var startCol = oSheet.Dimension.Start.Column;
                var endRow = oSheet.Dimension.End.Row;
                var endCol = oSheet.Dimension.End.Column;

                #region Khởi tạo datatable
                var result = new DataTable(sheetNames[isheet]);
                foreach (DataColumn item in inputColumns[isheet])
                {
                    result.Columns.Add(item.CopyTo());
                }
                #endregion

                double unitRow = (endRow - startRow + 1) == 0 ? unitSheet : unitSheet / (endRow - startRow + 1);
                var listHeader = new List<string>();

                for (int column = startCol; column <= endCol; column++)
                {
                    listHeader.Add(oSheet.Cells[positionHeaders[isheet], column].GetValue<string>());
                }
                listHeader.RemoveAll(c => c.IsNullOrEmpty());
                if (templateKey.IsNotNullOrEmpty() && !listHeader.Any(c => string.Equals(c, templateKey, StringComparison.InvariantCultureIgnoreCase)))
                {
                    throw new InvalidDataException("FileMauKhongHopLe");
                }
                Dictionary<int, string> mapColumnCell = new Dictionary<int, string>();
                for (int i = 0; i < listHeader.Count; i++)
                {
                    var column = inputColumns[isheet]
                                 .FirstOrDefault(col => col.ColumnName.Equals(listHeader[i]));
                    if (column != null && !skipColumns[isheet].Contains(column.ColumnName))
                    {
                        mapColumnCell.Add(i + 1, column.ColumnName);
                    }
                }

                for (var i = startRow; i <= endRow; i++)
                {
                    DataRow destRow = result.NewRow();
                    bool hasData = true;
                    foreach (var map in mapColumnCell)
                    {
                        int j = map.Key;
                        try
                        {
                            var cellValue = oSheet.Cells[i, j].Value?.ToString();
                            if (cellValue == null || cellValue.Trim().Length == 0)
                            {
                                continue;
                            }
                            hasData = true;
                            cellValue = cellValue.Trim();
                            if (result.Columns[map.Value].DataType == typeof(DateTime))
                            {
                                CultureInfo culture1 = CultureInfo.CreateSpecificCulture("vi-VN");
                                DateTime dateTime;
                                DateTime.TryParseExact(cellValue, "dd/MM/yyyy", culture1, DateTimeStyles.None, out dateTime);
                                destRow[map.Value] = dateTime;
                            }
                            else if (result.Columns[map.Value].DataType == typeof(bool))
                            {
                                if (cellValue.Equals("TRUE") || cellValue.Equals("True"))
                                    destRow[map.Value] = true;
                                else
                                    destRow[map.Value] = false;
                            }
                            else if (result.Columns[map.Value].DataType.IsEnum)
                            {
                                var nameEnum = result.Columns[map.Value].DataType;
                                destRow[map.Value] = Enum.Parse(nameEnum, cellValue);
                            }
                            else
                            {
                                switch (map.Value.ToLower())
                                {
                                    case "hoten":
                                    case "hovaten":
                                    case "name":
                                        if (cellValue.IsNullOrEmpty())
                                        {
                                            hasData = false;
                                            break;
                                        }
                                        else
                                        {
                                            destRow[map.Value] = cellValue;
                                        }
                                        break;
                                    case "dateofbirth":
                                    case "dayforidentitycard":
                                    case "validdateofpassport":
                                    case "expiredateofpassport":
                                    case "validdateofvisa":
                                    case "expiredateofvisa":
                                    case "validdateofworkpermit":
                                    case "expiredateofworkpermit":
                                    case "startworkdate":
                                    case "oficialstartdate":
                                    case "dateofbirth_tv":
                                    case "dayforidentitycard_tv":
                                    case "applicationstartdate_tv":
                                    case "applicationenddate_tv":
                                        if (cellValue.IsNullOrEmpty())
                                        {
                                            break;
                                        }
                                        if (regexDateTime.IsMatch(cellValue))
                                        {
                                            destRow[map.Value] = cellValue.ConvertCultureVN();
                                            Convert.ToDateTime(cellValue, culture).ToString("dd/MM/yyyy");
                                        }
                                        else
                                        {
                                            errorCells.Add(i, j);
                                        }
                                        break;
                                    case "nationalityid":
                                    case "masobhxh":
                                        if (cellValue.IsNullOrEmpty())
                                        {
                                            break;
                                        }
                                        if (cellValue.Length != 10)
                                        {
                                            errorCells.Add(i, j);
                                        }
                                        else if (regexMaSoBHXH.IsMatch(cellValue))
                                        {
                                            destRow[map.Value] = cellValue;
                                        }
                                        else
                                        {
                                            errorCells.Add(i, j);
                                        }
                                        break;
                                    case "provinceofbirthid":
                                    case "provinceofpernamentaddressid":
                                    case "provinceofcurrentaddressid":
                                    case "cmnd_tv":
                                        if (cellValue.IsNullOrEmpty())
                                        {
                                            break;
                                        }
                                        if (cellValue.Length >= 20)
                                        {
                                            errorCells.Add(new KeyValuePair<int, int>(i, j));
                                        }
                                        else
                                        {
                                            destRow[map.Value] = cellValue;
                                        }
                                        break;


                                    case "masobhxh_tv":
                                    case "SocialInsuranceNumber_TV":
                                        if (cellValue.IsNullOrEmpty())
                                        {
                                            break;
                                        }
                                        if (cellValue.Length != 10)
                                        {
                                            errorCells.Add(new KeyValuePair<int, int>(i, j));
                                        }
                                        else if (regexMaSoBHXH.IsMatch(cellValue))
                                        {
                                            destRow[map.Value] = cellValue;
                                        }
                                        else
                                        {
                                            errorCells.Add(new KeyValuePair<int, int>(i, j));
                                        }
                                        break;
                                    case "stt_hsns":
                                        if (StringExtentions.IsNumberFormatVN(cellValue))
                                        {
                                            destRow[map.Value] = cellValue;
                                        }
                                        else
                                        {
                                            errorCells.Add(new KeyValuePair<int, int>(i, j));
                                        }
                                        break;

                                    default:
                                        if (this.IsTCVN3)
                                            destRow[map.Value] =
                                                UnicodeConverter.TCVN3ToUnicode(
                                                    UnicodeConverter.MultiSpaceToOneSpace(
                                                        cellValue));
                                        else
                                            destRow[map.Value] = cellValue;
                                        break;

                                }
                            }
                        }
                        catch (Exception)
                        {
                            errorCells.Add(i, j);
                        }


                    }

                    if ((errorCells.Count == 0 || errorCells.Last().Key != i) && hasData)
                    {
                        result.Rows.Add(destRow);
                    }

                    if (Reading != null)
                        Reading(this, new ReadExcelEventArgs { Percent = (i - startRow + 1) * unitRow + isheet * unitSheet / maximum });
                }
                dataSet.Tables.Add(result);
                if (errorCells?.Any() == true)
                {
                    errors.TryAdd(nameSheet, errorCells);
                }
            }
            if (Reading != null)
                Reading(this, new ReadExcelEventArgs { Percent = 100 });
            if (Complete != null)
                Complete(this, new EventArgs());
            return dataSet;
        }



    }
}

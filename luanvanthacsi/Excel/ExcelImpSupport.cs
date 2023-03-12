using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace luanvanthacsi.Excel
{
    public class ExcelImpSupport
    {
        public static ISheet GetSheet_2003(string nameSheet, String FileName)
        {
            try
            {
                if (File.Exists(FileName))
                {
                    using (FileStream stream = new FileStream(FileName, FileMode.Open))
                    {
                        var excel = new HSSFWorkbook(stream);
                        stream.Close();
                        return excel.GetSheet(nameSheet);
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static ISheet GetSheet_2003(string nameSheet, MemoryStream stream)
        {
            try
            {
                var excel = new HSSFWorkbook(stream);
                return excel.GetSheet(nameSheet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static ISheet GetSheet_2003(int indexSheet, MemoryStream stream)
        {
            try
            {
                var excel = new HSSFWorkbook(stream);
                return excel.GetSheetAt(indexSheet);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static object ReflectPropertyValue(object source, string property)
        {
            try
            {
                string[] stack = property.Split('.');
                if (stack.Length == 1)
                {
                    return source.GetType().GetProperty(property).GetValue(source, null);
                }
                else
                {
                    object subSource = source.GetType().GetProperty(stack[0]).GetValue(source, null);
                    if (subSource != null)
                    {
                        string subProperty = "";
                        for (int i = 1; i < stack.Length; i++)
                        {
                            if (i != stack.Length - 1)
                            {
                                subProperty += stack[i] + ".";
                            }
                            else
                            {
                                subProperty += stack[i];
                            }
                        }
                        return ReflectPropertyValue(subSource, subProperty);
                    }
                    return String.Empty;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static ExcelWorksheet GetSheet_2007or2010(int indexSheet, String FileName)
        {
            try
            {
                if (File.Exists(FileName))
                {
                    using (var stream = new FileStream(FileName, FileMode.Open))
                    {
                        var excelPkg = new ExcelPackage();
                        excelPkg.Load(stream);
                        stream.Close();
                        return excelPkg.Workbook.Worksheets[indexSheet];
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static ExcelWorksheet GetSheet_2007or2010(string nameSheet, String FileName)
        {
            try
            {
                if (File.Exists(FileName))
                {
                    using (var stream = new FileStream(FileName, FileMode.Open))
                    {
                        var excelPkg = new ExcelPackage();
                        excelPkg.Load(stream);
                        stream.Close();
                        return excelPkg.Workbook.Worksheets[nameSheet];
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw;
                //throw new Exception("Lỗi đọc file excel", ex);
            }
        }
    }

    public class DictonaryWriteExcel
    {
        public string NameProperty { get; set; }
        public string SourceProperty { get; set; }
        public CellExcel Cell { get; set; }
        public CellExcelEpplus CellEpplus { get; set; }
        public int Row { get; set; }
        public string DisplayValue { get; set; }

        public DictonaryWriteExcel() { }
        public DictonaryWriteExcel(string namProperty, CellExcel cell)
        {
            NameProperty = namProperty;
            Cell = cell;
        }

        public DictonaryWriteExcel(string namProperty, CellExcel cell, string sourceProperty)
        {
            NameProperty = namProperty;
            Cell = cell;
            SourceProperty = sourceProperty;
        }

        public DictonaryWriteExcel(string vl1, CellExcelEpplus vl2)
        {
            NameProperty = vl1;
            CellEpplus = vl2;
        }
        public DictonaryWriteExcel(string vl1, CellExcelEpplus vl2, int vl3, string vl4)
        {
            NameProperty = vl1;
            CellEpplus = vl2;
            Row = vl3;
            DisplayValue = vl4;
        }
    }


    public enum CellExcel
    {
        A = 0,
        B = 1,
        C = 2,
        D = 3,
        E = 4,
        F = 5,
        G = 6,
        H = 7,
        I = 8,
        J = 9,
        K = 10,
        L = 11,
        M = 12,
        N = 13,
        O = 14,
        P = 15,
        Q = 16,
        R = 17,
        S = 18,
        T = 19,
        U = 20,
        V = 21,
        W = 22,
        X = 23,
        Y = 24,
        Z = 25,
        AA = Z + A + 1,
        AB = Z + B + 1,
        AC = Z + C + 1,
        AD = Z + D + 1,
        AE = Z + E + 1,
        AF = Z + F + 1,
        AG = Z + G + 1,
        AH = Z + H + 1,
        AI = Z + I + 1,
        AJ = Z + J + 1,
        AK = Z + K + 1,
        AL = Z + L + 1,
        AM = Z + M + 1,
        AN = Z + N + 1,
        AO = Z + O + 1,
        AP = Z + P + 1,
        AQ = Z + Q + 1,
        AR = Z + R + 1,
        AS = Z + S + 1,
        AT = Z + T + 1,
        AU = Z + U + 1,
        AV = Z + V + 1,
        AW = Z + W + 1,
        AX = Z + X + 1,
        AY = Z + Y + 1,
        AZ = Z + Z + 1,
        BA = Z + Z + A + 2,
        BB = Z + Z + B + 2,
        BC = Z + Z + C + 2,
        BD = Z + Z + D + 2,
        BE = Z + Z + E + 2,
        BF = Z + Z + F + 2,
        BG = Z + Z + G + 2,
        BH = Z + Z + H + 2,
        BI = Z + Z + I + 2,
        BJ = Z + Z + J + 2,
        BK = Z + Z + K + 2,
        BL = BK + 1,
        BM = BL + 1,
        BN = BM + 1,
        BO = BN + 1,
        BP = Z + Z + P + 2,
        BQ = Z + Z + Q + 2,
        BR = Z + Z + R + 2,
        BS = BR + 1,
        BT = BS + 1,
        BU = BS + 2,
        BV = BS + 3,
        BW = BS + 4,
        BX = BS + 5,
        BY = BS + 6,
        BZ = BS + 7,
        CA = BZ + 1,
        CB = BZ + 2,
        CC = BZ + 3,
        CD = BZ + 4,
        CE = BZ + 5,
        CF = BZ + 6,
        CG = BZ + 7,
        CH = CG + 1,
        CI = CG + 2

    }

    public enum CellExcelEpplus
    {
        A = 1,
        B = A + 1,
        C = A + 2,
        D = A + 3,
        E = A + 4,
        F = A + 5,
        G = A + 6,
        H = A + 7,
        I = A + 8,
        J = A + 9,
        K = A + 10,
        L = A + 11,
        M = A + 12,
        N = A + 13,
        O = A + 14,
        P = A + 15,
        Q = A + 16,
        R = A + 17,
        S = A + 18,
        T = A + 19,
        U = A + 20,
        V = A + 21,
        W = A + 22,
        X = A + 23,
        Y = A + 24,
        Z = A + 25,
        AA = Z + A,
        AB = Z + B,
        AC = Z + C,
        AD = Z + D,
        AE = Z + E,
        AF = Z + F,
        AG = Z + G,
        AH = Z + H,
        AI = Z + I,
        AJ = Z + J,
        AK = Z + K,
        AL = Z + L,
        AM = Z + M,
        AN = Z + N,
        AO = Z + O,
        AP = Z + P,
        AQ = Z + Q,
        AR = Z + R,
        AS = Z + S,
        AT = Z + T,
        AU = Z + U,
        AV = Z + V,
        AW = Z + W,
        AX = Z + X,
        AY = Z + Y,
        AZ = Z + Z,
        BA = Z + Z + A,
        BB = Z + Z + B,
        BC = Z + Z + C,
        BD = Z + Z + D,
        BE = Z + Z + E,
        BF = Z + Z + F,
        BG = Z + Z + G,
        BH = Z + Z + H,
        BI = Z + Z + I,
        BJ = Z + Z + J,
        BK = Z + Z + K,
        BL = Z + Z + L,
        BM = Z + Z + M,
        BN = Z + Z + N,
        BO = Z + Z + O
    }

    public enum TemplateExcel
    {
        [Description("TK01-TS.xlsx")]
        TK01 = 1,
        [Description("TK01-TS.V1.xlsx")]
        TK01_Xuat = 11,
        [Description("TK02-TS.xlsx")]
        TK02 = 2,
        [Description("D02-TS.xlsx")]
        D02 = 3,
        [Description("D01b-TS.xlsx")]
        D01b = 4,
        [Description("D01-TS.xlsx")]
        D01 = 5,
        [Description("C15a-TS.xlsx")]
        C15a = 6,
        [Description("D05-TS.xlsx")]
        D05TS = 7,
        [Description("D03-TS.xlsx")]
        D03TS = 8,
        [Description("03B-DSCLTH.xlsx")]
        B03TS = 9,
        [Description("03a-DSCLS.xlsx")]
        A03TS = 10,
        [Description("D07-TS.xlsx")]
        D07TS = 12,
        [Description("D03-TS_New.xlsx")]
        D03TS_In = 13,
        [Description("D03TS-DTK.xlsx")]
        D03TSDTK = 14,
        [Description("TK03-TS.xlsx")]
        TK03TS959 = 15,
        [Description("D02-TS-959.xlsx")]
        D02TS959 = 16,
        [Description("BieuMau.xlsx")]
        BangKe = 17,
        [Description("TK01-TS959.xlsx")]
        TK01TS959 = 18,
        [Description("TK01-TS959Xuat.xlsx")]
        TK01TS959Xuat = 19,
        [Description("D05TS_959.xlsx")]
        D05TS959 = 20,
        [Description("DK04-959.xlsx")]
        DK04_959 = 21,
        [Description("DK05-959.xlsx")]
        DK05_959 = 22,
        [Description("TK03Xem-TS.xlsx")]
        TK03Xem = 23,
        [Description("BienBanTraTheBHYT.xlsx")]
        BienBanTraTheBHYT = 24,
        [Description("C65-HD.xlsx")]
        C65HD_959 = 25,
        [Description("C70A-HD.xlsx")]
        C70A_HD_959 = 26,
        [Description("DK04-959Xem.xlsx")]
        DK04_959Xem = 27,
        [Description("DK05-959-Xem.xlsx")]
        DK05_959_Xem = 28,
        [Description("TB3590-321.xlsx")]
        TB3590321 = 29,
        [Description("TB3590-301.xlsx")]
        TB3590301 = 30,
        [Description("PhieuGiaoNhanHoSo.xlsx")]
        PhieuGiaoNhanHoSo = 31,
        [Description("PGN_601.xlsx")]
        PGN601 = 32,
        [Description("C70BHD_959.xlsx")]
        C70B_HD_959 = 33,
        [Description("C70BHD_959.xlsx")]
        C70BHD_959 = 34,
        [Description("Luong_HSNS.xlsx")]
        Luong_HSNS = 35,
        [Description("Filemau_HSNS.xls")]
        Filemau_HSNS = 36,
        [Description("SMS_D02.xls")]
        SMS_D02 = 37,
        [Description("TB3590-301_moi.xlsx")]
        TB3590301_22022016 = 38,
        [Description("PGN606_15092016.xlsx")]
        PGN606 = 39,
        [Description("HSB_12.xlsx")]
        HSB_12 = 40,
        [Description("HSB_13.xlsx")]
        HSB_13 = 41,
        [Description("HSB_14.xlsx")]
        HSB_14 = 42,
        [Description("D02-TS-959.xls")]
        D02TS959XLS = 43,
        [Description("C70A_HD_636.xlsx")]
        C70A_HD_636 = 44,
        [Description("Export_D02TS_TK01_595.xls")]
        Export_D02 = 45,
        [Description("Export_D05TS_TK01_595.xls")]
        Export_D05 = 46,
        [Description("Export_D03TS_TK01_595.xls")]
        Export_D03 = 47,
        [Description("Export_TK01TS_595.xls")]
        Export_TK01 = 48,
        [Description("Export_D01TS_595.xls")]
        Export_D01 = 49,
        [Description("Export_C70a_HD.xls")]
        Export_C70AHD_Full = 50,
        [Description("Export_C70a_HD_OmDau.xls")]
        Export_C70AHD_OmDau = 51,
        [Description("Export_C70a_HD_ThaiSan.xls")]
        Export_C70AHD_ThaiSan = 52,
        [Description("Export_C70a_HD_DuongSuc.xls")]
        Export_C70AHD_DuongSuc = 53,
        [Description("HSG_02_DS.xlsx")]
        HSG_02_DS = 54,
        [Description("HSG_01A_DSNS.xlsx")]
        HSG_01A_DSNS = 55,
        [Description("HSG_02A.xlsx")]
        HSG_02A = 56,
        [Description("Export_M01BHSB.xls")]
        Export_M01BHSB = 57,
        [Description("Export_M01BHSB_OmDau.xls")]
        Export_M01BHSB_OmDau = 58,
        [Description("Export_M01BHSB_ThaiSan.xls")]
        Export_M01BHSB_ThaiSan = 59,
        [Description("Export_M01BHSB_DuongSuc.xls")]
        Export_M01BHSB_DuongSuc = 60,
    }
}

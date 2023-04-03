using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Tekla.Structures;
using Tekla.Structures.Model;
using thd = TeklaHierarchicDefinitions;
using tsm = Tekla.Structures;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using System.Windows;

namespace TeklaHierarchicDefinitions.Models
{
    public class CheckResult
    {
        public CheckResult(
            string groupErrorCode,
            string errorCode,
            string groupError,
            string error)        
        {
            GroupErrorCode = groupErrorCode;
            ErrorCode = errorCode;
            GroupError = groupError;
            Error = error;
        }

        #region Свойства
        public List<string> GUIDs { get;} = new List<string>();
        public string GroupError { get; private set; }
        public string Error { get; private set; }
        public string GroupErrorCode { get; }

        public string ErrorCode { get; }

        public string CheckResultsCode
        {
            get => GroupErrorCode + " - " + ErrorCode;
        }
        #endregion

        #region Методы
        public void SelectErrorObjects(List<string> guids = null)
        {
            List<ModelObject> c;
            if (guids == null)
                c= TeklaAPIUtils.TeklaDB.model.FetchModelObjects(GUIDs, true);
            else
                c=TeklaAPIUtils.TeklaDB.model.FetchModelObjects(guids, true);
            TeklaAPIUtils.TeklaDB.SelectObjectsInModelView(new System.Collections.ArrayList(c));
        }
        #endregion


    }

    public class CheckResultSummary
    {
        public CheckResultSummary(
            string checkName,
            string result,
            string description)
        {
            CheckName = checkName;
            Result = result;
            Description = description;
        }

        #region Свойства
        public string CheckName { get; private set; }
        public string Result { get; private set; }
        public string Description { get; }

        public int TotalChecks { get; internal set; } = 1;
        public int ErrorNumber { get; internal set; } = 1;

        public double ErrorRate 
        { get
            {
                double res;
                if (TotalChecks > 0 & ErrorNumber > 0)
                {
                    var calculated = ((double)ErrorNumber / (double)TotalChecks * 100);
                    var calc = double.Parse(calculated.ToString());
                    calc = Math.Round(calc, 1);
                    return calc;
                }
                return 0;
            }
        }
        #endregion

        #region Методы

        #endregion
    }


    public static class CheckResultExtensions
    {
        private static string ExcelCellValue(ICell cell)
        {
            if (cell == null)
                return "-";
            switch (cell.CellType)
            {
                case CellType.Unknown:
                    throw new NotImplementedException();
                case CellType.Numeric:
                    return cell.NumericCellValue.ToString();
                case CellType.String:
                    return cell.StringCellValue;
                case CellType.Formula:
                    return cell.StringCellValue;
                case CellType.Blank:
                    return "-";
                case CellType.Boolean: throw new NotImplementedException();
                case CellType.Error:
                    return "-";

                default:
                    throw new ArgumentException(message: "invalid value", paramName: nameof(cell));
            }
        }

        public static (List<CheckResult>, List<CheckResultSummary>) LoadCheckResults(string path)
        {
            var listOfResults = new List<CheckResult>();
            var summaryListOfResults = new List<CheckResultSummary>();
            try
            {
                XSSFWorkbook workbook;
                using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    workbook = new XSSFWorkbook(file);
                }

                ISheet sheet = workbook.GetSheet("Группы");
                // Чтение данных файла
                string[] leafCodes = new string[sheet.LastRowNum];
                leafCodes[0] = null;
                Dictionary<string, string> errorGroupCodeValues = new Dictionary<string, string>();

                for (int row = 1; row <= sheet.LastRowNum; row++)
                {
                    if (sheet.GetRow(row) != null) //null is when the row only contains empty cells 
                    {
                        errorGroupCodeValues[ExcelCellValue(sheet.GetRow(row).GetCell(0))] = ExcelCellValue(sheet.GetRow(row).GetCell(1));
                    }
                }

                sheet = workbook.GetSheet("Параметры (обязательные)");
                // Чтение данных файла
                leafCodes = new string[sheet.LastRowNum];
                leafCodes[0] = null;
                Dictionary<string, string> errorCodeValues = new Dictionary<string, string>();

                for (int row = 1; row <= sheet.LastRowNum; row++)
                {
                    if (sheet.GetRow(row) != null) //null is when the row only contains empty cells 
                    {
                        errorCodeValues[ExcelCellValue(sheet.GetRow(row).GetCell(0))] = ExcelCellValue(sheet.GetRow(row).GetCell(1));
                    }
                }

                sheet = workbook.GetSheet("Объекты");
                // Чтение данных файла
                leafCodes = new string[sheet.LastRowNum];
                leafCodes[0] = null;

                for (int row = 1; row <= sheet.LastRowNum; row++)
                {
                    if (sheet.GetRow(row) != null) //null is when the row only contains empty cells 
                    {
                        var currentCR = listOfResults.LastOrDefault();
                        string guid = ExcelCellValue(sheet.GetRow(row).GetCell(1));
                        string groupErrorCode = ExcelCellValue(sheet.GetRow(row).GetCell(3));
                        string errorCode = ExcelCellValue(sheet.GetRow(row).GetCell(2));
                        if (currentCR == null )
                        {
                            currentCR = new CheckResult(
                                groupErrorCode, errorCode, 
                                errorGroupCodeValues[groupErrorCode], 
                                errorCodeValues[errorCode]);
                            listOfResults.Add(currentCR);
                        }
                        else if(!listOfResults.Select(t=>t.CheckResultsCode).Contains(groupErrorCode + " - " + errorCode))
                        {
                            currentCR = new CheckResult(
                                groupErrorCode, errorCode,
                                errorGroupCodeValues[groupErrorCode],
                                errorCodeValues[errorCode]);
                            listOfResults.Add(currentCR);
                        }
                        currentCR.GUIDs.Add(guid);
                    }
                }

                sheet = workbook.GetSheet("Результаты проверок");
                // Чтение данных файла
                leafCodes = new string[sheet.LastRowNum];
                leafCodes[0] = null;

                for (int row = 1; row <= sheet.LastRowNum; row++)
                {
                    if (sheet.GetRow(row) != null) //null is when the row only contains empty cells 
                    {
                        string checkName = ExcelCellValue(sheet.GetRow(row).GetCell(0));
                        string result = ExcelCellValue(sheet.GetRow(row).GetCell(1));
                        string description = ExcelCellValue(sheet.GetRow(row).GetCell(2));
                        var cr = new CheckResultSummary(checkName, result, description);
                        if (int.TryParse(ExcelCellValue(sheet.GetRow(row).GetCell(3)).ToString(), out int checksErrors))
                            cr.ErrorNumber = checksErrors;
                        if (int.TryParse(ExcelCellValue(sheet.GetRow(row).GetCell(4)).ToString(), out int checksTotal))
                            cr.TotalChecks = checksTotal;
                        summaryListOfResults.Add(cr);
                    }
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
            return ( listOfResults.OrderBy(t => t.CheckResultsCode).ToList(), summaryListOfResults);
        }
    }
}
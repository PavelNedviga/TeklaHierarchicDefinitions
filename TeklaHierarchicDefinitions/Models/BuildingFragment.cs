using NLog;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Tekla.Structures.Model;
using TeklaHierarchicDefinitions.Models;
using TeklaHierarchicDefinitions.TeklaAPIUtils;
using System;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

namespace TeklaHierarchicDefinitions.Models
{
    public class BuildingFragment : INotifyPropertyChanged, IDataErrorInfo
    {
        #region Внутренние параметры объекта

        private HierarchicDefinition _hierarchicDefinition;
        private MyObservableCollection<FoundationGroup> _foundationGroups = new MyObservableCollection<FoundationGroup>();
        #endregion

        #region Конструктор
        internal BuildingFragment(string buildingFragmentMark)
        {
            var mainHDForFragments = TeklaDB.GetHierarchicDefinitionWithName(TeklaDB.hierarchicDefinitionFoundationListName);
            _hierarchicDefinition = TeklaDB.CreateHierarchicDefinitionWithName(buildingFragmentMark, mainHDForFragments);
        }

        internal BuildingFragment(HierarchicDefinition buildingFragmentDefinition)
        {            
            _hierarchicDefinition = buildingFragmentDefinition;
            foreach(var mo in _hierarchicDefinition.GetChildren())
            {
                if (mo is HierarchicObject)
                {
                    var ho = mo as HierarchicObject;
                    //HierarchicObjectInTekla hierarchicObjectInTekla = new HierarchicObjectInTekla();
                    var fg = new FoundationGroup(ho, _hierarchicDefinition);
                    FoundationGroups.Add(fg);
                }
            }
        }
        #endregion

        private static Logger logger = LogManager.GetCurrentClassLogger();

        #region Свойства
        public string BuildingFragmentMark
        {
            get { return _hierarchicDefinition.Name; }
            private set
            {
                _hierarchicDefinition.Name = value;
                OnPropertyChanged();
            }
        }


        public MyObservableCollection<FoundationGroup> FoundationGroups 
        { 
            get => _foundationGroups;
            set 
            {
                _foundationGroups = value;
                OnPropertyChanged();
            }
        }

        public List<string> FoundationMarks
        {
            get 
            {                
                return FoundationGroups.Select(t => t.BasementMark).ToList(); 
            }
        }
        #endregion

        #region Методы
        internal void RemoveBuildingFragment()
        {
            foreach (var fg in FoundationGroups)
            {
                fg.Delete();                
            }
            TeklaDB.DeleteHierarchicDefinition(_hierarchicDefinition);
        }

        internal bool ImportFoundationGroups()
        {
            try
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();

                openFileDialog1.InitialDirectory = TeklaDB.model.GetInfo().ModelPath;
                openFileDialog1.Filter = "Lira export to Excel (*.xlsx)|*.xlsx";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;
                bool? result = openFileDialog1.ShowDialog();
                string path = string.Empty;
                if (result == true)
                {
                    path = openFileDialog1.FileName;
                }

                if (File.Exists(path)) //& !IsFileLocked(new FileInfo(path))

                {
                    try
                    {
                        XSSFWorkbook hssfwb;
                        using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
                        {
                            hssfwb = new XSSFWorkbook(file);
                        }
                        ISheet sheet = hssfwb.GetSheet("эРСУ в специальных элементах");

                        // Чтение данных файла
                        string[] leafCodes = new string[sheet.LastRowNum];
                        leafCodes[0] = null;
                        foreach (var fg in FoundationGroups)
                        {
                            fg.Delete();
                        }
                        FoundationGroups.Clear();
                        for (int row = 1; row <= sheet.LastRowNum; row++)
                        {
                            if (sheet.GetRow(row) != null) //null is when the row only contains empty cells 
                            {
                                FoundationGroup foundationGroupLoad = new FoundationGroup(ExcelCellValue(sheet.GetRow(row).GetCell(0)));
                                foundationGroupLoad.Joint = ExcelCellValue(sheet.GetRow(row).GetCell(1));
                                foundationGroupLoad.St = ExcelCellValue(sheet.GetRow(row).GetCell(2));
                                foundationGroupLoad.Cr = ExcelCellValue(sheet.GetRow(row).GetCell(3));
                                foundationGroupLoad.Sp = ExcelCellValue(sheet.GetRow(row).GetCell(4));
                                foundationGroupLoad.Gr = ExcelCellValue(sheet.GetRow(row).GetCell(5));
                                foundationGroupLoad.Crit = ExcelCellValue(sheet.GetRow(row).GetCell(6));
                                foundationGroupLoad.Rx = double.Parse(ExcelCellValue(sheet.GetRow(row).GetCell(7)));
                                foundationGroupLoad.Ry = double.Parse(ExcelCellValue(sheet.GetRow(row).GetCell(8)));
                                foundationGroupLoad.Rz = double.Parse(ExcelCellValue(sheet.GetRow(row).GetCell(9)));
                                foundationGroupLoad.Rux = double.Parse(ExcelCellValue(sheet.GetRow(row).GetCell(10)));
                                foundationGroupLoad.Ruy = double.Parse(ExcelCellValue(sheet.GetRow(row).GetCell(11)));
                                foundationGroupLoad.Ruz = double.Parse(ExcelCellValue(sheet.GetRow(row).GetCell(12)));
                                foundationGroupLoad.ForceMark = ExcelCellValue(sheet.GetRow(row).GetCell(13));
                                foundationGroupLoad.UpdateAndInsert();
                                FoundationGroups.Add(foundationGroupLoad);
                            }
                        }
                        TeklaDB.model.CommitChanges(this.BuildingFragmentMark + ": added load");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка: проверьте качество заполнения шаблона Excel");
                        return false;
                    }
                }
                else
                {
                    MessageBox.Show("Файл Excel либо отсутствует, либо используется");
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        #endregion

        #region Обработка изменения свойств
        /// <summary>
        /// Отслеживает изменения свойств
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        #endregion

        #region Валидация
        public string Error
        {
            get
            {
                return null;
            }
        }

        public string this[string name]
        {
            get
            {
                string result = null;

                //if (name == "Material")
                //{
                //    if ((Material != null) & (!MaterialIsAllowed()))
                //    {
                //        result = "Check material name";
                //    }
                //}


                return result;
            }
        }

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
        #endregion
    }

}
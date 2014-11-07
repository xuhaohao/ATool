using ATool.Common;
using ATool.Models;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using NLog;
using NPOI;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using TableView;

namespace ATool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            fyType.IsOpenChanged += (s, e) =>
            {
                if (!fyType.IsOpen)
                {
                    var list = medicineTypeView.CollectionData.Where(medicineType => string.IsNullOrEmpty(medicineType.StrName) || medicineType.Price <= 0).ToList();
                    foreach (var medicineType in list)
                    {
                        medicineTypeView.CollectionData.Remove(medicineType);
                    }
                }
            };
            logger.Info("初始化MainWindow完成");
        }

        IWorkbook _hssfworkbook;
        private readonly ObservableCollection<CusItem> _collection = new ObservableCollection<CusItem>();
        private readonly Dictionary<string, MedicineType> _medicineTypeDic = new Dictionary<string, MedicineType>();
        private readonly MultiDictionary<string, string, CusItem> _goodDictionary = new MultiDictionary<string, string, CusItem>();

        private static Logger logger = LogManager.GetCurrentClassLogger();


        private void InitColumns()
        {
            _goodDictionary.Clear();
            foreach (var medicineType in medicineTypeView.CollectionData)
            {
                _medicineTypeDic.Add(medicineType.StrName, medicineType);
                _goodDictionary.Columns.Add(medicineType.StrName);
            }
        }

        private void BtnOpenFile_OnClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "excel2007文件(*.xlsx)|*.xlsx|所有文件(*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == true)
            {
                gridOpenFile.Visibility = Visibility.Visible;
                InitColumns();

                using (Stream stream = openFileDialog.OpenFile())
                {
                    if (POIXMLDocument.HasOOXMLHeader(stream))
                    {
                        _hssfworkbook = new XSSFWorkbook(stream);

                        ISheet sheet = _hssfworkbook.GetSheetAt(0);
                        System.Collections.IEnumerator rows = sheet.GetRowEnumerator();

                        while (rows.MoveNext())
                        {
                            IRow xssfRow = (XSSFRow)rows.Current;

                            var cusItem = BuildCusItem(xssfRow);
                            var row = cusItem.DoctorName;
                            var column = cusItem.Medicine.StrName;
                            if (_goodDictionary.Rows.Contains(row) && _goodDictionary[row].ContainsKey(column) && _goodDictionary[row][column] != null)
                            {
                                _goodDictionary[row][column].Number += cusItem.Number;
                                _goodDictionary[row][column].SumPrice += cusItem.SumPrice;
                            }
                            else
                            {
                                _goodDictionary.Put(cusItem.DoctorName, cusItem.Medicine.StrName, cusItem);
                            }
                        }
                        foreach (var row in _goodDictionary.Rows)
                        {
                            foreach (var column in _goodDictionary.Columns)
                            {
                                if (_goodDictionary[row][column] == null)
                                {
                                    var cusItem = new CusItem();
                                    cusItem.DoctorName = row;
                                    cusItem.Medicine = _medicineTypeDic[column];
                                    cusItem.Number = 0;
                                    cusItem.SumPrice = 0;
                                    _goodDictionary[row][column] = cusItem;
                                }
                            }
                        }

                        BindingTvNormal();
                        BindingTvCross();
                    }
                    else
                    {
                        logger.Error("无法识别OOXML头，请检查是否打开的是xlsx文件");
                    }
                }
                gridOpenFile.Visibility = Visibility.Collapsed;
            }
            
        }

        private void BindingTvNormal()
        {
            _collection.Clear();
            var allCusItem = new CusItem { DoctorName = "全部总计", Number = 0, SumPrice = 0 };

            foreach (var row in _goodDictionary.Rows)
            {
                var rowCusItem = new CusItem { DoctorName = string.Format("{0}[总计]",row), Number = 0, SumPrice = 0 };

                foreach (var column in _goodDictionary[row].Keys)
                {
                    var cusItem = _goodDictionary[row][column];
                    rowCusItem.SumPrice += cusItem.SumPrice;
                    _collection.Add(cusItem);
                }
                allCusItem.SumPrice += rowCusItem.SumPrice;
                _collection.Add(rowCusItem);
            }
            _collection.Add(allCusItem);

            tvNorMal.ItemsSource = _collection;
        }

        private void BindingTvCross()
        {
            tvCross.Columns.Clear();

            var tableViewFirstColumn = new TableViewColumn { Width = 100, ContextBindingPath = "Key",Background = new SolidColorBrush(Colors.CornflowerBlue)};
            tvCross.Columns.Add(tableViewFirstColumn);

            foreach (var column in _goodDictionary.Columns)
            {
                var tableViewColumn = new TableViewColumn
                {
                    Title = column,
                    Width = 160,
                    ContextBinding = new Binding(string.Format("Value[{0}].SumPrice", column))
                };
                tvCross.Columns.Add(tableViewColumn);
            }
            //求和列
            var sumColumn = new TableViewColumn
            {
                Title = "总计",
                Width = 160,
                ContextBinding = new Binding("Value[总计].SumPrice")
            };
            tvCross.Columns.Add(sumColumn);

            foreach (var row in _goodDictionary.Rows)
            {
                var cusItem = new CusItem { DoctorName = row + "[总计]", Medicine = null, Number = 0, SumPrice = 0 };
                foreach (var column in _goodDictionary.Columns)
                {
                    if (!column.Equals("总计"))
                    {
                        cusItem.SumPrice += _goodDictionary[row][column].SumPrice;
                    }
                }
                _goodDictionary.Put(row, "总计", cusItem);
            }

            //求和行
            var dictionary = new Dictionary<string, CusItem>();
            foreach (var column in _goodDictionary.Columns)
            {

                var cusItem = new CusItem { DoctorName = column, Medicine = _medicineTypeDic.ContainsKey(column) ? _medicineTypeDic[column] : null, Number = 0, SumPrice = 0 };
                foreach (var row in _goodDictionary.Rows)
                {
                    cusItem.Number += _goodDictionary[row][column].Number;
                    cusItem.SumPrice += _goodDictionary[row][column].SumPrice;
                }
                dictionary.Add(column, cusItem);
            }
            _goodDictionary.Dic.Add("全部总计", dictionary);
            

            tvCross.ItemsSource = _goodDictionary.Dic;
        }



        private CusItem BuildCusItem(IRow row)
        {
            var cusItem = new CusItem();
            //DoctorName
            cusItem.DoctorName = row.GetCell(0).ToString().Trim();

            //Medicine
            var cellContent = row.GetCell(1).ToString();
            if (!_medicineTypeDic.ContainsKey(cellContent))
            {
                var medicineType = new MedicineType(cellContent,0);
                _medicineTypeDic.Add(cellContent, medicineType);
            }
            cusItem.Medicine = _medicineTypeDic[cellContent];

            //Number
            cusItem.Number = int.Parse(row.GetCell(2).ToString());
            //SumPrice
            cusItem.SumPrice = cusItem.Medicine.Price*cusItem.Number;

            return cusItem;
        }

        private void BtnExport_OnClick(object sender, RoutedEventArgs e)
        {
            gridOpenFile.Visibility = Visibility.Visible;
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.OverwritePrompt = true;

            var dateTime = DateTime.Now;
            saveFileDialog.FileName = string.Format("统计{0}-{1}-{2}", dateTime.Year, dateTime.Month, dateTime.Day);
            saveFileDialog.DefaultExt = "xlsx";
            saveFileDialog.Filter = "excel2007文件(*.xlsx)|*.xlsx|所有文件(*.*)|*.*";
            saveFileDialog.InitialDirectory =
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (saveFileDialog.ShowDialog() == true)
            {
                using (Stream fileStream = saveFileDialog.OpenFile())
                {
                        var newWorkBoox = new XSSFWorkbook();
                        var newSheet = newWorkBoox.CreateSheet();

                        newSheet.SetColumnWidth(0,256*15);
                        newSheet.SetColumnWidth(1, 256*50);
                        newSheet.SetColumnWidth(2, 256*20);
                        newSheet.SetColumnWidth(3, 256*20);
                        
                        for (int i = 0; i < _collection.Count; i++)
                        {
                            IRow row = newSheet.CreateRow(i);

                            var cusItem = _collection[i];
                            row.CreateCell(0).SetCellValue(cusItem.DoctorName);
                            if (cusItem.Medicine != null)
                            {
                                row.CreateCell(1).SetCellValue(cusItem.Medicine.StrName);
                            }
                            row.CreateCell(2).SetCellValue(cusItem.Number);
                            row.CreateCell(3).SetCellValue(cusItem.SumPrice);
                        }
                        newWorkBoox.Write(fileStream);
                }
            }
            gridOpenFile.Visibility = Visibility.Collapsed;
        }


        public MemoryStream RenderToExcel()
        {
            var ms = new MemoryStream();
            var newWorkBoox = new XSSFWorkbook();
            var newSheet = newWorkBoox.CreateSheet();

            for (int i = 0; i < _collection.Count; i++)
            {
                IRow row = newSheet.CreateRow(0);

                row.CreateCell(0).SetCellValue(_collection[i].DoctorName);
                row.CreateCell(1).SetCellValue(_collection[i].Medicine.StrName);
                row.CreateCell(2).SetCellValue(_collection[i].Number);
                row.CreateCell(3).SetCellValue(_collection[i].SumPrice);
            }

            newWorkBoox.Write(ms);
            ms.Flush();
            ms.Position = 0;

            return ms;
        }


        private void BtnType_OnClick(object sender, RoutedEventArgs e)
        {
            fyType.IsOpen = true;
        }
    }
}

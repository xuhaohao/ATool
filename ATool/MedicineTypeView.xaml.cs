using System.Collections.ObjectModel;
using ATool.Common;
using ATool.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Controls;
using NLog;

namespace ATool
{
    /// <summary>
    /// MedicineTypeView.xaml 的交互逻辑
    /// </summary>
    public partial class MedicineTypeView : UserControl
    {
        public MedicineTypeView()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (File.Exists(StrPath))
                {
                    var strContent = File.ReadAllText(StrPath);
                    if (!string.IsNullOrEmpty(strContent))
                    {
                        var list = Methods.Desrialize<List<MedicineType>>(strContent);
                        if (list != null)
                        {
                            foreach (var medicineType in list)
                            {
                                Add(medicineType.StrName, medicineType.Price);
                            }
                        }
                    }
                }
                if (CollectionData.Count == 0)
                {
                    Add("阿仑膦酸钠片10mg*7", 12);
                    Add("奥美拉唑肠溶胶囊20mg*14", 2);
                    Add("百乐眠胶囊0.27g*24", 13);
                    Add("醋酸曲安奈德益康唑乳膏15g", 9);
                    Add("碘海醇注射液(欧苏)100ml:30g(I).", 3);
                    Add("厄贝沙坦片75mg*12", 5);
                    Add("甲钴胺胶囊0.5mg*50", 6);
                    Add("甲磺酸罗哌卡因注射液10ml:89.4mg*2支", 22);
                    Add("克拉霉素分散片0.125g*6", 12);
                    Add("蓝芩口服液10ml*6支", 15);
                    Add("氯沙坦钾片50mg*7", 7);
                    Add("马来酸依那普利片10mg*32", 9);
                    Add("美洛昔康片7.5mg*10", 12);
                    Add("双氯芬酸钾片25mg*24", 8);
                    Add("苏黄止咳胶囊0.45g*24", 10);
                    Add("头孢丙烯片0.25g*10", 2);
                    Add("硝苯地平缓释片10mg*32", 17);
                    Add("盐酸西替利嗪片10mg*12", 12);
                    Add("盐酸左氧氟沙星胶囊0.1g*12", 7);
                    Add("盐酸左氧氟沙星氯化钠注射液100ml:0.3g", 10);
                    Add("依帕司他片50mg*10", 5);
                    Add("银杏叶片19.2mg*36", 6);
                    Add("注射用奥扎格雷钠20mg", 15);
                }

                App.Current.Exit += (s, e) =>
                {
                    var strContent = Methods.Serialize(CollectionData);
                    File.WriteAllText(StrPath, strContent);
                    logger.Info("持久化种类配置文件完成");
                };

                dgMain.ItemsSource = CollectionData;
            }
        }

        private static readonly string StrPath = string.Format(@"{0}setting.data", AppDomain.CurrentDomain.BaseDirectory);

        public ObservableCollection<MedicineType> CollectionData = new ObservableCollection<MedicineType>();

        private void Add(string strName, float price)
        {
            var medicineType = new MedicineType(strName, price);
            //Dic.Add(strName, medicineType);
            CollectionData.Add(medicineType);
        }


        private static Logger logger = LogManager.GetCurrentClassLogger();
    }
}

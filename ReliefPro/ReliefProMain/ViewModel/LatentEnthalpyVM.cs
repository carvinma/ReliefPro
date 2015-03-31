/*
 * 塔基础信息界面
 * 该文件主要是实现塔基础信息的导入与信息展示。
 * 信息包括了塔的名称，塔层数以及塔的feed,product,condenser,reboiler
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using ReliefProModel;
using ReliefProMain.Commands;
using ReliefProDAL;
using ReliefProBLL.Common;
using ReliefProCommon.CommonLib;
using NHibernate;
using ReliefProMain.View;
using UOMLib;
using ReliefProCommon.Enum;
using ReliefProBLL;

namespace ReliefProMain.ViewModel
{
    public class LatentEnthalpyVM : ViewModelBase
    {
        public ISession SessionPlant { set; get; }
        public ISession SessionProtectedSystem { set; get; }
        public string DirPlant { set; get; }
        public string DirProtectedSystem { set; get; }
        public SourceFile SourceFileInfo { set; get; }
        public string SourceFileName { set; get; }
        private string _TowerName;
        public string TowerName
        {
            get
            {
                return this._TowerName;
            }
            set
            {
                this._TowerName = value;
                OnPropertyChanged("TowerName");
            }
        }
        private string _TowerType;
        public string TowerType
        {
            get
            {
                return this._TowerType;
            }
            set
            {
                this._TowerType = value;
                TowerType_Color = ColorBorder.blue.ToString();
                OnPropertyChanged("TowerType");
            }
        }
        private string _TowerType_Color;
        public string TowerType_Color
        {
            get
            {
                return this._TowerType_Color;
            }
            set
            {
                this._TowerType_Color = value;
                OnPropertyChanged("TowerType_Color");
            }
        }
        private string _Desciption;
        public string Desciption
        {
            get
            {
                return this._Desciption;
            }
            set
            {
                this._Desciption = value;
                OnPropertyChanged("Desciption");
            }
        }
        private int _StageNumber;
        public int StageNumber
        {
            get
            {
                return this._StageNumber;
            }
            set
            {
                this._StageNumber = value;
                OnPropertyChanged("StageNumber");
            }
        }
        private ObservableCollection<string> _TowerTypes;
        public ObservableCollection<string> TowerTypes
        {
            get
            {
                return this._TowerTypes;
            }
            set
            {
                this._TowerTypes = value;
                OnPropertyChanged("TowerTypes");
            }
        }

        private string _ColorImport;
        public string ColorImport
        {
            get
            {
                return this._ColorImport;
            }
            set
            {
                this._ColorImport = value;
                OnPropertyChanged("ColorImport");
            }
        }



        public List<string> listMethodName { get; set; }
        public List<int> listStageNumber { get; set; }

        private string selectedMethodName = "100-30-30";
        public string SelectedMethodName
        {
            get { return selectedMethodName; }
            set
            {
                selectedMethodName = value;
                this.OnPropertyChanged("SelectedMethodName");
            }
        }

        private int? selectedStageNumber;
        public int? SelectedStageNumber
        {
            get { return selectedStageNumber; }
            set
            {
                selectedStageNumber = value;
                this.OnPropertyChanged("SelectedStageNumber");
            }
        }

        public UOMLib.UOMEnum uomEnum { get; set; }

        TowerDAL dbtower;
        Tower tower;
        int op = 1;
        public LatentEnthalpyVM(string towerName, ISession sessionPlant, ISession sessionProtectedSystem, string dirPlant, string dirProtectedSystem)
        {
            listMethodName = new List<string> { "Dew point", "Bubble point", "5% mol"};

            //是从１到塔的总层数。　　塔的总层数，可以从从tbtower表里获取
            SessionPlant = sessionPlant;
            SessionProtectedSystem = sessionProtectedSystem;
            uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionPlant == this.SessionPlant);
            ColorImport = "Gray";

            DirPlant = dirPlant;
            DirProtectedSystem = dirProtectedSystem;
            TowerName = towerName;
            TowerTypes = GetTowerTypes();
            TowerType = TowerTypes[0];
            if (!string.IsNullOrEmpty(TowerName))
            {
                dbtower = new TowerDAL();
                tower = dbtower.GetModel(SessionProtectedSystem);
                TowerName = tower.TowerName;
                TowerType = tower.TowerType;
                Desciption = tower.Description;
                StageNumber = tower.StageNumber;
                if(tower.StageNumber>0)
                {
                    listStageNumber.AddRange(Enumerable.Range(1, tower.StageNumber));
                }
                TowerType = tower.TowerType;
                TowerType_Color = tower.TowerType_Color;
               
                ColorImport = ColorBorder.blue.ToString();
            }
            else
            {
                TowerType_Color = ColorBorder.green.ToString();
                ColorImport = ColorBorder.red.ToString();
            }
        }

         private ICommand _SaveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (_SaveCommand == null)
                {
                    _SaveCommand = new RelayCommand(Save);

                }
                return _SaveCommand;
            }
        }



        public void Save(object obj)
        {
            if (string.IsNullOrEmpty(TowerName))
            {
                MessageBox.Show("You must Import Data first.", "Message Box");
                ColorImport = "red";
                return;
            }
            if (Feeds.Count == 0)
            {
                return;
            }
            if (op == 0)
            {
                ReImportBLL reimportbll = new ReImportBLL(SessionProtectedSystem);
                reimportbll.DeleteAllData();

                AddTowerInfo();
                System.Windows.Window wd = obj as System.Windows.Window;
                if (wd != null)
                {
                    wd.DialogResult = true;
                }
            }
            else if (op == 2)
            {
                MessageBoxResult r = MessageBox.Show("Are you sure to reimport all data?", "Message Box", MessageBoxButton.YesNo);
                if (r == MessageBoxResult.Yes)
                {
                    ReImportBLL reimportbll = new ReImportBLL(SessionProtectedSystem);
                    reimportbll.DeleteAllData();
                    AddTowerInfo();
                }
                System.Windows.Window wd = obj as System.Windows.Window;
                if (wd != null)
                {
                    wd.DialogResult = true;
                }
            }
            else
            {
                Tower tower = dbtower.GetModel(SessionProtectedSystem);
                if (tower.TowerType != TowerType)
                {
                    MessageBoxResult r = MessageBox.Show("Are you sure for changing tower type,it will delete all scenario?", "Message Box", MessageBoxButton.YesNo);
                    if (r == MessageBoxResult.Yes)
                    {
                        ScenarioBLL scbll = new ScenarioBLL(SessionProtectedSystem);
                        scbll.DeleteSCOther();
                        scbll.DeleteScenario();
                        tower.TowerType = TowerType;
                        tower.Description = Desciption;
                        tower.TowerType_Color = TowerType_Color;
                        dbtower.Update(tower, SessionProtectedSystem);
                        //SessionProtectedSystem.Flush();
                    }
                }
                SourceFileDAL sfdal = new SourceFileDAL();
                SourceFileInfo = sfdal.GetModel(tower.SourceFile, SessionPlant);
                System.Windows.Window wd = obj as System.Windows.Window;
                if (wd != null)
                {
                    wd.DialogResult = true;
                }
            }
        }

        /// <summary>
        /// 获得塔类型列表
        /// </summary>
        /// <returns></returns>
        private ObservableCollection<string> GetTowerTypes()
        {
            ObservableCollection<string> list = new ObservableCollection<string>();
            list.Add("Distillation");
            list.Add("Absorber");
            list.Add("Absorbent Regenerator");
            return list;
        }
      
    }


}

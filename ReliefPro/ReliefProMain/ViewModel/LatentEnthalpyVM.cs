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
       
        public List<string> ListMethodName { get; set; }
        public List<int> ListStageNumber { get; set; }

        private string selectedMethodName = "Dew point";
        public string SelectedMethodName
        {
            get { return selectedMethodName; }
            set
            {
                selectedMethodName = value;
                this.OnPropertyChanged("SelectedMethodName");
            }
        }

        private int? selectedStageNumber=1;
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
       
        public LatentEnthalpyVM(int stageNumber)
        {
            ListMethodName = new List<string> { "Dew point", "Bubble point", "5% mol"};
            ListStageNumber.AddRange(Enumerable.Range(1, stageNumber));           
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

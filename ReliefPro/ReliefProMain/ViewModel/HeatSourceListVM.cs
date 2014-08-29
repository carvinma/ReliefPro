using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Collections.ObjectModel;
using ReliefProModel;
using ReliefProMain.Commands;
using ReliefProDAL;
using ReliefProBLL.Common;
using ReliefProMain.Interface;
using ReliefProMain.Service;
using ReliefProMain.View;
using ReliefProMain.Models;
using NHibernate;
using ReliefProMain.ViewModel.Drums;
using System.Windows;
using System.IO;
using ReliefProMain.CustomControl;
using UOMLib;

namespace ReliefProMain.ViewModel
{
    public class HeatSourceListVM : ViewModelBase
    {
        private ISession SessionPlant { set; get; }
        private ISession SessionProtectedSystem { set; get; }
        private SourceFile SourceFileInfo;
        private HeatSourceDAL heatSourceDAL;
        private int SourceID;
        private Dictionary<string, ProIIEqData> dicHX;
        private ObservableCollection<HeatSourceModel> _HeatSources;
        public ObservableCollection<HeatSourceModel> HeatSources
        {
            get { return _HeatSources; }
            set
            {
                _HeatSources = value;
                OnPropertyChanged("HeatSources");
            }
        }
        private HeatSourceModel _SelectedHeatSource;
        public HeatSourceModel SelectedHeatSource
        {
            get { return _SelectedHeatSource; }
            set
            {
                _SelectedHeatSource = value;

                OnPropertyChanged("SelectedHeatSource");
            }
        }

        private ObservableCollection<string> _HeatSourceNames;
        public ObservableCollection<string> HeatSourceNames
        {
            get { return _HeatSourceNames; }
            set
            {
                _HeatSourceNames = value;
                OnPropertyChanged("HeatSourceNames");
            }
        }

        private ObservableCollection<string> _HeatSourceTypes;
        public ObservableCollection<string> HeatSourceTypes
        {
            get { return _HeatSourceTypes; }
            set
            {
                _HeatSourceTypes = value;
                OnPropertyChanged("HeatSourceTypes");
            }
        }

        public UOMLib.UOMEnum uomEnum { get; set; }
        private string TargetUnit;
        public HeatSourceListVM(int SourceID, SourceFile sourceFileInfo, ISession SessionPlant, ISession SessionProtectedSystem)
        {
            uomEnum = new UOMLib.UOMEnum(SessionPlant);
            this.SourceID = SourceID;
            heatSourceDAL = new HeatSourceDAL();
            this.SessionPlant = SessionPlant;
            this.SessionProtectedSystem = SessionProtectedSystem;
            SourceFileInfo = sourceFileInfo;
            dicHX = new Dictionary<string, ProIIEqData>();
            HeatSources = new ObservableCollection<HeatSourceModel>();
            IList<HeatSource> list = heatSourceDAL.GetAllList(SessionProtectedSystem, SourceID);
            for (int i = 0; i < list.Count; i++)
            {
                HeatSource m = list[i];
                HeatSourceModel model = new HeatSourceModel(m);
                model.SeqNumber = i;
                HeatSources.Add(model);
            }
            HeatSourceNames = GetHeatSourceNames();
            HeatSourceTypes = GetHeatSourceTypes();
            changeUnit += new ChangeUnitDelegate(ExcuteThumbMoved);
            ReadConvert();
        }

        private ICommand _AddCommand;
        public ICommand AddCommand
        {
            get
            {
                if (_AddCommand == null)
                {
                    _AddCommand = new DelegateCommand(Add);

                }
                return _AddCommand;
            }
        }

        public void Add()
        {
            HeatSource m = new HeatSource();
            m.SourceID = SourceID;
            HeatSourceModel model = new HeatSourceModel(m);
            model.data = dicHX;
            model.SeqNumber = HeatSources.Count;
            HeatSources.Add(model);
        }

        private ICommand _DeleteCommand;
        public ICommand DeleteCommand
        {
            get
            {
                if (_DeleteCommand == null)
                {
                    _DeleteCommand = new RelayCommand(Delete);

                }
                return _DeleteCommand;
            }
        }

        public void Delete(object obj)
        {
            int idx = int.Parse(obj.ToString());
            HeatSources.RemoveAt(idx);
            for (int i = 0; i < HeatSources.Count; i++)
            {
                HeatSourceModel model = HeatSources[i];
                model.SeqNumber = i;
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
            foreach (HeatSourceModel m in HeatSources)
            {
                if (string.IsNullOrEmpty(m.HeatSourceName))
                {
                    MessageBox.Show("HeatSource must be not empty.", "Message Box");
                    return;
                }
            }
            WriteConvert();
            IList<HeatSource> list = heatSourceDAL.GetAllList(SessionProtectedSystem, SourceID);
            for (int i = 0; i < list.Count; i++)
            {
                heatSourceDAL.Delete(list[i], SessionProtectedSystem);
            }

            foreach (HeatSourceModel m in HeatSources)
            {
                heatSourceDAL.Add(m.model, SessionProtectedSystem);
            }
            SessionProtectedSystem.Flush();
            System.Windows.Window wd = obj as System.Windows.Window;

            if (wd != null)
            {
                wd.DialogResult = true;
            }
        }

        private ObservableCollection<string> GetHeatSourceNames()
        {
            string fileName = SourceFileInfo.FileName;
            ObservableCollection<string> list = new ObservableCollection<string>();
            ProIIEqDataDAL dbproiieq = new ProIIEqDataDAL();
            IList<ProIIEqData> eqs = dbproiieq.GetAllList(SessionPlant, fileName, "Hx");
            foreach (ProIIEqData eq in eqs)
            {
                list.Add(eq.EqName);
                dicHX.Add(eq.EqName, eq);
            }
            return list;
        }
        private ObservableCollection<string> GetHeatSourceTypes()
        {
            ObservableCollection<string> list = new ObservableCollection<string>();
            list.Add("Fired Heater");
            list.Add("HX");
            list.Add("Feed/Bottom HX");
            return list;
        }

        private void WriteConvert()
        {
            if (!string.IsNullOrEmpty(TargetUnit))
            {
                foreach (var t in HeatSources)
                {
                    t.Duty = UnitConvert.Convert(TargetUnit, UOMEnum.EnthalpyDuty, t.Duty);
                }
            }
        }
        private void ReadConvert()
        {
            this.TargetUnit = uomEnum.UserEnthalpyDuty;
            foreach (var t in HeatSources)
            {
                t.Duty = UnitConvert.Convert(UOMEnum.EnthalpyDuty, uomEnum.UserEnthalpyDuty, t.Duty);
            }
        }
        public ChangeUnitDelegate changeUnit { get; set; }
        public void ExcuteThumbMoved(object ColInfo, object OrigionUnit, object TargetUnit)
        {
            int k = string.Compare(OrigionUnit.ToString(), TargetUnit.ToString(), true);
            if (k != 0 && ColInfo != null && ColInfo.ToString() == "2")
            {
                this.TargetUnit = TargetUnit.ToString();
                foreach (var t in HeatSources)
                {
                    t.Duty = UnitConvert.Convert(OrigionUnit.ToString(), TargetUnit.ToString(), t.Duty);
                }
            }
        }
    }
}

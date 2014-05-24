using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using NHibernate;
using ReliefProMain.Commands;

namespace ReliefProMain.ViewModel.Drum
{
    public class DrumDepressuringVM : ViewModelBase
    {
        public ICommand OKCMD { get; set; }
        private string selectedShotCut = "Shortcut";
        public string SelectedShotCut
        {
            get { return selectedShotCut; }
            set
            {
                selectedShotCut = value;
                if (value == "Shortcut")
                {
                    isEnableFireHeatInput = false;
                }
                else
                {
                    isEnableFireHeatInput = true;
                }
                OnPropertyChanged("SelectedShotCut");
            }
        }
        private bool enableFireHeatInput;
        public bool isEnableFireHeatInput
        {
            get { return enableFireHeatInput; }
            set
            {
                enableFireHeatInput = value;
                OnPropertyChanged("isEnableFireHeatInput");
            }
        }

        public List<string> lstShortCut { get; set; }

        private string selectedDeprRqe = "21bar/min";
        public string SelectedDeprRqe
        {
            get { return selectedDeprRqe; }
            set
            {
                selectedDeprRqe = value;
                OnPropertyChanged("SelectedDeprRqe");
            }
        }
        public List<string> lstDeprRqe { get; set; }

        private string selectedHeatInput = "API 521";
        public string SelectedHeatInput
        {
            get { return selectedHeatInput; }
            set
            {
                selectedHeatInput = value;
                OnPropertyChanged("SelectedHeatInput");
            }
        }
        public List<string> lstHeatInput { get; set; }
        private ISession SessionPS;
        private ISession SessionPF;
        public DrumDepressuringVM(int ScenarioID, ISession SessionPS, ISession SessionPF)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
            OKCMD = new DelegateCommand<object>(Save);

            lstShortCut = new List<string>(new[] { "Shortcut", "PROII DEPR Unit" });
            lstDeprRqe = new List<string>{
                "21bar/min","7bar/min","50% Design pressure in 15min","7barg in 15min","Specify"};
            lstHeatInput = new List<string>(new[] { "API 521", "API 521 Scale", "API 2000", "API 2000 Scale" });
        }
        private void WriteConvertModel()
        { }
        private void Save(object obj)
        {
            if (obj != null)
            {
                System.Windows.Window wd = obj as System.Windows.Window;
                if (wd != null)
                {
                    WriteConvertModel();
                    wd.DialogResult = true;
                }
            }
        }
    }
}

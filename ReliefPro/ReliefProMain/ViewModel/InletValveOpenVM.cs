using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ReliefProModel;
using ReliefProMain.Commands;
using ReliefProDAL;
using ReliefProBLL.Common;
using ReliefProMain.Interface;
using ReliefProMain.Service;
using NHibernate;
using ProII91;

namespace ReliefProMain.ViewModel
{
    public class InletValveOpenVM : ViewModelBase
    {
        public string dbProtectedSystemFile { get; set; }
        public string dbPlantFile { get; set; }
        private string SourceFile;
        private string EqName;
        public List<string> OperatingPhases { get; set; }


        private string _SelectedOperatingPhase { get; set; }
        public string SelectedOperatingPhase
        {
            get { return _SelectedOperatingPhase; }
            set
            {
                _SelectedOperatingPhase = value;
                OnPropertyChanged("SelectedOperatingPhase");
            }
        }
        private string _SelectedVessel { get; set; }
        public string SelectedVessel
        {
            get { return _SelectedVessel; }
            set
            {
                _SelectedVessel = value;
                UpStreamNames = GetEqProducts(dicEqData[_SelectedVessel]);

                OnPropertyChanged("SelectedVessel");
            }
        }

        private string _SelectedUpStream { get; set; }
        public string SelectedUpStream
        {
            get { return _SelectedUpStream; }
            set
            {
                _SelectedUpStream = value;
                MaxOperatingPressure = GetProIIStreamDataPressure(_SelectedUpStream);
                OnPropertyChanged("SelectedUpStream");
            }
        }

        private string _MaxOperatingPressure { get; set; }
        public string MaxOperatingPressure
        {
            get { return _MaxOperatingPressure; }
            set
            {
                _MaxOperatingPressure = value;

                OnPropertyChanged("MaxOperatingPressure");
            }
        }



        private ObservableCollection<string> _Vessels { get; set; }
        public ObservableCollection<string> Vessels
        {
            get { return _Vessels; }
            set
            {
                _Vessels = value;
                OnPropertyChanged("Vessels");
            }
        }
        public List<string> GetOperatingPhases()
        {
            List<string> list = new List<string>();
            list.Add("Full Liquid");
            list.Add("Vapor-liquid");
            return list;
        }

        private ObservableCollection<string> _UpStreamNames { get; set; }
        public ObservableCollection<string> UpStreamNames
        {
            get { return _UpStreamNames; }
            set
            {
                _UpStreamNames = value;
                OnPropertyChanged("UpStreamNames");
            }
        }
        private Dictionary<string, ProIIEqData> dicEqData = new Dictionary<string, ProIIEqData>();
        
        public InletValveOpenVM(int ScenarioID, string dbPSFile, string dbPFile)
        {
            OperatingPhases=GetOperatingPhases();
            dbProtectedSystemFile = dbPSFile;
            dbPlantFile = dbPFile;
            using (var helper = new NHibernateHelper(dbProtectedSystemFile))
            {
                var Session = helper.GetCurrentSession();
                dbTower dbtower = new dbTower();
                Tower m = dbtower.GetModel(Session);
                if (m != null)
                {
                    SourceFile = m.PrzFile;
                    EqName = m.TowerName;
                }
               

            }
            BasicUnit BU;
            using (var helper = new NHibernateHelper(dbPlantFile))
            {
                var Session = helper.GetCurrentSession();
                dbBasicUnit dbBU = new dbBasicUnit();
                IList<BasicUnit> list = dbBU.GetAllList(Session);
                BU = list.Where(s => s.IsDefault == 1).Single();
                Vessels = GetVessels(Session);
            }
            
            
        }

        private ICommand _Save;
        public ICommand Save
        {
            get
            {
                if (_Save == null)
                {
                    _Save = new RelayCommand(OKClick);

                }
                return _Save;
            }
        }

        private void OKClick(object window)
        {
            
            System.Windows.Window wd = window as System.Windows.Window;

            if (wd != null)
            {
                wd.Close();
            }
        }

        private ObservableCollection<string> GetVessels(ISession Session)
        {
            ObservableCollection<string> list = new ObservableCollection<string>();

            dbProIIEqData dbeq = new dbProIIEqData();
            IList<ProIIEqData> eqlist = dbeq.GetAllList(Session, SourceFile, "Flash");
            foreach (ProIIEqData d in eqlist)
            {
                if (d.EqName != EqName)
                {
                    list.Add(d.EqName);
                    dicEqData.Add(d.EqName, d);
                }
            }
            eqlist = dbeq.GetAllList(Session, SourceFile, "Column");
            foreach (ProIIEqData d in eqlist)
            {
                if (d.EqName != EqName)
                {
                    list.Add(d.EqName);
                    dicEqData.Add(d.EqName, d);
                }
            }
            return list;
        }

        public ObservableCollection<string> GetEqProducts(ProIIEqData data)
        {
            ObservableCollection<string> list = new ObservableCollection<string>();
            string productdata = data.ProductData;
            string[] arrProducts = productdata.Split(',');
         
            for (int i = 0; i < arrProducts.Length; i++)
            {
                list.Add(arrProducts[i]);
            }
            return list;
        }

        private string  GetProIIStreamDataPressure(string streamName)
        {
            string press = "0";
            using (var helper = new NHibernateHelper(dbPlantFile))
            {
                var Session = helper.GetCurrentSession();
                dbProIIStreamData db=new dbProIIStreamData();

               ProIIStreamData data = db.GetModel(Session, streamName);
               press = data.Pressure;
            }
            return press;
        }




        private double Darcy(string Rmass,string Cv,string UPStreamPressure,string DownStreamPressure)
        {
            double dRmass = double.Parse(Rmass);
            double dCv = double.Parse(Cv);
            double dUPStreamPressure = double.Parse(UPStreamPressure);
            double dDownStreamPressure = double.Parse(DownStreamPressure);
            double detaP = dUPStreamPressure - dDownStreamPressure;
            if (detaP < 0)
                return 0;
            else
            {
                double w = 0.00075397 * dCv * Math.Pow(detaP * 1000 * dRmass, 2) * 3600;
                return w;
            }
        }

        private void LiquidFlashing(string Rmass, string Cv, string UPStreamPressure, string DownStreamPressure)
        {
            string dir = "";
            string vapor=Guid.NewGuid().ToString().Substring(0,6);
            string liquid=Guid.NewGuid().ToString().Substring(0,6);
            CustomStream cs=new CustomStream();
            double Wliquidvalve = Darcy(Rmass, Cv, UPStreamPressure, DownStreamPressure);
            FlashWCalculation flashCalc = new FlashWCalculation();
            string f= flashCalc.Calculate("", 1, DownStreamPressure, 5, "0",cs,vapor,liquid,Wliquidvalve.ToString(),dir);

            //读取vapor的weightflow

        }

        private void VaporBreakthrough(string eqType,string przFile)
        {
            
            if (eqType == "Column")
            {
                //Copy 塔底的最后一块塔板的Vapor
            }
            else
            {
                //正常Vapor 的压力
            }
            

        }

    }
}

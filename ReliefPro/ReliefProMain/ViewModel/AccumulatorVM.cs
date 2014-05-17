using System;
using System.Collections.Generic;
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

namespace ReliefProMain.ViewModel
{
    public class AccumulatorVM : ViewModelBase
    {
        public string dbProtectedSystemFile { get; set; }
        public string dbPlantFile { get; set; } 
       
        private bool _Horiz;
        public bool Horiz
        {
            get
            {
                return this._Horiz;
            }
            set
            {
                this._Horiz = value;
                OnPropertyChanged("Horiz");
            }
        }

        private bool _Vertical;
        public bool Vertical
        {
            get
            {
                return this._Vertical;
            }
            set
            {
                this._Vertical = value;
                OnPropertyChanged("Vertical");
            }
        }

        private string _Diameter;
        public string Diameter
        {
            get
            {
                return this._Diameter;
            }
            set
            {
                this._Diameter = value;
                if (Horiz && string.IsNullOrEmpty(NormalLiquidLevel) && !string.IsNullOrEmpty(_Diameter))
                {
                    NormalLiquidLevel=(double.Parse(Diameter)/2).ToString();
                }
                OnPropertyChanged("Diameter");
            }
        }

        private string _Length;
        public string Length
        {
            get
            {
                return this._Length;
            }
            set
            {
                this._Length = value;
                if (Vertical && string.IsNullOrEmpty(NormalLiquidLevel) && !string.IsNullOrEmpty(_Length))
                {
                    NormalLiquidLevel = (double.Parse(Length) / 2).ToString();
                }
                OnPropertyChanged("Length");
            }
        }
        private string _NormalLiquidLevel;
        public string NormalLiquidLevel
        {
            get
            {
                return this._NormalLiquidLevel;
            }
            set
            {
                this._NormalLiquidLevel = value;
                OnPropertyChanged("NormalLiquidLevel");
            }
        }



        public List<string> AccumulatorTypes { get; set; }
        public Accumulator CurrentAccumulator { get; set; }
        public List<string> GetAccumulatorTypes()
        {
            List<string> list = new List<string>();
            list.Add("Pressure Driven");
            list.Add("Pump(Motor)");
            list.Add("Pump(Turbine)");
            return list;
        }
        public AccumulatorVM(string name, string dbPSFile, string dbPFile)
        {
            AccumulatorTypes = GetAccumulatorTypes();
            dbProtectedSystemFile = dbPSFile;
            dbPlantFile = dbPFile;
            BasicUnit BU;
            using (var helper = new NHibernateHelper(dbPlantFile))
            {
                var Session = helper.GetCurrentSession();
                dbBasicUnit dbBU = new dbBasicUnit();
                IList<BasicUnit> list = dbBU.GetAllList(Session);
                BU = list.Where(s => s.IsDefault == 1).Single();
            }
            using (var helper = new NHibernateHelper(dbProtectedSystemFile))
            {
                UnitConvert uc = new UnitConvert();
                var Session = helper.GetCurrentSession();
                dbAccumulator db = new dbAccumulator();
                CurrentAccumulator = db.GetModel(Session);
                Diameter = CurrentAccumulator.Diameter;
                Length = CurrentAccumulator.Length;
                NormalLiquidLevel = CurrentAccumulator.NormalLiquidLevel;
                if (CurrentAccumulator.Orientation)
                {
                    Horiz = true;
                    Vertical = false;
                }
                else
                {
                    Horiz = false;
                    Vertical = true;
                }

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
        
        private void Save(object window)
        {
            CurrentAccumulator.AccumulatorName=CurrentAccumulator.AccumulatorName.Trim();
            if (CurrentAccumulator.AccumulatorName == "")
            {
                throw new ArgumentException("Please type in a name for the Accumulator.");
            }
            BasicUnit BU;
            using (var helper = new NHibernateHelper(dbPlantFile))
            {
                var Session = helper.GetCurrentSession();
                dbBasicUnit dbBU = new dbBasicUnit();
                IList<BasicUnit> list = dbBU.GetAllList(Session);
                BU = list.Where(s => s.IsDefault == 1).Single();
            }
            using (var helper = new NHibernateHelper(dbProtectedSystemFile))
            {
                var Session = helper.GetCurrentSession();
                dbAccumulator db = new dbAccumulator();
                Accumulator m = db.GetModel(Session);

                m.AccumulatorName = CurrentAccumulator.AccumulatorName;
                m.Diameter = Diameter;
                m.Length = Length;
                m.NormalLiquidLevel = NormalLiquidLevel;
                if (Horiz)
                    m.Orientation = true;
                else
                    m.Orientation = false;
                db.Update(m, Session);
                Session.Flush();

            }
            System.Windows.Window wd = window as System.Windows.Window;

            if (wd != null)
            {
                wd.Close();
            }
        }

        
    }
}

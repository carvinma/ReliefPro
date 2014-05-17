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
using UOMLib;

namespace ReliefProMain.ViewModel
{
    public class PSVVM : ViewModelBase
    {
        public string dbProtectedSystemFile { get; set; }
        public string dbPlantFile { get; set; }
        public bool Horiz { get; set; }
        public bool Vertical { get; set; }


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
        public PSVVM(string name, string dbPSFile, string dbPFile)
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
                if (CurrentAccumulator.Orientation)
                {
                    Horiz = false;
                    Vertical = true;
                }
                else
                {
                    Horiz = true;
                    Vertical = false;
                }

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
            CurrentAccumulator.AccumulatorName = CurrentAccumulator.AccumulatorName.Trim();
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
                Accumulator ac = db.GetModel(Session);

                ac.AccumulatorName = CurrentAccumulator.AccumulatorName;
                ac.Diameter = CurrentAccumulator.Diameter;
                ac.Length = CurrentAccumulator.Length;
                ac.NormalLiquidLevel = CurrentAccumulator.NormalLiquidLevel;
                ac.Orientation = CurrentAccumulator.Orientation;
                db.Update(ac, Session);
                Session.Flush();

            }
            System.Windows.Window wd = window as System.Windows.Window;

            if (wd != null)
            {
                wd.Close();
            }
        }

        //public Action CloseAction { get; set; }
    }
}

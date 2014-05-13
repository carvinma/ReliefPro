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

namespace ReliefProMain.ViewModel
{
    public class ReboilerPinchVM : ViewModelBase
    {
        public string dbProtectedSystemFile { get; set; }
        public string dbPlantFile { get; set; }

        private ObservableCollection<string> _SourceTypes;
        public ObservableCollection<string> SourceTypes
        {
            get { return _SourceTypes; }
            set
            {
                _SourceTypes = value;
                OnPropertyChanged("SourceTypes");
            }
        }

        private string _SelectedSourceType;
        public string SelectedSourceType
        {
            get { return _SelectedSourceType; }
            set
            {
                _SelectedSourceType = value;
                OnPropertyChanged("SelectedSourceType");
            }
        }

        public ReboilerPinch CurrentReboilerPinch { get; set; }
        public ObservableCollection<string> GetSourceTypes()
        {
            ObservableCollection<string> list = new ObservableCollection<string>();
            list.Add("Hot Oil/water");          
            list.Add("Steam");
            return list;
        }
        public ReboilerPinchVM(string name, string dbPSFile, string dbPFile)
        {
            SourceTypes = GetSourceTypes();
            dbProtectedSystemFile = dbPSFile;
            dbPlantFile = dbPFile;
            BasicUnit BU;
            using (var helper = new NHibernateHelper(dbPlantFile))
            {
                var Session = helper.GetCurrentSession();
                dbBasicUnit dbBU = new dbBasicUnit();
                IList<BasicUnit> list=dbBU.GetAllList(Session);
                BU = list.Where(s=>s.IsDefault==1).Single();
            }



            using (var helper = new NHibernateHelper(dbProtectedSystemFile))
            {
                UnitConvert uc=new UnitConvert();
                var Session = helper.GetCurrentSession();
               
            }
            
        }

        private void HeatMediumMethod(double productTin, double productTout, double reliefProductTout, double reboilerTin, double reboilerTout, double coeff, double area, double duty, bool isUseSteam, double qaenGuess, double MaxErrorRate, ref double factor, ref bool isPinch, ref int iterateNumber)
        {
            int iterateSum = 0;
            double nextQaenGuess = qaenGuess;
            double curErrorRate = 1;
            double reliefProductTin = 0;
            double reliefReboilerTin = reboilerTin;
            double reliefReboilerTout = 0;
            double ltmd = 0;
            double t = 0;
            double relieft = 0;
            double reliefLtmd = 0;
            double reliefCoeff = coeff;
            double reliefArea = area;
            double reliefDuty = 0;
            double assumedQA = 0;
            double calculatedQR = 0;
            double QRQA = 1;
            double lnValue = 1;
            double uQAQR = 0;
            while (curErrorRate > MaxErrorRate && iterateSum < iterateNumber)
            {
                reliefProductTin = reliefProductTout - nextQaenGuess * (productTout - productTin);
                reliefReboilerTin = reboilerTin;
                reliefReboilerTout = reliefReboilerTin - nextQaenGuess * (reboilerTin - reboilerTout);
                t = reboilerTout + productTout - reboilerTin - productTin;
                lnValue = (reboilerTout - productTin) / (reboilerTin - productTout);
                ltmd = t / Math.Log(lnValue);

                relieft = reliefReboilerTout + reliefProductTout - reliefProductTin - reliefReboilerTin;
                lnValue = (reliefReboilerTout - reliefProductTin) / (reliefReboilerTin - reliefProductTout);
                reliefLtmd = relieft / Math.Log(lnValue);

                reliefCoeff = coeff;
                reliefArea = area;
                reliefDuty = reliefLtmd * reliefCoeff * reliefArea;
                assumedQA = duty * nextQaenGuess;
                calculatedQR = reliefDuty;
                QRQA = calculatedQR / assumedQA;
                uQAQR = assumedQA / calculatedQR - 1;
                curErrorRate = Math.Abs(uQAQR);
                iterateSum = iterateSum + 1;
                if (curErrorRate > MaxErrorRate)
                {
                    nextQaenGuess = QRQA * (1 + 0.5 * uQAQR);
                }

            }
            if (QRQA < 1)
                isPinch = true;
            factor = QRQA;


        }

    }
}

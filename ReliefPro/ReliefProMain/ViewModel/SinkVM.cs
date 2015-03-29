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
using NHibernate;
using ReliefProMain.View;
using ReliefProMain.Models;
using System.Windows;
using ReliefProBLL;

namespace ReliefProMain.ViewModel
{
    public class SinkVM : ViewModelBase
    {
        public SinkModel model { get; set; }
        public int deviceId;
        UOMLib.UOMEnum uomEnum;
        aSinkBLL sinkBLL;
        aScenarioBLL scenarioBLL;
        public SinkVM(string streamName,int deviceId)
        {
            this.deviceId = deviceId;
            uomEnum = UOMSingle.plantsInfo.FirstOrDefault(p => p.Id == 0).UnitInfo;
            sinkBLL = new aSinkBLL();
            scenarioBLL = new aScenarioBLL();
            tbSink sink = sinkBLL.GetModel(streamName, deviceId);
            model = new SinkModel(sink);
            InitUnit();
            ReadConvert();
        }

        private ICommand _SaveCommand;

        public ICommand SaveCommand
        {
            get { return _SaveCommand ?? (_SaveCommand = new RelayCommand(Save)); }
        }


        private void Save(object window)
        {
            if (model.SinkName.Trim() == "")
            {
                throw new ArgumentException("Please input a name for the Sink.");
            }
            
            bool bEdit = false;
            if (model.dbmodel.Maxpossiblepressure != UnitConvert.Convert(model.PressureUnit, UOMEnum.Pressure, model.MaxPossiblePressure) || model.dbmodel.Sinktype != model.SinkType)
            {
                bEdit = true;
            }
            System.Windows.Window wd = window as System.Windows.Window;
            if (bEdit)
            {
                List<tbScenario> scList = scenarioBLL.GetList(deviceId);
                if (scList.Count > 0)
                {
                    MessageBoxResult r = MessageBox.Show("Are you sure to edit data? it need to rerun all Scenario", "Message Box", MessageBoxButton.YesNo);
                    if (r == MessageBoxResult.Yes)
                    {
                        scenarioBLL.DeleteSCOther();
                        scenarioBLL.ClearScenario();
                    }
                    else
                    {
                        if (wd != null)
                        {
                            wd.Close();
                            return;
                        }
                    }
                }
                WriteConvert();
                model.dbmodel.SinktypeColor = model.SinkType_Color;
                model.dbmodel.MaxpossiblepressureColor = model.MaxPossiblePressure_Color;
                model.dbmodel.Sinktype = model.SinkType;
                model.dbmodel.Description = model.Description;
                sinkBLL.SaveSink(model.dbmodel);
            }
            

            if (wd != null)
            {
                wd.Close();
            }
        }

        private void ReadConvert()
        {
            MainModel.MaxPossiblePressure = UnitConvert.Convert(UOMEnum.Pressure, MainModel.PressureUnit, MainModel.MaxPossiblePressure);
        }
        private void WriteConvert()
        {
            MainModel.dbmodel.Maxpossiblepressure = UnitConvert.Convert(MainModel.PressureUnit, UOMEnum.Pressure, MainModel.MaxPossiblePressure);
        }
        private void InitUnit()
        {
            MainModel.PressureUnit = uomEnum.UserPressure;
        }

    }
}

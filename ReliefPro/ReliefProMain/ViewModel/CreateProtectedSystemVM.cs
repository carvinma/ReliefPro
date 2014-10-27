using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ReliefProMain.Commands;
using System.IO;
using NHibernate;
using ReliefProDAL;
using ReliefProModel;
using System.Windows;
using ReliefProMain.Models;

namespace ReliefProMain.ViewModel
{
    public class CreateProtectedSystemVM : ViewModelBase
    {
        public ISession SessionPlant { set; get; }
        public string dirUnit { get; set; }

        public string dirProtectedSystem{ get; set; }
        public string visioProtectedSystem { get; set; }
        public string dbProtectedSystemFile { get; set; }
        public TVPS tvPS { get; set; } 
        private int UnitID;
        private string _ProtectedSystemName;
        
        public string ProtectedSystemName
        {
            get
            {
                return this._ProtectedSystemName;
            }
            set
            {
                this._ProtectedSystemName = value.Trim();
                OnPropertyChanged("ProtectedSystemName");
            }
        }

        private string _ProtectedSystemName_Color;
        public string ProtectedSystemName_Color
        {
            get
            {
                return this._ProtectedSystemName_Color;
            }
            set
            {
                this._ProtectedSystemName_Color = value;
                OnPropertyChanged("ProtectedSystemName_Color");
            }
        }

        public CreateProtectedSystemVM(ISession SessionPlant,int UnitID, string dir)
        {
            this.SessionPlant = SessionPlant;
            this.UnitID = UnitID;
            dirUnit = dir;
            ProtectedSystemName = "ProtectedSystem1";
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
            if (!CheckData()) return;
            if (string.IsNullOrEmpty(ProtectedSystemName))
            {
                MessageBox.Show("Unit Name could not be empty!", "Message Box");
                return;
            }
            TreePSDAL tpsdal = new TreePSDAL();
            TreePS tps = tpsdal.GetModel(SessionPlant, UnitID, ProtectedSystemName);
            if (tps != null)
            {
                MessageBox.Show("this Protected System is exist!", "Message Box");
                return;
            }
            dirProtectedSystem = dirUnit + @"\" + ProtectedSystemName;
            try
            {

                if (Directory.Exists(dirProtectedSystem))
                {
                    Directory.Delete(dirProtectedSystem, true);
                }
                Directory.CreateDirectory(dirProtectedSystem);
            }
            catch (Exception ex)
            {
            }
            TreePSDAL treePSDAL = new TreePSDAL();
            TreePS treePS = new TreePS();
            treePS.PSName = ProtectedSystemName;
            treePS.UnitID = UnitID;
            treePSDAL.Add(treePS, SessionPlant);

            
            string protectedsystem1 = dirProtectedSystem;
            string dbProtectedSystem = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"template\protectedsystem.mdb";
            string dbProtectedSystem_target = protectedsystem1 + @"\protectedsystem.mdb";
            System.IO.File.Copy(dbProtectedSystem, dbProtectedSystem_target, true);
            visioProtectedSystem = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"template\protectedsystem.vsd";
            string visioProtectedSystem_target = protectedsystem1 + @"\design.vsd";
            System.IO.File.Copy(visioProtectedSystem, visioProtectedSystem_target, true);

            dbProtectedSystemFile = dbProtectedSystem_target;
            dirProtectedSystem = protectedsystem1;
            visioProtectedSystem = visioProtectedSystem_target;

            tvPS = new TVPS();
            tvPS.dbProtectedSystemFile = dbProtectedSystem_target;
            tvPS.FullPath = dirProtectedSystem;
            tvPS.Name = ProtectedSystemName;


            System.Windows.Window wd = window as System.Windows.Window;
            if (wd != null)
            {
                wd.DialogResult = true;
            }
        }

    }
}

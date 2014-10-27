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
using ReliefProMain.View;

namespace ReliefProMain.ViewModel
{
    public class CreateUnitVM : ViewModelBase
    {
        public ISession SessionPlant { set; get; }
        public string dirPlant { get; set; }
        public string dirUnit { get; set; }

        public string dirProtectedSystem { get; set; }
        public string visioProtectedSystem { get; set; }
        public string dbProtectedSystemFile { get; set; }
        public TVUnit tvUnit { get; set; } 

        private string _UnitName;
        
        public string UnitName
        {
            get
            {
                return this._UnitName;
            }
            set
            {
                this._UnitName = value;

                OnPropertyChanged("UnitName");
            }
        }

        private string _UnitName_Color;
        public string UnitName_Color
        {
            get
            {
                return this._UnitName_Color;
            }
            set
            {
                this._UnitName_Color = value;
                OnPropertyChanged("UnitName");
            }
        }

        public CreateUnitVM(ISession SessionPlant,string dir )
        {
            UnitName = "Unit1";
            this.SessionPlant = SessionPlant;
            dirPlant = dir;
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
            if (string.IsNullOrEmpty(UnitName))
            {
                MessageBox.Show("Unit Name could not be empty!", "Message Box");
                return;
            }
            string dirUnit = dirPlant + @"\" + UnitName;
            try
            {
                if (Directory.Exists(dirUnit))
                {
                    Directory.Delete(dirUnit, true);
                }
                Directory.CreateDirectory(dirUnit);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Current plant folder is opened,close it and try again.or Unit Name is not legal.", "Message Box");
                return;
            }

            TreeUnitDAL tudal = new TreeUnitDAL();
            TreeUnit tu = tudal.GetModel(SessionPlant, UnitName);
            if (tu != null)
            {
                MessageBox.Show("this Unit is exist!", "Message Box");
                return;
            }

            TreeUnitDAL treeUnitDAL = new TreeUnitDAL();
            TreeUnit treeUnit = new TreeUnit();
            treeUnit.UnitName = UnitName;
            treeUnitDAL.Add(treeUnit, SessionPlant);

            //TreePSDAL treePSDAL = new TreePSDAL();
            //TreePS treePS = new TreePS();
            //treePS.PSName = "ProtectedSystem1";
            //treePS.UnitID = treeUnit.ID;
            //treePSDAL.Add(treePS, SessionPlant);

            
            tvUnit = new TVUnit();
            tvUnit.ID = treeUnit.ID;
            tvUnit.Name = UnitName;
            tvUnit.FullPath = dirUnit;

            CreateProtectedSystemView psView = new CreateProtectedSystemView();
            CreateProtectedSystemVM psVM = new CreateProtectedSystemVM(SessionPlant,tvUnit.ID,dirUnit);
            psView.DataContext = psVM;
            psView.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            
            //string protectedsystem1 = dirUnit + @"\ProtectedSystem1";
            //Directory.CreateDirectory(protectedsystem1);
            //string dbProtectedSystem = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"template\protectedsystem.mdb";
            //string dbProtectedSystem_target = protectedsystem1 + @"\protectedsystem.mdb";
            //System.IO.File.Copy(dbProtectedSystem, dbProtectedSystem_target, true);
            //string visioProtectedSystem = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"template\protectedsystem.vsd";
            //string visioProtectedSystem_target = protectedsystem1 + @"\design.vsd";
            //System.IO.File.Copy(visioProtectedSystem, visioProtectedSystem_target, true);
            //dirProtectedSystem = protectedsystem1;
            //visioProtectedSystem = visioProtectedSystem_target;
            //dbProtectedSystemFile = dbProtectedSystem_target;

           
            
            
            System.Windows.Window wd = window as System.Windows.Window;
            if (wd != null)
            {
                wd.DialogResult = true;
            }
        }

    }
}

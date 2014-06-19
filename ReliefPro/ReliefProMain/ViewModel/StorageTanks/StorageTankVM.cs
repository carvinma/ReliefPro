﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;
using ReliefProModel;
using ReliefProMain.Commands;
using ReliefProDAL;
using ReliefProBLL.Common;
using ReliefProMain.Interface;
using ReliefProMain.Service;
using UOMLib;
using NHibernate;
using ReliefProMain.View;
using ReliefProMain.Model;

namespace ReliefProMain.ViewModel.StorageTanks
{
   public class StorageTankVM:ViewModelBase
    {
       public ICommand OKCMD { get; set; }
       private ISession SessionPlant { set; get; }
        private ISession SessionProtectedSystem { set; get; }
        
        private string DirPlant { set; get; }
        private string DirProtectedSystem { set; get; }
        private CustomStreamModel _CurrentModel;
        public CustomStreamModel CurrentModel
        {
            get
            {
                return this._CurrentModel;
            }
            set
            {
                this._CurrentModel = value;
                OnPropertyChanged("CurrentModel");
            }
        }
        public string przFile { set; get; }
       private CustomStreamDAL db;
       public StorageTankVM(string name, ISession sessionPlant, ISession sessionProtectedSystem, string dirPlant, string dirProtectedSystem)
       {
           SessionPlant = sessionPlant;
           SessionProtectedSystem = sessionProtectedSystem;
           DirPlant = dirPlant;
           DirProtectedSystem = dirProtectedSystem;
           BasicUnit BU;
           BasicUnitDAL dbBU = new BasicUnitDAL();
           IList<BasicUnit> list = dbBU.GetAllList(sessionPlant);
           BU = list.Where(s => s.IsDefault == 1).Single();

           UnitConvert uc = new UnitConvert();
           db = new CustomStreamDAL();
           if (name != string.Empty)
           {
               CustomStream cs = db.GetModel(SessionProtectedSystem, name);
               CurrentModel = new CustomStreamModel(cs);
           }
           OKCMD = new DelegateCommand<object>(Save);
       }

       private ICommand _ImportCommand;
       public ICommand ImportCommand
       {
           get
           {
               if (_ImportCommand == null)
               {
                   _ImportCommand = new RelayCommand(Import);

               }
               return _ImportCommand;
           }
       }
       private void Import(object obj)
       {
           SelectStreamView v = new SelectStreamView();
           SelectStreamVM vm = new SelectStreamVM( SessionPlant, DirPlant);
           v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
           v.DataContext = vm;
           if (v.ShowDialog() == true)
           {
               if (!string.IsNullOrEmpty(vm.SelectedEq))
               {
                   //根据设该设备名称来获取对应的物流线信息和其他信息。
                   ProIIStreamDataDAL proIIStreamDataDAL = new ProIIStreamDataDAL();
                   przFile = vm.SelectedFile + ".prz";
                   ProIIStreamData data = proIIStreamDataDAL.GetModel(SessionPlant, vm.SelectedEq, przFile);
                   CustomStream cs = ProIIToDefault.ConvertProIIStreamToCustomStream(data);
                   CurrentModel = new CustomStreamModel(cs);

               }
           }
       }

       private void Save(object obj)
       {
           if (obj != null)
           {
               if (CurrentModel.ID == 0)
               {
                   db.Add(CurrentModel.model, SessionProtectedSystem);
                   StorageTankDAL storageTankDAL = new StorageTankDAL();
                   StorageTank tank= new StorageTank();
                   tank.StorageTankName = CurrentModel.model.StreamName;
                   tank.PrzFile = przFile;
                   storageTankDAL.Add(tank, SessionProtectedSystem);
               }
               else
               {
                   db.Update(CurrentModel.model, SessionProtectedSystem);
                   StorageTankDAL storageTankDAL = new StorageTankDAL();
                   StorageTank tank = storageTankDAL.GetModel(SessionProtectedSystem);
                   tank.StorageTankName = CurrentModel.model.StreamName;
                   tank.PrzFile = przFile;
                   storageTankDAL.Update(tank, SessionProtectedSystem);
                   SessionProtectedSystem.Flush();
               }


               System.Windows.Window wd = obj as System.Windows.Window;
               if (wd != null)
               {
                   
                   wd.DialogResult = true;
               }
           }
       }

    }
}

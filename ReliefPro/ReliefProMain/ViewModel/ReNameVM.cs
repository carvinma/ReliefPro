﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ReliefProMain.Commands;
using System.IO;
using System.Windows;

namespace ReliefProMain.ViewModel
{
    public class ReNameVM : ViewModelBase
    {
       
        public string OldDir { get; set; } 
        public int Type{ get; set; }
        public string OldName { get; set; }
        public string NewName { get; set; }
        public string NewDir { get; set; } 
        private string _Name;
        public string Name
        {
            get
            {
                return this._Name;
            }
            set
            {
                this._Name = value;

                OnPropertyChanged("Name");
            }
        }

        public ReNameVM(string name, string dir,int type)
        {
            Name = name;
            OldDir = dir;
            Type = type;
            OldName = name;
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
            if (OldName.ToLower().Trim() == Name.ToLower().Trim())
            {
                MessageBox.Show("Please give a different name!", "Message Box");
                return;
            }
            else
            {
                string dirParent = System.IO.Path.GetDirectoryName(OldDir);
                NewDir = dirParent + @"\" + Name;
                bool isExist = Directory.Exists(NewDir);
                if (isExist)
                {
                    MessageBox.Show("Please give a different name,it contains a same name!", "Message Box");
                    return;
                }

                Directory.Move(OldDir, NewDir);
                System.Windows.Window wd = window as System.Windows.Window;

                if (wd != null)
                {
                    wd.DialogResult = true;
                }
            }
            
            
            
        }

    }
}

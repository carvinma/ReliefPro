using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using System.Windows.Input;
using ReliefProModel;
using ReliefProMain.Commands;
using ReliefProDAL;
using ReliefProBLL.Common;
using System.Collections.ObjectModel;
using NHibernate;
using ReliefProBLL;

namespace ReliefProMain.ViewModel
{
   public class SelectStreamVM:ViewModelBase
    {
        private ISession SessionPlant { set; get; }
        public SourceFile SourceFileInfo { set; get; }      
        private string _SelectedFile;
        public string SelectedFile
        {
            get
            {
                return this._SelectedFile;
            }
            set
            {
                this._SelectedFile = value;
                SourceFileBLL sfbll = new SourceFileBLL(SessionPlant);
                SourceFileInfo = sfbll.GetSourceFileInfo(_SelectedFile);
                EqNames = GetEqNames();
                OnPropertyChanged("SelectedFile");
            }
        }
        private string _SelectedEq;
        public string SelectedEq
        {
            get
            {
                return this._SelectedEq;
            }
            set
            {
                this._SelectedEq = value;
                OnPropertyChanged("SelectedEq");
            }
        }



        public SelectStreamVM( ISession sessionPlant)
        {
            SessionPlant = sessionPlant;
            SourceFiles = GetSourceFiles();
            EqNames = GetEqNames();
        }
        private ObservableCollection<string> _SourceFiles;
        public ObservableCollection<string> SourceFiles
        {
            get { return _SourceFiles; }
            set
            {
                _SourceFiles = value;
                OnPropertyChanged("SourceFiles");
            }
        }
        private ObservableCollection<string> _EqNames;
        public ObservableCollection<string> EqNames
        {
            get { return _EqNames; }
            set
            {
                _EqNames = value;
                OnPropertyChanged("EqNames");
            }
        }

        public ObservableCollection<string> GetEqNames()
        {
            ObservableCollection<string> list = new ObservableCollection<string>();
            ProIIStreamDataDAL db = new ProIIStreamDataDAL();
            if (!string.IsNullOrEmpty(SelectedFile))
            {
                int idx = SelectedFile.LastIndexOf(".");
                string ext = SelectedFile.Substring(idx + 1);
                if (ext.ToLower() == "prz")
                {
                    IList<ProIIStreamData> data = db.GetAllList(SessionPlant, SelectedFile);
                    foreach (ProIIStreamData d in data)
                    {
                        list.Add(d.StreamName);
                    }
                }
            }
            return list;
        }

        public ObservableCollection<string> GetSourceFiles()
        {
            ObservableCollection<string> list = new ObservableCollection<string>();
            SourceFileDAL dal = new SourceFileDAL();
            IList<SourceFile> files = dal.GetAllList(SessionPlant);
            foreach (SourceFile df in files)
            {
                list.Add(df.FileName);
            }

            return list;
        }

       private ICommand _OKCommand;
        public ICommand OKCommand
        {
            get
            {
                if (_OKCommand == null)
                {
                    _OKCommand = new RelayCommand(OK);

                }
                return _OKCommand;
            }
        }

        public void OK(object obj)
        {
            if(string.IsNullOrEmpty(SelectedEq))
            {
                return;
            }
            System.Windows.Window wd = obj as System.Windows.Window;

            if (wd != null)
            {
                wd.DialogResult = true;
            }
        }
    }
}

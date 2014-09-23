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
   public class SelectPathVM:ViewModelBase
    {   
        public ISession SessionPlant;
        public SourceFile SourceFileInfo;
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
                OnPropertyChanged("SelectedFile");
            }
        }


        public SelectPathVM( ISession sessionPlant)
        {
            
            SessionPlant = sessionPlant;
            SourceFiles = GetSourceFiles();
           
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

        public ObservableCollection<string> GetSourceFiles()
        {
            ObservableCollection<string> list = new ObservableCollection<string>();
            SourceFileDAL dal = new SourceFileDAL();
            IList<SourceFile> files = dal.GetAllList(SessionPlant);
            foreach (SourceFile df in files)
            {
                list.Add(df.FileName);
            }
            if (list.Count > 0)
            {
                SelectedFile = list[0];
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
            if(string.IsNullOrEmpty(SelectedFile))
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

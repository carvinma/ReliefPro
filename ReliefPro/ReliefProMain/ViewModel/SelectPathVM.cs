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

namespace ReliefProMain.ViewModel
{
   public class SelectPathVM:ViewModelBase
    {   
        private string DirPlant { set; get; }
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

                OnPropertyChanged("SelectedFile");
            }
        }


        public SelectPathVM( string dirPlant)
        {
            DirPlant = dirPlant;           
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
            string[] files = Directory.GetFiles(DirPlant);
            foreach (string f in files)
            {
                FileInfo fi = new FileInfo(f);
                if (fi.Extension.ToString().ToLower() == ".prz")
                {
                    string filename=System.IO.Path.GetFileNameWithoutExtension(f);
                    list.Add(filename);
                }
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

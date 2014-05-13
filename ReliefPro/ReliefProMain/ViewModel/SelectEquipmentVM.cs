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

namespace ReliefProMain.ViewModel
{
   public class SelectEquipmentVM:ViewModelBase
    {
       private string dbProtectedSystemFile;
        private string dbPlantFile;
        private string EqType;
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

        public SelectEquipmentVM(string eqType, string dbPSFile, string dbPFile)
        {
            dbProtectedSystemFile = dbPSFile;
            dbPlantFile = dbPFile;
            EqType = eqType;
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
            using (var helper = new NHibernateHelper(dbPlantFile))
            {
                var Session=helper.GetCurrentSession();
                dbProIIEqData db = new dbProIIEqData();
                string file = SelectedFile + ".prz";
                IList<ProIIEqData> data = db.GetAllList(Session,file,EqType);
                foreach (ProIIEqData d in data)
                {
                    list.Add(d.EqName);
                }

            }
            return list;
        }

        public ObservableCollection<string> GetSourceFiles()
        {
            ObservableCollection<string> list = new ObservableCollection<string>();
            string strDir = System.IO.Path.GetDirectoryName(dbPlantFile);

            string[] files = Directory.GetFiles(strDir);
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

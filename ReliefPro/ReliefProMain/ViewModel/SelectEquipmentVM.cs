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
using ReliefProCommon.Enum;

namespace ReliefProMain.ViewModel
{
   public class SelectEquipmentVM:ViewModelBase
    {
        private ISession SessionPlant { set; get; }       
        private string EqType;
        public SourceFile SourceFileInfo;

        private string _SelectedFile;
        [ReliefProMain.Util.Required(ErrorMessage = "NotEmpty")]
        public string SelectedFile
        {
            get
            {
                return this._SelectedFile;
            }
            set
            {
                this._SelectedFile = value;
                if (string.IsNullOrEmpty(_SelectedFile))
                {
                    SelectedFile_Color = ColorBorder.red.ToString();
                }
                SourceFileBLL sfbll = new SourceFileBLL(SessionPlant);
                SourceFileInfo = sfbll.GetSourceFileInfo(_SelectedFile);
                EqNames = GetEqNames();
                OnPropertyChanged("SelectedFile");
            }
        }

        private string _SelectedFile_Color;
        public string SelectedFile_Color
        {
            get
            {
                return this._SelectedFile_Color;
            }
            set
            {
                this._SelectedFile_Color = value;
                OnPropertyChanged("SelectedFile_Color");
            }
        }
        
        private string _SelectedEq;
        [ReliefProMain.Util.Required(ErrorMessage = "NotEmpty")]
        public string SelectedEq
        {
            get
            {
                return this._SelectedEq;
            }
            set
            {
                this._SelectedEq = value;
                if (string.IsNullOrEmpty(_SelectedEq))
                {
                    SelectedEq_Color = ColorBorder.red.ToString();
                }
                OnPropertyChanged("SelectedEq");
            }
        }
        private string _SelectedEq_Color;
        public string SelectedEq_Color
        {
            get
            {
                return this._SelectedEq_Color;
            }
            set
            {
                this._SelectedEq_Color = value;
                OnPropertyChanged("SelectedEq_Color");
            }
        }

        public SelectEquipmentVM(string eqType, ISession sessionPlant)
        {
            SelectedFile_Color = ColorBorder.green.ToString();
            SelectedEq_Color = ColorBorder.green.ToString();
            SessionPlant = sessionPlant;
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
            ProIIEqDataDAL db = new ProIIEqDataDAL();
            if (!string.IsNullOrEmpty(SelectedFile))
            {
                int idx = SelectedFile.LastIndexOf(".");
                string ext = SelectedFile.Substring(idx + 1);
                if (ext.ToLower() == "prz")
                {
                    IList<ProIIEqData> data = db.GetAllList(SessionPlant, SelectedFile, EqType);
                    foreach (ProIIEqData d in data)
                    {
                        list.Add(d.EqName);
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
            if (!CheckData()) return;
            System.Windows.Window wd = obj as System.Windows.Window;

            if (wd != null)
            {
                wd.DialogResult = true;
            }
        }
    }
}

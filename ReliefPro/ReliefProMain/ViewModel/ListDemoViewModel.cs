
using System;
using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using ReliefProMain.Model;
using ReliefProModel;
namespace ReliefProMain.ViewModel
{
    public class ListDemoViewModel : ViewModelBase, INotifyPropertyChanged
    {
        /// <summary>
        /// 增加的命令
        /// </summary>
        private ICommand _Add;
        public ICommand AddCommand { get { return _Add; } set { _Add = value; } }

        public ListDemoViewModel()
        {
            ListDemoCollection = new ListDemoModel();
           // AddInfo(null);
            AddCommand = new DelegateCommand<object>(AddInfo);
        }
        /// <summary>
        /// 合并警情的具体实现
        /// </summary>
        public void AddInfo(object obj)
        {
            Random rand = new Random();
            DemoModel model = new DemoModel
            {
                ID = rand.Next(1, 1000),
                col1 = rand.Next(2000, 3000),
                colDate = DateTime.Today,
                colString = "测试工工式"
            };
            ListDemoCollection.Add(model);
        }
        #region 数据源List

        private ListDemoModel listDemoCollection;
        public ListDemoModel ListDemoCollection
        {
            get { return this.listDemoCollection; }
            set
            {
                this.listDemoCollection = value;
                NotifyPropertyChanged("ListDemoCollection");
            }
        }
        #endregion

        #region 通知
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}

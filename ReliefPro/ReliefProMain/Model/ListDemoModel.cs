
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ReliefProModel;
namespace ReliefProMain.Model
{
    public class ListDemoModel : ObservableCollection<DemoModel>
    {
        public ListDemoModel() { }
        public ListDemoModel(IList<DemoModel> list)
            : base(list)
        {

        }
    }
}

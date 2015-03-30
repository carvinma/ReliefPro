using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AxMicrosoft.Office.Interop.VisOcx;
using System.Collections.ObjectModel;
using ReliefProModel;

namespace CustomVisio
{
    public interface IVisio
    {
        string[] GetDoubleShapeName(AxDrawingControl visioControl);
        void DrawTank(string shapeName);
        void DrawTower(string shapeText, int StageNumber, ObservableCollection<CustomStream> Feeds, ObservableCollection<CustomStream> Products, ObservableCollection<TowerHX> Condensers, ObservableCollection<TowerHX> HxCondensers, ObservableCollection<TowerHX> Reboilers, ObservableCollection<TowerHX> HxReboilers);
        void DrawDrum(string shapeText, ObservableCollection<CustomStream> Feeds, ObservableCollection<CustomStream> Products);
        void DrawHX(string shapeText, string hxType, ObservableCollection<CustomStream> Feeds, ObservableCollection<CustomStream> Products, string TubeFeedStreams, string ShellFeedStreams, string ColdInlet, string ColdOutlet, string HotInlet, string HotOutlet);
        void DrawCompressor(string shapeText, ObservableCollection<CustomStream> Feeds, ObservableCollection<CustomStream> Products);
        void DrawReactor(string shapeText);
    }
}

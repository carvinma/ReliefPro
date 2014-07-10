using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ReliefProMain.View.Reports
{
    /// <summary>
    /// PlantSummaryView.xaml 的交互逻辑
    /// </summary>
    public partial class PlantSummaryView : Window
    {

        public PlantSummaryView()
        {
            this.VisualEdgeMode = EdgeMode.Aliased;
            InitializeComponent();
            double ddd = 100.40000;
            string ss= ddd.ToString("N");

            double d = 1.00d;
            string s = d.ToString("N2"); 
            var host = new DrawingTest();

            this.grd.Children.Add(host);
        }
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            dc.DrawRectangle(Brushes.LightBlue, new Pen(Brushes.Red, 1),
                new Rect(new Point(10, 10), new Size(100, 50)));

            dc.DrawText(new FormattedText("my canvas", CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight, new Typeface("Tahoma"), 20, Brushes.Green),
                new Point(50, 25));
        }

    }
    class MyCanvas : Canvas
    {
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            dc.DrawRectangle(Brushes.LightBlue, new Pen(Brushes.Red, 1),
                new Rect(new Point(10, 10), new Size(100, 50)));

            dc.DrawText(new FormattedText("my canvas", CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight, new Typeface("Tahoma"), 20, Brushes.Green),
                new Point(50, 25));
        }
    }
    public class DrawingHost : FrameworkElement
    {
        private DrawingVisual _drawingVisual = new DrawingVisual();

        public DrawingHost()
        {
            // 必须加入到VisualTree中才能显示
            this.AddVisualChild(_drawingVisual);
            this.Draw();
        }

        // 重载自己的VisualTree的孩子的个数，由于只有一个DrawingVisual，返回1
        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        // 重载当WPF框架向自己要孩子的时候，返回返回DrawingVisual
        protected override Visual GetVisualChild(int index)
        {
            if (index == 0)
                return _drawingVisual;

            throw new IndexOutOfRangeException();
        }

        // 绘制代码
        private void Draw()
        {
            var dc = _drawingVisual.RenderOpen();
            dc.DrawEllipse(Brushes.Blue, null, new Point(50, 50), 50, 50);
            dc.Close();
        }
    }

    public class DrawingTest : FrameworkElement
    {
        DrawingVisual dv = new DrawingVisual();
        DataTable dt = new DataTable();
        public DrawingTest()
        {
            dt.Columns.Add("A", typeof(int));
            dt.Columns.Add("B", typeof(DateTime));
            for (int i = 0; i < 60; i++)
            {
                DataRow dr = dt.NewRow();
                dr[0] = i;
                dr[1] = DateTime.Now.AddDays(-14).AddDays(i % 31);
                dt.Rows.Add(dr);
            }
            dt.AcceptChanges();
            this.AddVisualChild(dv);
            this.Draw();
        }
        private void Draw()
        {
            Pen blackp = new Pen(Brushes.Black, 0.2);
            Brush borderBrush = Brushes.LightBlue;
            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("zh-cn");
            FlowDirection fd = FlowDirection.LeftToRight;
            Typeface tf = new Typeface("宋体");
            double d = 12;
            using (DrawingContext dc = dv.RenderOpen())
            {
                for (int i = 0; i < 5; i++)
                {
                    dc.DrawRectangle(borderBrush, blackp, new Rect(217 * i, 0, 217, 30));
                    dc.DrawText(new FormattedText("第" + (i + 1).ToString() + "周", ci, fd, tf, d, Brushes.Green), new Point(217 * i + 70, 10));
                }
                for (int i = 0; i < 35; i++)
                    dc.DrawRectangle(null, blackp, new Rect(31 * i, 30, 31, 30));
                int w = new DateTime(2011, 3, 1).DayOfWeek.GetHashCode();
                for (int i = w; i < w + 31; i++)
                    dc.DrawText(new FormattedText((i - w + 1).ToString(), ci, fd, tf, d, Brushes.Black), new Point(31 * i + 5, 40));
                for (int i = 0; i < 35; i++)
                    dc.DrawRectangle(null, blackp, new Rect(31 * i, 60, 31, 60));
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    int week = ((DateTime)dt.Rows[i][1]).DayOfWeek.GetHashCode();
                    int day = ((DateTime)dt.Rows[i][1]).Day;
                    string s = dt.Rows[i][0].ToString();
                    double left = (double)(week + (day + week / 7 * 7 + 1) * 31 + 5);
                    double top = i > 30 ? 100 : 70;
                    dc.DrawText(new FormattedText(s, ci, fd, tf, d, Brushes.Black), new Point(left, top));
                }
            }
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }
        protected override Visual GetVisualChild(int index)
        {
            if (index == 0)
                return dv;
            throw new IndexOutOfRangeException();
        }
    }
}

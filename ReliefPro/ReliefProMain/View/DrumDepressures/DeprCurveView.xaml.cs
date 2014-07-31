﻿using System;
using System.Collections.Generic;
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
using System.Windows.Controls.DataVisualization.Charting;

namespace ReliefProMain.View.DrumDepressures
{
    /// <summary>
    /// DeprCurveView.xaml 的交互逻辑
    /// </summary>
    public partial class DeprCurveView : Window
    {
        public KeyValuePair<int, double>[] ChartSource;
        public DeprCurveView()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ((LineSeries)this.mcChart.Series[0]).DependentRangeAxis = new LinearAxis() { Orientation = AxisOrientation.Y, Minimum = 0 };
            ((LineSeries)this.mcChart.Series[0]).ItemsSource = ChartSource;
        }
    }
}

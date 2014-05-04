using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ReliefProMain.CustomControl
{
    public class CustomComboBox : ComboBox
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomComboBox"/> class.
        /// </summary>
        public CustomComboBox()
        {
            Loaded += ComboBoxLoaded;
            SelectionChanged += ComboBoxSelectionChanged;
        }

        void ComboBoxLoaded(object sender, RoutedEventArgs e)
        {
            SetSelectedItem();
        }

        void ComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedItem = e.AddedItems.Count > 0 ? e.AddedItems[0] : null;
        }

        public new object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public new static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(CustomComboBox),
            new PropertyMetadata((o, e) => ((CustomComboBox)o).SetSelectedItem()));

        private void SetSelectedItem()
        {
            if (null != SelectedItem)
            {
                base.SelectedIndex = Items.IndexOf(SelectedItem);
                return;
            }
            if (Items.Count > 0)
                base.SelectedIndex = 0;

            //var value = SelectedItem;
            //if (Items.Count > 0 && value != null)
            //{
            //    base.SelectedIndex = Items.IndexOf(value);
            //}
        }

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            SetSelectedItem();
        }
    }
}

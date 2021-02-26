using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace Warehouse.Views
{
    /// <summary>
    /// Interaction logic for MainControl.xaml
    /// </summary>
    public partial class MainControl : UserControl
    {
        public MainControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Focuses on MainGrid to unfocus/unselect all other elements
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                MainGrid.Focus();
                DataView.UnselectAll();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to unfocus/unselect Main DataGrid. Error: {ex}");
            }
        }
    }
}

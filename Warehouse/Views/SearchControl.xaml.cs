using System;
using System.Windows;
using System.Windows.Controls;


namespace Warehouse.Views
{
    /// <summary>
    /// Interaction logic for SearchControl.xaml
    /// </summary>
    public partial class SearchControl : UserControl
    {
        // Search textbox input format
        private readonly string Format = "Pavyzdys: 1x1x1 2x2x2...";
        public SearchControl()
        {
            InitializeComponent();

            // Initial settings
            SearchTextBox.Opacity = 0.5;
        }

        /// <summary>
        /// Removes text from search texbox
        /// </summary>
        private void RemoveText()
        {
            if (SearchTextBox.Text == Format)
            {
                SearchTextBox.Text = "";
            }
        }

        /// <summary>
        /// Adds text to search textbox
        /// </summary>
        private void AddTextSetOpacity()
        {
            if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                SearchTextBox.Text = Format;
                SearchTextBox.Opacity = 0.5;
            }
        }

        /// <summary>
        /// Removes text from SearchTextBox
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                RemoveText();
                SearchTextBox.Opacity = 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to remove SearchTextBox text. Error: {ex}");
            }
        }

        /// <summary>
        /// Adds text to SearchTextBox
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                AddTextSetOpacity();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to add SearchTextBox text. Error: {ex}");
            }
        }

        /// <summary>
        /// Makes StatePicker visible
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void StateCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                StatePicker.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to make StatePicker visible. Error: {ex}");
            }
        }

        /// <summary>
        /// Makes StatePicker hidden
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void StateCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                StatePicker.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to make StatePicker hidden. Error: {ex}");
            }
        }

        /// <summary>
        /// Makes DatePicker visible
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DateCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                DatePicker.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to make DatePicker visible. Error: {ex}");
            }
        }

        /// <summary>
        /// Makes DatePicker hidden
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void DateCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                DatePicker.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to make DatePicker hidden. Error: {ex}");
            }
        }

        /// <summary>
        /// Focuses MainGrid to unfocus all other elements
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void UserControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                MainGrid.Focus();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to focus MainGrid. Error: {ex}");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Warehouse.Views
{
    /// <summary>
    /// Interaction logic for ChangeControl.xaml
    /// </summary>
    public partial class ChangeControl : UserControl
    {
        // wood size format example
        public static string Format = "10x10x10...";

        public ChangeControl()
        {
            InitializeComponent();

            // Initial settings
        }

        /// <summary>
        /// Shows single wood TextBox
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void SinglesCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                SinglesTextBox.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to set Singles Textbox to visible. Error: {ex}");
            }
        }

        /// <summary>
        /// Hides single wood textbox
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void SinglesCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                SinglesTextBox.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to set Singles Textbox to visible. Error: {ex}");
            }
        }

        /// <summary>
        /// Handles wood param textbox GotFocus event
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void WoodParamTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                RemoveText();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to delete ParamTextbox text. Error: {ex}");
            }
        }

        /// <summary>
        /// Handles wood param textbox LostFocus event
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void WoodParamTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                AddText();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to add ParamTextbox text. Error: {ex}");
            }
        }

        /// <summary>
        /// Removes text from textbox
        /// </summary>
        private void RemoveText()
        {
            if (WoodParamTextBox.Text == Format)
            {
                WoodParamTextBox.Text = "";
            }
        }

        /// <summary>
        /// Adds text to textbox
        /// </summary>
        private void AddText()
        {
            if (string.IsNullOrWhiteSpace(WoodParamTextBox.Text))
            {
                WoodParamTextBox.Text = Format;
            }
        }

        /// <summary>
        /// Handles packages text input event
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void PackagesTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            OnlyInts(e);
        }

        /// <summary>
        /// Handles singles text input event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SinglesTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            OnlyInts(e);
        }

        /// <summary>
        /// Lets only ints to be written
        /// </summary>
        /// <param name="e">e</param>
        private void OnlyInts(TextCompositionEventArgs e)
        {
            try
            {
                Regex regex = new Regex("[^0-9]+");
                e.Handled = regex.IsMatch(e.Text);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed regex. Error: {ex}");
            }

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
                ChangeDataView.UnselectAll();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to focus MainGrid or Unselect it's rows. Error: {ex}");
            }
        }

        /// <summary>
        /// Makes StartTimeBox2 Visible
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void StartTimeBoxCheck_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                StartTimeBox2.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to set StartTime Textbox to visible. Error: {ex}");
            }
        }

        /// <summary>
        /// Makes StartTimeBox2 hidden
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void StartTimeBoxCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                StartTimeBox2.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to set StartTime Textbox to hidden. Error: {ex}");
            }
        }

        /// <summary>
        /// Makes NoteTextBox visible
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void NoteCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                NoteTextBox.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to set NoteTextBox to visible. Error: {ex}");
            }
        }

        /// <summary>
        /// Makes NoteTextBox hidden
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void NoteCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                NoteTextBox.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to set NoteTextBox Textbox to hidden. Error: {ex}");
            }
        }

        /// <summary>
        /// Makes TypeCombobox visible
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void TypeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                TypeCombobox.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to set Typecombobox to visible. Error: {ex}");
            }
        }

        /// <summary>
        /// Makes TypeCombobox hidden
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void TypeCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                TypeCombobox.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to set Typecombobox to hidden. Error: {ex}");
            }
        }

        /// <summary>
        /// Makes EndTimeBox2 visible
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void EndTimeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                EndTimeBox2.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to set EndTimeBox2 to visible. Error: {ex}");
            }
        }

        /// <summary>
        /// Makes EndTimeBox2 hidden
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void EndTimeCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                EndTimeBox2.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to set EndTimeBox2 to hidden. Error: {ex}");
            }

        }

        /// <summary>
        /// Unselects nested datagrid (InsideDataGrid) rows
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void InsideDataView_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                DataGrid InsideDataGrid = sender as DataGrid;
           //     if(!InsideDataGrid.IsVisible)
                  //  InsideDataGrid.UnselectAll();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to Unselect all InsideDataGrid rows. Error: {ex}");
            }

        }
    }
}

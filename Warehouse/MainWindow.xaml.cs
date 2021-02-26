using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using Squirrel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using Warehouse.viewModel;

namespace Warehouse
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        // Main unit list
        readonly static ObservableCollection<Unit> Units = new ObservableCollection<Unit>();

        // Views
        readonly MainViewModel mainView = new MainViewModel(Units);
        readonly ChangeViewModel changeView = new ChangeViewModel(Units);
        readonly SearchViewModel searchView = new SearchViewModel(Units);

        // Timer
        private DispatcherTimer dispatcherTimer;

        private readonly DataAccess db = new DataAccess();

        public MainWindow()
        {
            InitializeComponent();

            // Sets timer
            SetTimer();

            // Sets content control to main view
            DataContext = mainView;

            SetTimer();
        }

        /// <summary>
        /// Sets DispatcherTimer parameters and starts it
        /// </summary>
        private void SetTimer()
        {
            try
            {
                dispatcherTimer = new DispatcherTimer();
                dispatcherTimer.Tick += DispatcherTimer_Tick;
                // 1 Second interval
                dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
                dispatcherTimer.Start();
            }
            catch (Exception ex)
            { Console.WriteLine($"Failed to set up and start timer. Error: {ex}"); }
        }

        /// <summary>
        /// DispatcherTimer event
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void DispatcherTimer_Tick(object sender, object e)
        {
            RefreshInformation();
        }

        /// <summary>
        /// Updates states of units
        /// </summary>
        private void RefreshInformation()
        {
            // All unit states; Just for visibility
            string Baigta = UnitStates.Baigta;
            string Dziovinama = UnitStates.Dziovinama;
            string Tuscia = UnitStates.Tuscia;
            string Kraunama = UnitStates.Kraunama;
            string Pakrauta = UnitStates.Pakrauta;

            foreach (Unit unit in Units)
            {
                try
                {
                    // Check if unit is finished working and set to: Baigta
                    if (unit.EndTime <= DateTime.Now && unit.State.Equals(Dziovinama))
                    {
                        int check = db.UpdateUnitStateByID(unit.Id, Baigta);

                        if (check > 0)
                            unit.State = Baigta;
                    }

                    // If unit is Tuscias and if it has Wood, set to: Kraunama
                    if (unit.Woods.Count > 0 && unit.State.Equals(Tuscia))
                    {
                        int check = db.UpdateUnitStateByID(unit.Id, Kraunama);

                        if (check > 0)
                            unit.State = Kraunama;
                    }

                    // If unit doesn't have wood and is not Tuscia and
                    // is either Dziovinama or Baigta or Pakrauta, set it to: Tuscia
                    if (unit.Woods.Count == 0 && !unit.State.Equals(Tuscia) &&
                      (unit.State.Equals(Dziovinama) || unit.State.Equals(Baigta) || unit.State.Equals(Pakrauta)))
                    {
                        int check = db.UpdateUnitStateByID(unit.Id, Tuscia);

                        if (check > 0)
                            unit.State = Tuscia;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to update unit states. Error: {ex}");
                }
            }
        }


        /// <summary>
        /// Changes datacontext of content control to Main view.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void Main_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Set DataContext
                DataContext = mainView;

                // Manage UI 
                main.Background = Brushes.Gray;
                change.Background = Brushes.LightGray;
                search.Background = Brushes.LightGray;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to change view to mainView. Error: {ex}");
            }
        }

        /// <summary>
        /// Changes datacontext of content control to Change view.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void Change_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Change DataContext
                DataContext = changeView;

                // Manage UI 
                main.Background = Brushes.LightGray;
                change.Background = Brushes.Gray;
                search.Background = Brushes.LightGray;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to change view to changeView. Error: {ex}");
            }
        }

        /// <summary>
        /// Changes datacontext of content control to Search view.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Change DataContext
                DataContext = searchView;

                // Manage UI 
                main.Background = Brushes.LightGray;
                change.Background = Brushes.LightGray;
                search.Background = Brushes.Gray;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to change view to searchView. Error: {ex}");
            }
        }


        /// <summary>
        /// Imports Excel file to Unit list & Database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Import_Click(object sender, RoutedEventArgs e)
        {

            // If import fails
            ObservableCollection<Unit> backupUnits = CreateABackUpList();

            // establish everything needed
            ApplicationClass excapp = new ApplicationClass();
            Workbooks workbooks = null;
            Workbook workbook = null;
            Sheets sheets = null;
            Worksheet sheet = null;
            Range rng = null;

            try
            {
                dispatcherTimer.Stop();


                // File browser
                OpenFileDialog fd = new OpenFileDialog
                {
                    Title = "Open Excel File",
                    Filter = "Excel File|*.xlsx"
                };
                bool? result = fd.ShowDialog();

                // Check before writing
                if (fd.FileName == "" || result == false)
                    return;



                // Clear Unit list
                if (db.DeleteAll() > 0)
                    Units.Clear();

                workbooks = excapp.Workbooks;
                workbook = workbooks.Open(fd.FileName);
                sheets = workbook.Sheets;
                sheet = (Worksheet)sheets[1];

                // Excel params
                excapp.Visible = false;
                excapp.DisplayAlerts = false;

                // Read excel data
                ReadFromExcel(sheet);

                // Quit excel
                workbook.Close(false);
                excapp.Quit();

                // Refresh DataGrid with new information
                RefreshData();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to import excel file. Error: {ex}");

                // Clear any bad imports
                Units.Clear();

                // Write old units
                foreach (Unit unit in backupUnits)
                {
                    Units.Add(unit);
                }
            }
            finally
            {
                DisposeExcelCom(rng, sheet, sheets, workbook, workbooks, excapp);
                dispatcherTimer.Start();
            }

        }

        /// <summary>
        /// Refreshes Unit list using Database data
        /// </summary>
        private void RefreshData()
        {
            Units.Clear();
            foreach (Unit unit in db.GetAllUnits())
            {
                Units.Add(unit);
            }
        }

        /// <summary>
        /// Exports Units list information to Excel file
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void Export_Click(object sender, RoutedEventArgs e)
        {

            // establish everything needed
            ApplicationClass excapp = new ApplicationClass();
            Workbooks workbooks = null;
            Workbook workbook = null;
            Sheets sheets = null;
            Worksheet sheet = null;
            Range rng = null;

            try
            {
                dispatcherTimer.Stop();
                // File browser
                SaveFileDialog fd = new SaveFileDialog
                {
                    Filter = "Excel File|*.xlsx",
                    Title = "Save Excel File"
                };
                bool? result = fd.ShowDialog();

                // Check before writing
                if (fd.FileName == "" || result == false)
                    return;

                // Sort new list, so UI list doesn't change
                List<Unit> tempUnits = new List<Unit>();
                tempUnits = Units.ToList();
                tempUnits.Sort((a, b) => a.Key.CompareTo(b.Key));

                // Excel parameters
                excapp.Visible = false;
                excapp.DisplayAlerts = false;

                // Create new excel workbook and sheet
                workbooks = excapp.Workbooks;
                workbook = workbooks.Add(XlWBATemplate.xlWBATWorksheet);
                sheets = workbook.Sheets;
                sheet = (Worksheet)sheets[1];

                // Print date of export in first line
                PrintGenerationDate(sheet);
                // Print unit count in second line
                PrintUnitCount(tempUnits, sheet);
                // Print all unit information
                WriteToExcel(tempUnits, sheet);

                // Save file
                workbook.SaveAs(fd.FileName);

                // Close workbook, False = don't save workbook values.
                workbook.Close(false);
                excapp.Quit();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to export excel file. Error: {ex}");
            }
            finally
            {
                DisposeExcelCom(rng, sheet, sheets, workbook, workbooks, excapp);
                dispatcherTimer.Start();
            }
        }

        /// <summary>
        /// Prints date of Excel file generation to first line
        /// </summary>
        /// <param name="sheet">Worksheet</param>
        private void PrintGenerationDate(Worksheet sheet)
        {
            // Print generation date. 
            WriteToCell("A", 1, "Sugeneruota:", sheet);
            // Put additional " " around the text for date to be visible in excel
            WriteToCell("B", 1, $"{'"'}{DateTime.Now}{'"'}", sheet);
        }

        /// <summary>
        /// Print Unit list Count
        /// </summary>
        /// <param name="unitList">Unit list</param>
        /// <param name="sheet">Worksheet</param>
        private void PrintUnitCount(List<Unit> unitList, Worksheet sheet)
        {
            // Print label
            WriteToCell("A", 2, "Dziovyklu sk:", sheet);
            // Print number
            WriteToCell("B", 2, unitList.Count.ToString(), sheet);
        }

        /// <summary>
        /// Prints Wood information
        /// </summary>
        /// <param name="unitList">Unit list</param>
        /// <param name="row">Row to print to</param>
        /// <param name="unitIndex">Unit number</param>
        /// <param name="woodIndex">Wood Number</param>
        /// <param name="sheet">Worksheet</param>
        private void PrintWoodBody(List<Unit> unitList, int row, int unitIndex, int woodIndex, Worksheet sheet)
        {
            // Print unit wood Height
            WriteToCell("B", row, unitList[unitIndex].Woods[woodIndex].Height.ToString(), sheet);

            // Print unit wood Width
            WriteToCell("C", row, unitList[unitIndex].Woods[woodIndex].Width.ToString(), sheet);

            // Print unit wood Depth
            WriteToCell("D", row, unitList[unitIndex].Woods[woodIndex].Depth.ToString(), sheet);

            // Print unit wood Packages
            WriteToCell("E", row, unitList[unitIndex].Woods[woodIndex].Packages.ToString(), sheet);

            // Print unit wood Singles
            WriteToCell("F", row, unitList[unitIndex].Woods[woodIndex].Singles.ToString(), sheet);

            // Print unit wood Type
            WriteToCell("G", row, unitList[unitIndex].Woods[woodIndex].Type.ToString(), sheet);

            // Print unit wood Note
            WriteToCell("H", row, unitList[unitIndex].Woods[woodIndex].Note.ToString(), sheet);
        }

        /// <summary>
        /// Prints header for Wood body
        /// </summary>
        /// <param name="row">Row to print to</param>
        /// <param name="sheet">Worksheet</param>
        private void PrintWoodHeader(int row, Worksheet sheet)
        {
            // Print unit wood Height label
            WriteToCell("B", row, "Ilgis", sheet);

            // Print unit wood Width label
            WriteToCell("C", row, "Plotis", sheet);

            // Print unit wood Depth label
            WriteToCell("D", row, "Aukstis", sheet);

            // Print unit wood Packages label
            WriteToCell("E", row, "Paketai", sheet);

            // Print unit wood Singles label
            WriteToCell("F", row, "Vienetai", sheet);

            // Print unit wood Type label
            WriteToCell("G", row, "Rusis", sheet);

            // Print unit wood Note label
            WriteToCell("H", row, "Pastaba", sheet);

        }

        /// <summary>
        /// Prints Wood list Count
        /// </summary>
        /// <param name="unitList">Unit list</param>
        /// <param name="row">Row to print to</param>
        /// <param name="unitIndex">Unit number</param>
        /// <param name="sheet">Worksheet</param>
        private void PrintWoodCount(List<Unit> unitList, int row, int unitIndex, Worksheet sheet)
        {
            // Print unit wood label
            WriteToCell("B", row, "Paketu sk:", sheet);

            // Print how much wood unit has
            WriteToCell("C", row, unitList[unitIndex].Woods.Count.ToString(), sheet);
        }

        /// <summary>
        /// Prints Unit information
        /// </summary>
        /// <param name="unitList">Unit list</param>
        /// <param name="row">Row to print to</param>
        /// <param name="unitIndex">Unit number</param>
        /// <param name="sheet">Worksheet</param>
        private void PrintUnitBody(List<Unit> unitList, int row, int unitIndex, Worksheet sheet)
        {
            // Print units key
            WriteToCell("A", row, unitList[unitIndex].Key.ToString(), sheet);

            // Print unit state
            WriteToCell("B", row, unitList[unitIndex].State.ToString(), sheet);

            // Print unit startDate
            WriteToCell("C", row, $"{'"'}{unitList[unitIndex].StartTime}{'"'}", sheet);

            // Print unit endDate
            WriteToCell("D", row, $"{'"'}{unitList[unitIndex].EndTime}{'"'}", sheet);

        }

        /// <summary>
        /// Prints Unit header
        /// </summary>
        /// <param name="row">Row to print to</param>
        /// <param name="sheet">Worksheet</param>
        private void PrintUnitHeader(int row, Worksheet sheet)
        {
            // Print units key
            WriteToCell("A", row, "Dziovykla", sheet);

            // Print unit state
            WriteToCell("B", row, "Busena", sheet);

            // Print unit startDate
            WriteToCell("C", row, "Pradzios data", sheet);

            // Print unit endDate
            WriteToCell("D", row, "Pabaigos data", sheet);

        }

        /// <summary>
        /// Writes to Excel Cell
        /// </summary>
        /// <param name="letter">Cell letter</param>
        /// <param name="number">Cell number</param>
        /// <param name="value">New value</param>
        /// <param name="sheet">Worksheet</param>
        private void WriteToCell(string letter, int number, string value, Worksheet sheet)
        {
            // Cell to write
            string cellName = $"{letter}{number}";
            Range rng = sheet.get_Range(cellName);

            rng.Value2 = value;
            Marshal.ReleaseComObject(rng);
        }

        /// <summary>
        /// Reads data from Excel Worksheet
        /// </summary>
        /// <param name="sheet">worksheet</param>
        private void ReadFromExcel(Worksheet sheet)
        {
            // Skip first two lines because headers
            int newRow = 3;

            for (int i = 0; i < ReturnUnitCount(sheet); i++)
            {
                // Find new row and wood count
                int unitRow = (i + newRow + 1);
                int woodCount = ReturnWoodCount(unitRow + 1, sheet);

                // Add unit and return it's key
                int key = ReadAddUnit(unitRow, sheet);

                for (int j = 0; j < woodCount; j++)
                {
                    int rowNumber = (i + j + 4 + newRow);
                    // Read and add wood to Unit by key
                    ReadAddWoodByKey(key, rowNumber, sheet);
                }
                // Calculates new row, + 4 because of static headers
                newRow += woodCount + 4;
            }
        }

        /// <summary>
        /// Returns Unit list Count
        /// </summary>
        /// <param name="sheet">Worksheet</param>
        /// <returns>Count</returns>
        private int ReturnUnitCount(Worksheet sheet)
        {
            Range rng = sheet.get_Range("B2");

            int value = int.Parse(rng.Value2.ToString());

            Marshal.ReleaseComObject(rng);
            return value;
        }

        /// <summary>
        /// Returns Wood list Count
        /// </summary>
        /// <param name="row">Row to print to</param>
        /// <param name="sheet">Worksheet</param>
        /// <returns>Count</returns>
        private int ReturnWoodCount(int row, Worksheet sheet)
        {
            // Cell to read
            string cellName = $"C{row}";
            Range rng = sheet.get_Range(cellName);

            // Value
            int value = int.Parse(rng.Value2.ToString());

            // Manual clean up
            Marshal.ReleaseComObject(rng);
            return value;
        }

        /// <summary>
        /// Returns info of cell
        /// </summary>
        /// <param name="letter">Cell letter</param>
        /// <param name="number">Cell number</param>
        /// <param name="sheet">Worksheet</param>
        /// <returns></returns>
        private string ReturnInfo(string letter, int number, Worksheet sheet)
        {
            // Cell to read
            string cellName = $"{letter}{number}";
            Range rng = sheet.get_Range(cellName);

            // Return blank string if empty
            string value = "";
            if (rng.Value2 != null)
                value = rng.Value2.ToString();

            // Manual clean up
            Marshal.ReleaseComObject(rng);
            return value;
        }

        /// <summary>
        /// Reads and adds Unit
        /// </summary>
        /// <param name="row">Row to print to</param>
        /// <param name="sheet">Worksheet</param>
        /// <returns></returns>
        private int ReadAddUnit(int row, Worksheet sheet)
        {
            int key = int.Parse(ReturnInfo("A", row, sheet));

            string state = ReturnInfo("B", row, sheet);

            // We have to replace \" with spaces because we generate date with additional "" for visibility
            string startTimeString = ReturnInfo("C", row, sheet).Replace("\"", " ");
            DateTime startTime = DateTime.Parse(startTimeString);

            string endTimeString = ReturnInfo("D", row, sheet).Replace("\"", " ");
            DateTime endTime = DateTime.Parse(endTimeString);

            db.AddUnit(key, state, startTime, endTime);

            return key;
        }

        /// <summary>
        /// Reads and adds Wood to Unit by key
        /// </summary>
        /// <param name="key">Unit key</param>
        /// <param name="row">Row to print to</param>
        /// <param name="sheet">Worksheet</param>
        private void ReadAddWoodByKey(int key, int row, Worksheet sheet)
        {
            double height = double.Parse(ReturnInfo("B", row, sheet));
            double width = double.Parse(ReturnInfo("C", row, sheet));
            double depth = double.Parse(ReturnInfo("D", row, sheet));
            int packages = int.Parse(ReturnInfo("E", row, sheet));
            int singles = int.Parse(ReturnInfo("F", row, sheet));
            string type = ReturnInfo("G", row, sheet);
            string note = ReturnInfo("H", row, sheet);

            Unit unit = db.GetUnitByKey(key);
            db.AddWoodToUnit(height, width, depth, packages, singles, type, note, unit.Id);
        }

        /// <summary>
        /// Creates a backup of main Unit list
        /// </summary>
        /// <returns>Backup Unit list</returns>
        private ObservableCollection<Unit> CreateABackUpList()
        {
            ObservableCollection<Unit> newList = new ObservableCollection<Unit>();
            foreach (Unit unit in Units)
            {
                newList.Add(unit);
            }
            return newList;
        }

        /// <summary>
        /// Disposes of COM objects after excel
        /// </summary>
        /// <param name="rng">Range</param>
        /// <param name="sheet">Worksheet</param>
        /// <param name="sheets">Sheets</param>
        /// <param name="workbook">Workbook</param>
        /// <param name="workbooks">Workbooks</param>
        /// <param name="excapp">ApplicationClass</param>
        private void DisposeExcelCom(Range rng, Worksheet sheet, Sheets sheets, Workbook workbook, Workbooks workbooks, ApplicationClass excapp)
        {
            // Manual disposal because of COM
            if (rng != null)
            {
                Marshal.FinalReleaseComObject(rng);
            }
            if (sheet != null)
            {
                Marshal.FinalReleaseComObject(sheet);
            }
            if (sheets != null)
            {
                Marshal.FinalReleaseComObject(sheets);
            }
            if (workbook != null)
            {
                Marshal.FinalReleaseComObject(workbook);
            }
            if (workbooks != null)
            {
                Marshal.FinalReleaseComObject(workbooks);
            }
            if (excapp != null)
            {
                Marshal.FinalReleaseComObject(excapp);
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// Writes to excel sheet
        /// </summary>
        /// <param name="tempUnits">Unit type list</param>
        /// <param name="sheet">Excel Worksheet</param>
        private void WriteToExcel(List<Unit> tempUnits, Worksheet sheet)
        {
            // Skip first two lines
            int newRow = 3;

            for (int i = 0; i < tempUnits.Count; i++)
            {
                // Rows 
                int unitLabelRow = (i + newRow);
                int unitRow = (i + newRow + 1);
                int woodCountRow = (i + newRow + 2);
                int woodLabelRow = (i + newRow + 3);

                PrintUnitHeader(unitLabelRow, sheet);
                PrintUnitBody(tempUnits, unitRow, i, sheet);
                PrintWoodCount(tempUnits, woodCountRow, i, sheet);
                PrintWoodHeader(woodLabelRow, sheet);

                // Print unit woods
                for (int j = 0; j < tempUnits[i].Woods.Count; j++)
                {
                    int rowNumber = (i + j + 4 + newRow);
                    PrintWoodBody(tempUnits, rowNumber, i, j, sheet);
                }

                // We add woods.Count and additional header lines to skip for each iteration
                newRow += tempUnits[i].Woods.Count + 4;
            }
        }
    }
}

using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Warehouse.viewModel
{
    class ChangeViewModel : INotifyPropertyChanged
    {

        // Main Unit list for datagrid
        public ObservableCollection<Unit> Units { get; set; }

        // List for UI new state combobox
        public ObservableCollection<string> States { get; set; }
        public ObservableCollection<string> Types { get; set; }

        // Access to database
        private readonly DataAccess db = new DataAccess();

        // Relay commands
        public RelayCommand ClearUnitCommand { get; }
        public RelayCommand AddUnitCommand { get; }
        public RelayCommand DeleteUnitCommand { get; }
        public RelayCommand DeleteWoodCommand { get; }
        public RelayCommand AddWoodCommand { get; }

        public RelayCommand ChangeTimesCommand { get; }

        // Event handler
        public event PropertyChangedEventHandler PropertyChanged;


        // INotify Properties
        private string selectedState;
        private Wood selectedWood;
        private Unit selectedUnit;
        private string woodParam;
        private int packages;
        private int singles;
        private DateTime startTime;
        private DateTime endTime;
        private DateTime startTimeHourly;
        private DateTime endTimeHourly;
        private string note;
        private string selectedType;
        private bool startTimeCheck;
        private bool endTimeCheck;
        private bool singlesCheck;
        private bool noteCheck;
        private bool typeCheck;


        /// <summary>
        /// Create ChangeViewModel object 
        /// </summary>
        /// <param name="units">Unit list</param>
        public ChangeViewModel(ObservableCollection<Unit> units)
        {

            Units = units;
            States = new ObservableCollection<string>();
            Types = new ObservableCollection<string>();


            // Relay commands
            ClearUnitCommand = new RelayCommand(() => Clear());
            AddWoodCommand = new RelayCommand(() => AddWood());
            DeleteWoodCommand = new RelayCommand(() => DeleteWood());
            AddUnitCommand = new RelayCommand(() => Add());
            DeleteUnitCommand = new RelayCommand(() => Delete());
            ChangeTimesCommand = new RelayCommand(() => ChangeTime());

            // Functions
            SetLabels();
            AddItemsToComboBoxes();
            SetTimes();
        }

        /// <summary>
        /// 
        /// </summary>
        private void ChangeTime()
        {
            if (selectedUnit != null)
            {
                DateTime newStartTime;
                DateTime newEndTime;
                // New times for Unit, If checked we add hh:mm input
                if (StartTimeCheck)
                    newStartTime = new DateTime(StartTime.Year, StartTime.Month, StartTime.Day, StartTimeHourly.Hour, StartTimeHourly.Minute, StartTimeHourly.Second);
                else
                    newStartTime = new DateTime(StartTime.Year, StartTime.Month, StartTime.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

                if (EndTimeCheck)
                    newEndTime = new DateTime(EndTime.Year, EndTime.Month, EndTime.Day, EndTimeHourly.Hour, EndTimeHourly.Minute, EndTimeHourly.Second);
                else
                    newEndTime = new DateTime(EndTime.Year, EndTime.Month, EndTime.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

                // Update database
                int check = db.UpdateUnitTimeByID(selectedUnit.Id, newStartTime, newEndTime);

                // If update successful, update Datagrid and reset inputs
                if (check > 0)
                {
                    SelectedUnit.StartTime = newStartTime;
                    SelectedUnit.EndTime = newEndTime;


                    SetTimes();
                }
            }
        }

        /// <summary>
        /// Sets time input box texts
        /// </summary>
        private void SetTimes()
        {
            StartTime = DateTime.Now;
            EndTime = DateTime.Now;
            StartTimeHourly = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            EndTimeHourly = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
        }

        /// <summary>
        /// Adds all available states to combobox
        /// </summary>
        private void AddItemsToComboBoxes()
        {
            // Add states
            States.Add(UnitStates.Kraunama);
            States.Add(UnitStates.Pakrauta);
            States.Add(UnitStates.Dziovinama);
            States.Add(UnitStates.Baigta);
            States.Add(UnitStates.Tuscia);

            // Add Types
            Types.Add("E");
            Types.Add("P");
        }

        /// <summary>
        /// Sets Textboxes on first load
        /// </summary>
        private void SetLabels()
        {
            try
            {
                // Sets Parameters textbox
                WoodParam = "10x10x10...";

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to add Param Textbox text. Error: {ex}");
            }
        }

        /// <summary>
        /// Deletes selected wood from list
        /// </summary>
        private void DeleteWood()
        {
            try
            {
                if (SelectedUnit != null && SelectedWood != null && SelectedUnit.Woods.Contains(SelectedWood))
                {
                    int check = db.DeleteWoodByID(SelectedWood.Id);

                    if (check > 0)
                        SelectedUnit.Woods.Remove(SelectedWood);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to delete Unit from ChangeDataView (datagrid). Error: {ex}");
            }
        }

        /// <summary>
        /// Clears Unit
        /// </summary>
        private void Clear()
        {
            try
            {
                if (SelectedUnit != null)
                {
                    int check = db.ClearUnitByID(SelectedUnit.Id);

                    if (check > 0)
                        SelectedUnit.Woods.Clear();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to Clear ChangeDataView (datagrid). Error: {ex}");
            }

        }

        /// <summary>
        /// Deletes Selected Unit
        /// </summary>
        private void Delete()
        {
            try
            {
                if (SelectedUnit != null)
                {
                    int check = db.DeleteUnitByID(SelectedUnit.Id);

                    if (check > 0)
                        Units.Remove(SelectedUnit);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to delete Unit from ChangeDataView (datagrid). Error: {ex}");
            }
        }

        /// <summary>
        /// Adds empty Unit to the Unit list
        /// </summary>
        private void Add()
        {
            try
            {
                int newKey = MissingKey();
                int result = db.AddEmptyUnit(newKey);

                if (result > 0)
                {
                    Unit unit = db.GetUnitByKey(newKey);
                    Units.Add(unit);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to add Unit to Main ChangeDataView (datagrid). Error: {ex}");
            }
        }

        /// <summary>
        /// Calculates missing key in Units and returns it
        /// </summary>
        /// <returns>Missing Key from Unit list</returns>
        private int MissingKey()
        {

            List<int> keyList = KeyList();
            if (keyList.Count == 0)
                return 1;

            keyList.Sort();

            for (int i = 1; i < keyList.Max(); i++)
            {
                if (!keyList.Contains(i))
                    return i;
            }

            return keyList.Max() + 1;
        }

        /// <summary>
        /// Gets all Keys from Units list
        /// </summary>
        /// <returns>Unit Key list</returns>
        private List<int> KeyList()
        {
            List<int> keyList = new List<int>();
            foreach (Unit un in Units)
            {
                try
                {
                    keyList.Add(un.Key);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Couldn't add to key list. Error: {ex}");
                }
            }
            return keyList;
        }


        /// <summary>
        /// Adds Wood to Selected Unit
        /// </summary>
        private void AddWood()
        {
            try
            {
                if (SelectedUnit != null)
                {
                    // Gets all wood parameters
                    string[] values = WoodParam.Split('x');

                    // Empty check
                    if (values.Length == 0)
                        return;

                    // Replace so you can type , or .
                    if (!double.TryParse(values[0].Replace(',', '.'), out double height) ||
                        !double.TryParse(values[1].Replace(',', '.'), out double width) ||
                        !double.TryParse(values[2].Replace(',', '.'), out double depth))
                        return;

                    int singles = 0;
                    if (SinglesCheck == true)
                        singles = Singles;

                    string type = "";
                    if (SelectedType != null && TypeCheck == true)
                        type = SelectedType;

                    string note = "";
                    if (Note != null && NoteCheck == true)
                        note = Note;

                    int check = db.AddWoodToUnit(height, width, depth, Packages, singles, type, note, SelectedUnit.Id);

                    if (check > 0)
                    {
                        // Update UI
                        SelectedUnit.Woods = db.GetWoodsById(SelectedUnit.Id);

                        // Reset UI values
                        Singles = 0;
                        Note = "";
                        Packages = 0;
                        WoodParam = "10x10x10...";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to Add Wood to Unit. Error: {ex}");
            }
        }

        /// <summary>
        /// INotify event
        /// </summary>
        /// <param name="propertyName">Property name</param>
        private void NotifyPropertyChanged(String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Sets or Gets Selected Wood from DataGrid
        /// </summary>
        public Wood SelectedWood
        {
            get { return selectedWood; }
            set
            {
                selectedWood = value;
                NotifyPropertyChanged("SelectedWood");
            }
        }

        /// <summary>
        /// Sets or Gets Selected Unit from DataGrid
        /// </summary>
        public Unit SelectedUnit
        {
            get { return selectedUnit; }
            set
            {
                selectedUnit = value;
                NotifyPropertyChanged("SelectedUnit");
            }
        }

        /// <summary>
        /// Sets or gets Wood parameter text
        /// </summary>
        public string WoodParam
        {
            get { return woodParam; }
            set
            {
                woodParam = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Sets or Gets Packages
        /// </summary>
        public int Packages
        {
            get { return packages; }
            set
            {
                packages = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Sets or Gets Singles
        /// </summary>
        public int Singles
        {
            get { return singles; }
            set
            {
                singles = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Sets or Gets StartTime
        /// </summary>
        public DateTime StartTime
        {
            get
            {
                return startTime;
            }
            set
            {
                startTime = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Sets or Gets EndTime
        /// </summary>
        public DateTime EndTime
        {
            get { return endTime; }
            set
            {
                endTime = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Sets or Gets StartTimeHourly
        /// </summary>
        public DateTime StartTimeHourly
        {
            get { return startTimeHourly; }
            set
            {
                startTimeHourly = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Sets or Gets EndTimeHourly
        /// </summary>
        public DateTime EndTimeHourly
        {
            get { return endTimeHourly; }
            set
            {
                endTimeHourly = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Sets or Gets SelectedState with DB
        /// This approuch enables us to select same item many times
        /// </summary>
        public string SelectedState
        {
            get { return selectedState; }
            set
            {
                // Set UI & Database
                int check = db.UpdateUnitStateByID(selectedUnit.Id, value);
                if (check > 0)
                    SelectedUnit.State = value;
            }
        }

        /// <summary>
        /// Sets or Gets Note
        /// </summary>
        public string Note
        {
            get { return note; }
            set
            {
                note = value;
                NotifyPropertyChanged();

            }
        }

        /// <summary>
        /// Sets or Gets SelectedTye
        /// </summary>
        public string SelectedType
        {
            get { return selectedType; }
            set
            {
                selectedType = value;
                NotifyPropertyChanged();

            }
        }

        /// <summary>
        /// Sets or Gets NoteCheck
        /// </summary>
        public bool NoteCheck
        {
            get { return noteCheck; }
            set
            {
                noteCheck = value;
                NotifyPropertyChanged();

            }
        }

        /// <summary>
        /// Sets or Gets TypeCheck
        /// </summary>
        public bool TypeCheck
        {
            get { return typeCheck; }
            set
            {
                typeCheck = value;
                NotifyPropertyChanged();

            }
        }

        /// <summary>
        /// Sets or Gets EndTimeCheck
        /// </summary>
        public bool EndTimeCheck
        {
            get { return endTimeCheck; }
            set
            {
                endTimeCheck = value;
                NotifyPropertyChanged("EndTimeCheck");

            }
        }

        /// <summary>
        /// Sets or Gets StartTimeCheck
        /// </summary>
        public bool StartTimeCheck
        {
            get { return startTimeCheck; }
            set
            {
                startTimeCheck = value;
                NotifyPropertyChanged("StartTimeCheck");
            }
        }

        /// <summary>
        /// Sets or Gets SinglesCheck
        /// </summary>
        public bool SinglesCheck
        {
            get { return singlesCheck; }
            set
            {
                singlesCheck = value;
                NotifyPropertyChanged();
            }
        }
    }
}

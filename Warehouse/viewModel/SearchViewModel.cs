using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Warehouse.viewModel
{
    class SearchViewModel : INotifyPropertyChanged
    {
        // Main Unit list
        public ObservableCollection<Unit> Units { get; set; }

        // New list that we show on search page DataGrid
        public ObservableCollection<Unit> SearchUnits { get; set; }

        // Relay Commands
        public RelayCommand SearchCommand { get; }
        public RelayCommand ClearCommand { get; }

        // Even handler property
        public event PropertyChangedEventHandler PropertyChanged;

        // INotify properties
        private string labelText;
        private string state;
        private DateTime date;
        private bool stateCheck;
        private bool dateCheck;
        private string errorText;

        // Possible search errors
        private readonly static string badInputSize = "Klaida: Įrašykite dydžius";
        private readonly static string smallParamCount = "Klaida: Trūksta parametrų";
        private readonly static string bigParamCount = "Klaida: Per daug parametrų";
        private readonly static string badParams = "Klaida: Blogi parametrai";
        private readonly static string searchLabelText = "Pavyzdys: 1x1x1 2x2x2...";


        /// <summary>
        /// Create new SearchViewModel object
        /// </summary>
        /// <param name="units">Unit list</param>
        public SearchViewModel(ObservableCollection<Unit> units)
        {
            Units = units;
            SearchUnits = new ObservableCollection<Unit>();

            // Relay commands
            SearchCommand = new RelayCommand(() => Search());
            ClearCommand = new RelayCommand(() => Clear());

            // Functions
            SetDatePicker();
            SetSearchLabel();
        }

        /// <summary>
        /// Sets search textbox text on first load
        /// </summary>
        private void SetSearchLabel()
        {
            try
            {
                LabelText = searchLabelText;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to add text to Search TextBox. Error: {ex}");
            }
        }

        /// <summary>
        /// Sets Date Picker to DateTime.now
        /// </summary>
        private void SetDatePicker()
        {
            try
            {
                Date = DateTime.Now;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to set date to DatePicker. Error: {ex}");
            }
        }



        /// <summary>
        /// Clears search results
        /// </summary>
        private void Clear()
        {
            try
            {
                if (SearchUnits.Count > 0)
                    SearchUnits.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to clear Search datagrid. Error: {ex}");
            }
        }

        /// <summary>
        /// Searches Unit list for Units with wanted wood
        /// </summary>
        private void Search()
        {
            // Resets search datagrid and error label
            SearchUnits.Clear();
            ErrorText = "";

            // Check if input didn't change from init
            if (LabelText.Equals(searchLabelText))
            {
                ErrorText = badInputSize;
                return;
            }

            // Gets wanted wood
            List<Wood> woodList = WoodList();

            if (woodList.Count <= 0)
                return;

            if (DateCheck == false && StateCheck == false)
                WoodSearchWithoutParams(woodList);
            else if (DateCheck == true && StateCheck == false)
                WoodSearchWithDateCheck(woodList);
            else if (DateCheck == false && stateCheck == true)
                WoodSearchWithStateCheck(woodList);
            else if (DateCheck == true && StateCheck == true)
                WoodSearchWithBothParams(woodList);


            if (SearchUnits.Count == 0 &&
               (ErrorText != badParams || ErrorText != smallParamCount || ErrorText != bigParamCount || ErrorText != badInputSize))
                ErrorText = "Nerasta";

        }

        /// <summary>
        /// Searches Unit list for wanted wood. Checks Date and State
        /// </summary>
        /// <param name="woodList"></param>
        private void WoodSearchWithBothParams(List<Wood> woodList)
        {
            foreach (Unit unit in Units)
            {
                if (CompareDates(unit) && unit.State.Equals(State))
                    UnitSearch(unit, woodList);
            }
        }

        /// <summary>
        /// Searches Units list for wanted wood. Doesn't check Date or State
        /// </summary>
        private void WoodSearchWithoutParams(List<Wood> woodList)
        {
            // Loops through units
            foreach (Unit unit in Units)
                UnitSearch(unit, woodList);
        }

        /// <summary>
        /// Searches Unit list for wanted wood. Checks Date
        /// </summary>
        /// <param name="woodList">Wood to look for</param>
        private void WoodSearchWithDateCheck(List<Wood> woodList)
        {
            foreach (Unit unit in Units)
            {
                if (CompareDates(unit))
                    UnitSearch(unit, woodList);

            }
        }

        /// <summary>
        /// Searches Unit list for wanted wood. Checks State
        /// </summary>
        /// <param name="woodList">Wood to look for</param>
        private void WoodSearchWithStateCheck(List<Wood> woodList)
        {
            foreach (Unit unit in Units)
            {
                if (unit.State.Equals(State))
                    UnitSearch(unit, woodList);
            }
        }

        /// <summary>
        /// Searches the Unit for the wanted Wood
        /// </summary>
        /// <param name="unit">Unit to search</param>
        /// <param name="woodList">Wood to look for</param>
        private void UnitSearch(Unit unit, List<Wood> woodList)
        {
            try
            {
                ObservableCollection<Wood> returnedWoods = CheckUnitForWood(unit, woodList);
                if (returnedWoods.Count > 0)
                    AddToSearchList(returnedWoods, unit);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to add to the SearchUnit list. DateCheck && StateCheck == false. Error: {ex}");
            }
        }

        /// <summary>
        /// Adds to SearchList
        /// </summary>
        /// <param name="returnedWoods">woods to add to the list</param>
        /// <param name="unit">Unit to add to</param>
        private void AddToSearchList(ObservableCollection<Wood> returnedWoods, Unit unit)
        {

            ObservableCollection<Wood> woods = new ObservableCollection<Wood>();

            foreach (Wood wo in returnedWoods)
                woods.Add(wo);

            SearchUnits.Add(new Unit(unit.Id, unit.Key, unit.State, woods, unit.StartTime, unit.EndTime));

        }


        /// <summary>
        /// Compares Unit DateTime to wanted DateTime
        /// </summary>
        /// <param name="unit">Unit to compare</param>
        /// <returns>True if Year:Month:Day is Equal, False otherwise</returns>
        private bool CompareDates(Unit unit)
        {
            if (unit.EndTime.Year.Equals(Date.Year) &&
               unit.EndTime.Month.Equals(Date.Month) &&
               unit.EndTime.Day.Equals(Date.Day))
                return true;
            return false;
        }

        /// <summary>
        /// Checks if unit has wanted wood
        /// </summary>
        /// <param name="unit">Unit we want to search</param>
        /// <param name="woodList">Wood type we want to search for</param>
        /// <returns>True if found/False if not found</returns>
        private ObservableCollection<Wood> CheckUnitForWood(Unit unit, List<Wood> woodList)
        {
            ObservableCollection<Wood> returnList = new ObservableCollection<Wood>();
            foreach (Wood wood in woodList)
            {
                foreach (Wood unitWood in unit.Woods)
                {
                    if (unitWood.Height == wood.Height && unitWood.Width == wood.Width && unitWood.Depth == wood.Depth)
                        returnList.Add(unitWood);
                }
            }
            return returnList;
        }

        /// <summary>
        /// Formats textbox input to Wood list
        /// </summary>
        /// <returns>Wood list</returns>
        private List<Wood> WoodList()
        {
            // create new temporary list
            List<Wood> returnList = new List<Wood>();

            // Check if search is empty
            if (string.IsNullOrWhiteSpace(labelText))
            {
                ErrorText = badInputSize;
                return returnList;
            }

            // Split and get all wanted sizes
            string[] woodSizes = LabelText.Split(' ');

            FilterAndAddWood(returnList, woodSizes);

            return returnList;
        }

        /// <summary>
        /// Parses written input to Wood object 
        /// </summary>
        /// <param name="newList">Wood list</param>
        /// <param name="woodSizes">Wood parameters/input</param>
        private void FilterAndAddWood(List<Wood> newList, string[] woodSizes)
        {
            foreach (string size in woodSizes)
            {
                try
                {
                    // Split and get all wanted single wood parameters
                    string[] woodParams = size.Split('x');

                    // White space skip
                    if (woodParams[0] == "")
                    {
                        continue;
                    }

                    // Bad param count
                    if (woodParams.Length < 3)
                        ErrorText = smallParamCount;
                    else if (woodParams.Length > 3)
                        ErrorText = bigParamCount;
                    else
                    {
                        // Check if we can get all parameters
                        if (!double.TryParse(woodParams[0].Replace(',', '.'), out double height) ||
                            !double.TryParse(woodParams[1].Replace(',', '.'), out double width) ||
                            !double.TryParse(woodParams[2].Replace(',', '.'), out double depth))
                            ErrorText = badParams;
                        else
                        {
                            Wood temp = new Wood(0, height, width, depth, 0, 0);
                            newList.Add(temp);
                        }
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Failed to Parse and Add Input Wood. Error: {ex}");
                }

                
            }
        }
        /// <summary>
        /// Gets or Sets text of search textbox
        /// </summary>
        public string LabelText
        {
            get { return labelText; }
            set
            {
                labelText = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or Sets boolean value of State checkbox
        /// </summary>
        public bool StateCheck
        {
            get { return stateCheck; }
            set
            {
                stateCheck = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or Sets boolean value of Date checkbox
        /// </summary>
        public bool DateCheck
        {
            get { return dateCheck; }
            set
            {
                dateCheck = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or Sets value of State
        /// </summary>
        public string State
        {
            get { return state; }
            set
            {
                state = value;
                NotifyPropertyChanged("State");
            }
        }

        /// <summary>
        /// Gets or Sets value of Date
        /// </summary>
        public DateTime Date
        {
            get { return date; }
            set
            {
                date = value;
                NotifyPropertyChanged();
            }
        }
        /// <summary>
        /// Gets or Sets value of ErrorText
        /// </summary>
        public string ErrorText
        {
            get { return errorText; }
            set
            {
                errorText = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// INotify event
        /// </summary>
        /// <param name="propertyName">Property name</param>
        private void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

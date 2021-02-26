using System.Collections.ObjectModel;

namespace Warehouse.viewModel
{

    class MainViewModel
    {
        // Main Unit list
        public ObservableCollection<Unit> Units { get; set; }

        private readonly DataAccess db = new DataAccess();

        /// <summary>
        /// Creates new MainViewModel object
        /// </summary>
        /// <param name="units">Unit list</param>
        public MainViewModel(ObservableCollection<Unit> units)
        {
            Units = units;

            // Adds all Units first load
            ObservableCollection<Unit> AllUnits = db.GetAllUnits();
            if (AllUnits.Count > 0)
                foreach (Unit unit in AllUnits)
                    Units.Add(unit);
        }
    }
}

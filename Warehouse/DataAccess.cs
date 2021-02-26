using System;
using Dapper;
using System.Data;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.SQLite;

namespace Warehouse
{
    class DataAccess
    {
        /// <summary>
        /// Main connection string
        /// </summary>
        /// <param name="id">DB id</param>
        /// <returns>connection string</returns>
        private static string ConnectionString(string id = "Dziovyklos")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

        /// <summary>
        /// Returns all Woods by ID
        /// </summary>
        /// <param name="id">Wood ID</param>
        /// <returns>Wood list</returns>
        public ObservableCollection<Wood> GetWoodsById(int id)
        {
            ObservableCollection<Wood> returnList = new ObservableCollection<Wood>();

            DataTable dt = new DataTable();

            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString()))
            {
                SQLiteCommand cmd = new SQLiteCommand($"Select Wood.id, Wood.Height, Wood.Width, Wood.Depth, Wood.Packages, Wood.Singles, Wood.WoodType, Wood.Note FROM Wood WHERE Wood.fk_Unit = @Id", connection);
                cmd.Parameters.Add(new SQLiteParameter("@Id", id));
                SQLiteDataAdapter mda = new SQLiteDataAdapter(cmd);
                mda.Fill(dt);
            }

            foreach(DataRow row in dt.Rows)
            {
                Wood wood = new Wood(Convert.ToInt32(row["id"]),Convert.ToDouble(row["Height"]), Convert.ToDouble(row["Width"]), Convert.ToDouble(row["Depth"]), Convert.ToInt32(row["Packages"]), Convert.ToInt32(row["Singles"]), Convert.ToString(row["WoodType"]), Convert.ToString(row["Note"]));
                returnList.Add(wood);

            }


            return returnList;
        }

        /// <summary>
        /// Returns all units 
        /// </summary>
        /// <returns>Unit list</returns>
        public ObservableCollection<Unit> GetAllUnits()
        {
            ObservableCollection<Unit> returnList = new ObservableCollection<Unit>();

            DataTable dt = new DataTable();

            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString()))
            { 
                SQLiteCommand cmd = new SQLiteCommand($"Select Unit.id, Unit.Key, Unit.State, Unit.StartTime, Unit.EndTime FROM Unit", connection);
                SQLiteDataAdapter mda = new SQLiteDataAdapter(cmd);
                mda.Fill(dt);
            }

            foreach(DataRow row in dt.Rows)
            {
                int id = Convert.ToInt32(row["id"]);
                Unit unit = new Unit(id, Convert.ToInt32(row["Key"]), Convert.ToString(row["State"]), GetWoodsById(id), Convert.ToDateTime(row["StartTime"]), Convert.ToDateTime(row["EndTime"]));
                returnList.Add(unit);
            }

            return returnList;
        }

        /// <summary>
        /// Adds empty Unit
        /// </summary>
        /// <param name="key">Unit Key</param>
        /// <returns>1 or more if success/0 or less otherwise</returns>
        public int AddEmptyUnit(int key)
        {
            using (IDbConnection connection = new SQLiteConnection(ConnectionString()))
            {
                int result = connection.Execute("INSERT INTO Unit (State, StartTime, EndTime, Key) VALUES (@State,@StartTime,@EndTime,@Key)", new { State = UnitStates.Tuscia, StartTime = DateTime.Now, EndTime = DateTime.Now, Key = key });
                return result;
            }
        }

        /// <summary>
        /// Adds Unit
        /// </summary>
        /// <param name="key">Unit Key</param>
        /// <param name="state">Unit State</param>
        /// <param name="startTime">Unit Start Time</param>
        /// <param name="endTime">Unit End Time</param>
        /// <returns>1 or more if success/0 or less if failed</returns>
        public int AddUnit(int key, string state, DateTime startTime, DateTime endTime)
        {
            using (IDbConnection connection = new SQLiteConnection(ConnectionString()))
            {
                int result = connection.Execute("INSERT INTO Unit (State, StartTime, EndTime, Key) VALUES (@State,@StartTime,@EndTime,@Key)", new { State = state, StartTime = startTime, EndTime = endTime, Key = key });
                return result;
            }
        }

        /// <summary>
        /// Gets Unit by Key
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Unit</returns>
        public Unit GetUnitByKey(int key)
        {
            DataTable dt = new DataTable();
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString()))
            {
                SQLiteCommand cmd = new SQLiteCommand($"Select Unit.id, Unit.Key, Unit.State, Unit.StartTime, Unit.EndTime FROM Unit WHERE Unit.Key = @key", connection);
                cmd.Parameters.Add(new SQLiteParameter("@key", key));
                SQLiteDataAdapter mda = new SQLiteDataAdapter(cmd);
                mda.Fill(dt);
            }
            int id = Convert.ToInt32(dt.Rows[0]["id"]);
            Unit unit = new Unit(id, Convert.ToInt32(dt.Rows[0]["Key"]),Convert.ToString(dt.Rows[0]["State"]),GetWoodsById(id), Convert.ToDateTime(dt.Rows[0]["StartTime"]),Convert.ToDateTime(dt.Rows[0]["EndTime"]));
            return unit;
        }


        /// <summary>
        /// Adds Wood to Unit by Unit ID
        /// </summary>
        /// <param name="height">Wood Height</param>
        /// <param name="width">Wood Width</param>
        /// <param name="depth">Wood Depth</param>
        /// <param name="packages">Wood Packages</param>
        /// <param name="singles">Wood Singles</param>
        /// <param name="type">Wood Type</param>
        /// <param name="note">Wood Note</param>
        /// <param name="fk_unit">Wood foreign key/Unit id</param>
        /// <returns></returns>
        public int AddWoodToUnit(double height, double width, double depth, int packages, int singles, string type, string note, int fk_unit)
        {
            using (IDbConnection connection = new SQLiteConnection(ConnectionString()))
            {
                int result = connection.Execute("INSERT INTO Wood (Height, Width, Depth, Packages, Singles, WoodType, Note, fk_Unit) VALUES (@Height,@Width,@Depth,@Packages,@Singles,@Type,@Note,@fk_Unit)", new { Height = height, Width = width, Depth = depth, Packages = packages, Singles = singles, Type = type, Note = note ,fk_Unit = fk_unit});
                return result;
            }
        }

        /// <summary>
        /// Deletes Unit by ID
        /// </summary>
        /// <param name="id">Unit ID</param>
        /// <returns>1 or more if success/ 0 or less if failed</returns>
        public int DeleteUnitByID(int id)
        {
            using (IDbConnection connection = new SQLiteConnection(ConnectionString()))
            {
                int result = connection.Execute("DELETE FROM Wood WHERE fk_Unit = @ID", new { ID = id });

                if (result >= 0)
                    result = connection.Execute("DELETE FROM Unit WHERE id = @ID", new { ID = id });

                return result;
            }
        }

        /// <summary>
        /// Deletes All Units
        /// </summary>
        /// <returns>1 or more if success/0 or less if failed</returns>
        public int DeleteAll()
        {
            using (IDbConnection connection = new SQLiteConnection(ConnectionString()))
            {
                int result = connection.Execute("DELETE FROM Wood");

                if (result >= 0)
                    result = connection.Execute("DELETE FROM Unit");

                return result;
            }
        }

        /// <summary>
        /// Deletes Wood by ID
        /// </summary>
        /// <param name="id">Wood ID</param>
        /// <returns>1 or more if success/0 or less if failed</returns>
        public int DeleteWoodByID(int id)
        {
            using (IDbConnection connection = new SQLiteConnection(ConnectionString()))
            {
                int result = connection.Execute("DELETE FROM Wood WHERE id = @ID", new { ID = id });
                return result;
            }
        }

        /// <summary>
        /// Removes all Wood from Unit by ID
        /// </summary>
        /// <param name="id">Unit ID</param>
        /// <returns>1 or more if success/0 or less if failed</returns>
        public int ClearUnitByID(int id)
        {
            using (IDbConnection connection = new SQLiteConnection(ConnectionString()))
            {
                int result = connection.Execute("DELETE FROM Wood WHERE fk_Unit = @ID", new { ID = id });
                return result;
            }
        }

        /// <summary>
        /// Updates Unit State by ID
        /// </summary>
        /// <param name="id">Unit ID</param>
        /// <param name="state">Unit new State</param>
        /// <returns>1 or more if success/0 or less if failed</returns>
        public int UpdateUnitStateByID(int id, string state) 
        {
            using (IDbConnection connection = new SQLiteConnection(ConnectionString())) 
            {
                int result = connection.Execute("UPDATE Unit SET State = @State WHERE id = @ID", new {State = state, ID = id });
                return result;
            }
        }

        /// <summary>
        /// Updates Unit Start and End Times by ID
        /// </summary>
        /// <param name="id">Unit ID</param>
        /// <param name="startTime">Unit Start Time</param>
        /// <param name="endTime">Unit End Time</param>
        /// <returns>1 or more if success/0 or less if failed</returns>
        public int UpdateUnitTimeByID(int id, DateTime startTime, DateTime endTime)
        {
            using (IDbConnection connection = new SQLiteConnection(ConnectionString()))
            {
                int result = connection.Execute("UPDATE Unit SET StartTime = @StartTime, EndTime = @EndTime WHERE id = @ID", new { StartTime = startTime, EndTime = endTime, ID = id });
                return result;
            }
        }

    }
}

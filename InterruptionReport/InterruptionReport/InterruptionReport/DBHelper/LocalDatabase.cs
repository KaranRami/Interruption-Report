using InterruptionReport.Model.DBModel;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterruptionReport.DBHelper
{
    public class SqliteDateComparer : IComparer<string>
    {
        public int Compare(string s1, string s2) => DateTime.Compare(DateTime.Parse(s1), DateTime.Parse(s2));
    }
    public class LocalDatabase
    {
        readonly SQLiteAsyncConnection database;
        public readonly string Path;
        public LocalDatabase(string dbPath)
        {
            Path = dbPath;
            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<InterruptionDbModel>().Wait();
        }

        public async Task<List<InterruptionDbModel>> GetItemsAsync(DateTime fromDate, DateTime toDate, string subStation, string feeder, string interruptionType)
        {
            if (fromDate != null && toDate != null)
            {
                //AND([ReportedDate] <= '{1}' AND[ReportTimeFrom] < '{2}')
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(string.Format("SELECT * FROM [InterruptionDbModel] WHERE ([ReportedDate] = '{0}' AND [ReportTimeFrom] >= '{2}') OR [ReportedDate] > '{0}' INTERSECT SELECT * FROM [InterruptionDbModel] WHERE ([ReportedDate] = '{1}' AND[ReportTimeFrom] < '{2}') OR [ReportedDate] < '{1}'", fromDate.AddDays(-1).ToString("yyyy-MM-dd"), toDate.ToString("yyyy-MM-dd"), new TimeSpan(16, 0, 0)));
                if (subStation != "All Substations")
                {
                    stringBuilder.Append(string.Format(" AND [SubStation] = '{0}'", subStation));
                }
                if (feeder != "All Feeders")
                {
                    stringBuilder.Append(string.Format(" AND [Feeder] = '{0}'", feeder));
                }
                if (!string.IsNullOrEmpty(interruptionType))
                {
                    stringBuilder.Append(string.Format(" AND [InterruprionType] = '{0}'", interruptionType));
                }
                List<InterruptionDbModel> response = await database.QueryAsync<InterruptionDbModel>(stringBuilder.ToString());
                return response.OrderBy(v => v.ReportedDate, new SqliteDateComparer()).ToList();
            }
            else
            {
                List<InterruptionDbModel> response = await database.Table<InterruptionDbModel>().ToListAsync();
                return response.OrderBy(v => v.ReportedDate, new SqliteDateComparer()).ToList();
            }
        }
        //public Task<List<InterruptionDbModel>> GetItemsNotDoneAsync()
        //{
        //    return database.QueryAsync<InterruptionDbModel>("SELECT * FROM [InterruptionDbModel] WHERE [Done] = 0");
        //}

        public Task<InterruptionDbModel> GetItemAsync(long id)
        {
            return database.Table<InterruptionDbModel>().Where(i => i.ID == id).FirstOrDefaultAsync();
        }

        public Task<int> SaveItemAsync(InterruptionDbModel item)
        {
            return database.InsertAsync(item);
        }

        public Task<int> DeleteItemAsync(InterruptionDbModel item)
        {
            return database.DeleteAsync(item);
        }
        public async Task<bool> CustomQueryAsync(string query)
        {
            await database.QueryAsync<InterruptionDbModel>(query);
            return true;
        }
    }
}

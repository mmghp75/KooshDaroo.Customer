using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using System.Threading.Tasks;
using System.IO;
using KooshDaroo.Models;

namespace KooshDaroo.Data
{
    class KooshDarooDatabase
    {
        //Define SQLLite Database
        readonly SQLiteAsyncConnection database;
        private string databaseFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "KooshDaroo.db3");

        public KooshDarooDatabase()
        {
            //CreateDatabaseAndTables();
            database = new SQLiteAsyncConnection(databaseFilePath);
            database.CreateTableAsync<tblMember>().Wait();
        }
        public KooshDarooDatabase(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<tblMember>().Wait();
        }
        public Task<List<tblMember>> GetItemsAsync()
        {
            return database.Table<tblMember>().ToListAsync();
        }
        public Task<tblMember> GetItemAsync(string phoneno)
        {
            return database.Table<tblMember>().Where(i => i.PhoneNo == phoneno).FirstOrDefaultAsync();
        }
        public Task<int> SaveItemAsync(tblMember item)
        {
            if (item.id != 0)
            {
                return database.UpdateAsync(item);
            }
            else
            {
                return database.InsertAsync(item);
            }
        }
        public Task<int> DeleteItemAsync(tblMember item)
        {
            return database.DeleteAsync(item);
        }
        //public void CreateDatabaseAndTables()
        //{
        //    if (File.Exists(databaseFilePath))
        //        return;

        //    database.CreateTableAsync<tblMember>();
        //}
    }
}

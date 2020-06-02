using System;
using LinqToDB;
using LinqToDB.Data;
using GraphQLCSharpExample.Model;
using Microsoft.Extensions.Logging;

namespace GraphQLCSharpExample.DataAccess.Database
{
    /*
     * This demo uses the embedded SQLite.
     * This database instance must be singleton object which is shared by the whole app,
     * otherwise, different module will see different databases.
     * 
     * This class is registered as singleton object in Startup.cs
     * don't do this if the database is not SQLite
     */
    public class SingletonSQLiteDb : DataConnection
    {
        private ILogger<SingletonSQLiteDb> logger;

        public SingletonSQLiteDb(
            ILogger<SingletonSQLiteDb> logger
        ) : base(
            dataProvider: new LinqToDB.DataProvider.SQLite.SQLiteDataProvider(),
            connectionString: "Data Source =:memory: "
        ) {
            this.logger = logger;

            TurnTraceSwitchOn();
            WriteTraceLine =
                (message, displayName) =>
                { 
                    logger.LogInformation($"{message} {displayName}"); 
                };
        }

        public ITable<Department> Departments => GetTable<Department>();

        public ITable<Employee> Employees => GetTable<Employee>();
    }
}

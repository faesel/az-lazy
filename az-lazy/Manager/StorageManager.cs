using System;
using System.Collections.Generic;
using az_lazy.Model;
using LiteDB;

namespace az_lazy.Manager
{
    public interface ILocalStorageManager
    {
        void AddConnection(string connectionName, string connectionString);
         List<Connection> GetConnections();
    }

    public class LocalStorageManager : ILocalStorageManager
    {
        private readonly string ConnectionCollection = @$"{Environment.CurrentDirectory}\connections.db";

        public void AddConnection(string connectionName, string connectionString)
        {
            using var db = new LiteDatabase(ConnectionCollection);
            var collection = db.GetCollection<Connection>(nameof(ModelNames.Connection));
            var connection = new Connection(connectionName, connectionString, false);

            collection.Insert(connection);
            collection.EnsureIndex(x => x.ConnectionName);
        }

        public List<Connection> GetConnections()
        {
            using var db = new LiteDatabase(ConnectionCollection);
            var collection = db.GetCollection<Connection>(nameof(ModelNames.Connection));

            return collection.Query()
                .ToList();
        }
    }
}
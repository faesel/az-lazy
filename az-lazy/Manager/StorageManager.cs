using System;
using System.Collections.Generic;
using az_lazy.Model;
using LiteDB;

namespace az_lazy.Manager
{
    public interface ILocalStorageManager
    {
        void AddConnection(string connectionName, string connectionString);
        void SelectConnection(string connectionName);
        void RemoveConnection(string connectionName);
        List<Connection> GetConnections();
    }

    public class LocalStorageManager : ILocalStorageManager
    {
        private readonly string ConnectionCollection = @$"{Environment.CurrentDirectory}\connections.db";

        public void AddConnection(string connectionName, string connectionString)
        {
            using var db = new LiteDatabase(ConnectionCollection);
            var collection = db.GetCollection<Connection>(nameof(ModelNames.Connection));
            var connection = new Connection(connectionName, connectionString);

            collection.Insert(connection);
            collection.EnsureIndex(x => x.ConnectionName);
            collection.EnsureIndex(x => x.IsSelected);
        }

        public List<Connection> GetConnections()
        {
            using var db = new LiteDatabase(ConnectionCollection);
            var collection = db.GetCollection<Connection>(nameof(ModelNames.Connection));

            return collection.Query()
                .ToList();
        }

        public void SelectConnection(string connectionName)
        {
            using var db = new LiteDatabase(ConnectionCollection);
            var collection = db.GetCollection<Connection>(nameof(ModelNames.Connection));

            var selectedConnection = collection.FindOne(x => x.IsSelected);
            selectedConnection.SetUnselected();

            collection.Update(selectedConnection);

            var connectionToSelect = collection.FindOne(x => x.ConnectionName.Equals(connectionName, StringComparison.InvariantCultureIgnoreCase));
            connectionToSelect.SetSelected();

            collection.Update(connectionToSelect);
        }

        public void RemoveConnection(string connectionName)
        {
            using var db = new LiteDatabase(ConnectionCollection);
            var collection = db.GetCollection<Connection>(nameof(ModelNames.Connection));

            var connectionToRemove = collection.FindOne(x => x.ConnectionName.Equals(connectionName, StringComparison.InvariantCultureIgnoreCase));

            //TODO: This needs work
            collection.Delete(new BsonValue(connectionName));
        }
    }
}
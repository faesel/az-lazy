using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using az_lazy.Model;
using LiteDB;

namespace az_lazy.Manager
{
    public interface ILocalStorageManager
    {
        void AddConnection(string connectionName, string connectionString);
        void AddDevelopmentConnection(ILiteCollection<Connection> connectionCollection);
        bool SelectConnection(string connectionName);
        bool RemoveConnection(string connectionName);
        List<Connection> GetConnections();
        Connection GetSelectedConnection();
    }

    public class LocalStorageManager : ILocalStorageManager
    {
        private const string DevConnectionName = "devStorage";
        private const string DevConnectionString = "UseDevelopmentStorage=true";

        private readonly string ConnectionCollection = @$"{Environment.CurrentDirectory}\connections.db";

        public void AddConnection(string connectionName, string connectionString)
        {
            using var db = new LiteDatabase(ConnectionCollection);
            var collection = db.GetCollection<Connection>(nameof(ModelNames.Connection));
            var connection = new Connection(connectionName, connectionString);
                
            if(collection.Count() == 0)
            {
                AddDevelopmentConnection(collection);
            }

            collection.Insert(connection);
            collection.EnsureIndex(x => x.ConnectionName);
            collection.EnsureIndex(x => x.IsSelected);
        }

        public void AddDevelopmentConnection(ILiteCollection<Connection> connectionCollection)
        {
            var developmentStorage = new Connection(DevConnectionName, DevConnectionString);
            developmentStorage.SetDevelopmentStorage();

            if (connectionCollection == null)
            {
                using var db = new LiteDatabase(ConnectionCollection);
                connectionCollection = db.GetCollection<Connection>(nameof(ModelNames.Connection));
                connectionCollection.Insert(developmentStorage);
            }
            else 
            {
                connectionCollection.Insert(developmentStorage);
            }
        }

        public List<Connection> GetConnections()
        {
            using var db = new LiteDatabase(ConnectionCollection);
            var collection = db.GetCollection<Connection>(nameof(ModelNames.Connection));

            return collection.Query()
                .ToList();
        }

        public Connection GetSelectedConnection()
        {
            using var db = new LiteDatabase(ConnectionCollection);
            var collection = db.GetCollection<Connection>(nameof(ModelNames.Connection));

            return collection.FindOne(x => x.IsSelected);
        }

        public bool SelectConnection(string connectionName)
        {
            using var db = new LiteDatabase(ConnectionCollection);
            var collection = db.GetCollection<Connection>(nameof(ModelNames.Connection));

            var connectionToSelect = collection.FindOne(x => x.ConnectionName.Equals(connectionName, StringComparison.InvariantCultureIgnoreCase));
           
            if (connectionToSelect != null)
            {
                var selectedConnection = collection.FindOne(x => x.IsSelected);

                if (selectedConnection != null)
                {
                    selectedConnection.SetUnselected();
                    collection.Update(selectedConnection);
                }

                connectionToSelect.SetSelected();
                collection.Update(connectionToSelect);

                return true;
            }

            return false;
        }

        public bool RemoveConnection(string connectionName)
        {
            using var db = new LiteDatabase(ConnectionCollection);
            var collection = db.GetCollection<Connection>(nameof(ModelNames.Connection));

            var connectionToRemove = collection.FindOne(x => x.ConnectionName.Equals(connectionName, StringComparison.InvariantCultureIgnoreCase));

            if (!connectionToRemove.IsDevelopmentStorage)
            {
                collection.Delete(new BsonValue(connectionToRemove.Id));

                return true;
            }

            return false;
        }
    }
}
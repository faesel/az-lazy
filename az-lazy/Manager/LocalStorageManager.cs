using System.Linq;
using System.IO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using az_lazy.Model;
using LiteDB;
using System.Reflection;

namespace az_lazy.Manager
{
    public interface ILocalStorageManager
    {
        void AddConnection(string connectionName, string connectionString, bool selectConnection = false);
        void AddDevelopmentConnection();
        bool SelectConnection(string connectionName);
        bool RemoveConnection(string connectionName);
        bool RemoveAllConnections(string connectionName);
        List<Connection> GetConnections();
        Connection GetSelectedConnection();
    }

    public class LocalStorageManager : ILocalStorageManager
    {
        private const string DevConnectionName = "devStorage";
        private const string DevConnectionString = "UseDevelopmentStorage=true";
        private readonly string ConnectionCollection = @$"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\.dotnet\tools\.store\az-lazy\connections.db";

        public void AddConnection(string connectionName, string connectionString, bool selectConnection = false)
        {
            using var db = new LiteDatabase(ConnectionCollection);
            var collection = db.GetCollection<Connection>(nameof(ModelNames.Connection));
            var connection = new Connection(connectionName, connectionString);

            collection.Insert(connection);
            collection.EnsureIndex(x => x.ConnectionName);
            collection.EnsureIndex(x => x.IsSelected);

            if(selectConnection)
            {
                SelectConnection(connectionName);
            }
        }

        public void AddDevelopmentConnection()
        {
            var developmentStorage = new Connection(DevConnectionName, DevConnectionString);
            developmentStorage.SetSelected();
            developmentStorage.SetDevelopmentStorage();

            using var db = new LiteDatabase(ConnectionCollection);
            var collection = db.GetCollection<Connection>(nameof(ModelNames.Connection));

            var hasDevelopmentStorage = collection.Query()
                .Where(x => x.IsDevelopmentStorage)
                .Count() > 0;

            if(!hasDevelopmentStorage)
            {
                collection.Insert(developmentStorage);
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

            if (connectionToRemove?.IsDevelopmentStorage == false)
            {
                collection.Delete(new BsonValue(connectionToRemove.Id));

                return true;
            }

            return false;
        }

        public bool RemoveAllConnections(string connectionName)
        {
            using var db = new LiteDatabase(ConnectionCollection);
            var collection = db.GetCollection<Connection>(nameof(ModelNames.Connection));

            var connectionToRemove = collection
                .Find(x => !x.IsDevelopmentStorage).ToList();

            foreach(var connection in connectionToRemove)
            {
                collection.Delete(new BsonValue(connection.Id));
            }

            return true;
        }
    }
}
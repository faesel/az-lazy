using System;

namespace az_lazy.Model
{
    public class Connection
    {
        public Connection(
            string connectionName,
            string connectionString,
            bool isSelected)
        {
            this.ConnectionName = connectionName;
            this.ConnectionString = connectionString;
            this.DateAdded = DateTime.UtcNow;
            this.IsSelected = isSelected;

        }

        public string ConnectionName { get; set; }
        public string ConnectionString { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsSelected { get; set; }
    }
}
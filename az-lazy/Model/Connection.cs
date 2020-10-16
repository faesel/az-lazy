using System;

namespace az_lazy.Model
{
    public class Connection
    {
        public Connection(
            string connectionName,
            string connectionString)
        {
            this.Id = Guid.NewGuid();
            this.ConnectionName = connectionName;
            this.ConnectionString = connectionString;
            this.DateAdded = DateTime.UtcNow;
            this.IsSelected = false;
        }

        public void SetSelected()
        {
            this.IsSelected = true;
        }

        public void SetUnselected()
        {
            this.IsSelected = false;
        }

        public Guid Id { get; set; }
        public string ConnectionName { get; set; }
        public string ConnectionString { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsSelected { get; private set; }
    }
}
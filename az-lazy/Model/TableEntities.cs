using System.Collections.Generic;

namespace az_lazy.Model
{
    public class TableEntities
    {
        public List<string> ColumnNames { get; set; } = new List<string>();
        public List<TableRow> Rows { get; set; } = new List<TableRow>();
    }

    public class TableRow
    {
        public int One { get; set;}
        public string test { get; set; }
        public List<object> RowValues { get; set; } = new List<object>();
    }
} 
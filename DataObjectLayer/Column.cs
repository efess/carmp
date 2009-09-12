using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataObjectLayer
{
    public class Column
    {
        public String Name;
        public DataType Type;
        public Boolean Key;
        public Boolean AutoNumber;
        public Int32 Length;
        public Boolean AllowNull;
        public Boolean Set;

        public Column(String name, DataType type, Boolean key, Boolean autoNumber, Int32 length, Boolean allowNulls)
        {
            this.Name = name;
            this.Type = type;
            this.Key = key;
            this.AutoNumber = autoNumber;
            this.Length = length;
            this.AllowNull = allowNulls;
        }
    }

    public class Columns : List<Column>
    {
        public void ColumnSet(String name)
        {
            base.Find(delegate(Column col) { return col.Name == name; }).Set = true;
        }
        public List<Column> GetKeys()
        {
            return base.FindAll(delegate(Column col) { return col.Key || col.AutoNumber; });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataObjectLayer
{
    public enum DataType
    {
        String,
        Int,
        Long,
        Boolean,
        DateTime,
        Text,
        Blob
    }

    public enum DataSource
    {
        SQLite,
        XML,
        FlatFile
    }
}

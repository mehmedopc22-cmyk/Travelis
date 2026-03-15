using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.SqlClient;

namespace DAL.Interfaces
{
    public interface IFactory<T>
    {
        public SqlConnection GetConnection();
    }
}

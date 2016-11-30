using Capstone.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Web.Dal_s
{
    interface ITableDal
    {
        Table FindTable(int tableID);
        bool CreateTable(Table table, UserModel user);
        List<Table> GetAllTables();
    }
}

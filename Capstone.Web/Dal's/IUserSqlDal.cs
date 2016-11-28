using Capstone.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Web.Dal_s
{
    public interface IUserSqlDal
    {
        bool Register(UserModel user);
        List<string> GetAllUsernames();
        UserModel Login(string username, string password);

    }
}

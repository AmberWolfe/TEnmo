using System.Collections.Generic;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface IUserDAO
    {
        User GetUser(string username);
        User GetUserById(int userId);
        User AddUser(string username, string password);
        List<User> GetUsers();
        decimal GetUserBalance(int userId);
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.DAO;
using TenmoServer.Models;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private static IUserDAO userDao;

        public UserController(IUserDAO _userDao)
        {
            userDao = _userDao;
        }

        [HttpGet]
        public List<User> GetAllUsers()
        {
            return userDao.GetUsers();
        }

        [HttpGet("{userId}")]
        public User GetUserById(int userId)
        {
            return userDao.GetUserById(userId);
        }

    }
}

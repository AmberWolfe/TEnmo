using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.DAO;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class BalanceController : ControllerBase
    {
        private static IUserDAO userDao;

        public BalanceController(IUserDAO _userDao)
        {
            userDao = _userDao;
        }

        [HttpGet("{userId}")]
        public decimal GetBalance(int userId)
        {
            return userDao.GetUserBalance(userId);
        }

    }
}

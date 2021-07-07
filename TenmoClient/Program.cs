using System;
using System.Collections.Generic;
using TenmoClient.Data;

namespace TenmoClient
{
    public class Program
    {
        private static readonly ConsoleService consoleService = new ConsoleService();
        private static readonly AuthService authService = new AuthService();
        private static readonly BalanceService balanceService = new BalanceService();
        private static readonly TransferService transferService = new TransferService();

        static void Main(string[] args)
        {
            consoleService.Run(authService, balanceService, transferService);
        }       
    }
}


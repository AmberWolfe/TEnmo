using System;
using System.Collections.Generic;
using TenmoClient.Data;

namespace TenmoClient
{
    public class ConsoleService
    {
        private static readonly ConsoleService consoleService = new ConsoleService();
        private API_User AuthenticatedUser = null;
        /// <summary>
        /// Prompts for transfer ID to view, approve, or reject
        /// </summary>
        /// <param name="action">String to print in prompt. Expected values are "Approve" or "Reject" or "View"</param>
        /// <returns>ID of transfers to view, approve, or reject</returns>
        public int PromptForTransferID(string action)
        {
            Console.WriteLine("");
            Console.Write("Please enter transfer ID to " + action + " (0 to cancel): ");
            if (!int.TryParse(Console.ReadLine(), out int auctionId))
            {
                Console.WriteLine("Invalid input. Only input a number.");
                return 0;
            }
            else
            {
                return auctionId;
            }
        }

        public LoginUser PromptForLogin()
        {
            Console.Write("Username: ");
            string username = Console.ReadLine();
            string password = GetPasswordFromConsole("Password: ");

            LoginUser loginUser = new LoginUser
            {
                Username = username,
                Password = password
            };
            return loginUser;
        }

        private string GetPasswordFromConsole(string displayMessage)
        {
            string pass = "";
            Console.Write(displayMessage);
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                // Backspace Should Not Work
                if (!char.IsControl(key.KeyChar))
                {
                    pass += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                    {
                        pass = pass.Remove(pass.Length - 1);
                        Console.Write("\b \b");
                    }
                }
            }
            // Stops Receving Keys Once Enter is Pressed
            while (key.Key != ConsoleKey.Enter);
            Console.WriteLine("");
            return pass;
        }

        public void Run(AuthService authServiceIn, BalanceService balanceServiceIn, TransferService transferServiceIn)
        {
            AuthService authService = authServiceIn;
            BalanceService balanceService = balanceServiceIn;
            TransferService transferService = transferServiceIn;
            
            int menuSelection = -1;
            int loginRegister = -1;
            IntroText();
            do 
            {
                while ((loginRegister != 1 && loginRegister != 2) || menuSelection == 6)
                {
                    menuSelection = -1;  // reset variable so that the login loop is able to exit
                    consoleService.WriteloginHeader();                   
                    if (!int.TryParse(Console.ReadLine(), out loginRegister))
                    {
                        Console.WriteLine("Invalid input. Please enter only a number.");
                    }
                    else if (loginRegister == 1)
                    {
                        while (!UserService.IsLoggedIn()) //will keep looping until user is logged in
                        {
                            LoginUser loginUser = consoleService.PromptForLogin();
                            AuthenticatedUser = authService.Login(loginUser);
                            if (AuthenticatedUser != null)
                            {
                                UserService.SetLogin(AuthenticatedUser);
                            }
                        }
                    }
                    else if (loginRegister == 2)
                    {
                        bool isRegistered = false;
                        while (!isRegistered) //will keep looping until user is registered
                        {
                            LoginUser registerUser = consoleService.PromptForLogin();
                            isRegistered = authService.Register(registerUser);
                            if (isRegistered)
                            {
                                Console.WriteLine("");
                                Console.WriteLine("Registration successful. You can now log in.");
                                loginRegister = -1; //reset outer loop to allow choice for login
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid selection.");
                    }
                }
                menuSelection = MenuSelection(authService, balanceService, transferService, AuthenticatedUser);
            }  while (menuSelection != 0);  // loop until exit            
            Console.WriteLine("Goodbye!\n");
            Environment.Exit(0);
        }


        private static int MenuSelection(AuthService authServiceIn, BalanceService balanceServiceIn, TransferService transferServiceIn, API_User authUserInMenu)
        {
            AuthService authService = authServiceIn;
            BalanceService balanceService = balanceServiceIn;
            TransferService transferService = transferServiceIn;

            int userId = UserService.GetUserId();
            int menuSelection = -1;
            while (menuSelection != 0)
            {
                consoleService.WriteMenu();

                if (!int.TryParse(Console.ReadLine(), out menuSelection))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else if (menuSelection == 1)
                {
                    // view your current balance
                    Console.WriteLine("\nYour balance is:");
                    Console.WriteLine("$" + balanceService.GetBalance(userId, authUserInMenu));
                }
                else if (menuSelection == 2)
                {
                    // view all your past transfers and then allowing any individual transfer to be selected and the details of that transfer displayed (step 6 in ReadMe)
                    GetAllUsers get = new GetAllUsers();
                    Console.WriteLine("\nTransfer History:");
                    List<Transfer> transferList = transferService.GetTransfers(userId, authUserInMenu);
                    foreach (Transfer transfer in transferList)
                    {
                        Console.WriteLine($"Transfer ID: {transfer.TransferId} in the amount of ${transfer.Amount} was sent from {transfer.SenderUserName} to {transfer.ReceiverUserName}");
                    }
                    Console.WriteLine();
                    Console.WriteLine("To see the details of a transfer type the Transfer ID number.");
                    if (!int.TryParse(Console.ReadLine(), out int userInput))
                    {
                        Console.WriteLine("Invalid entry, try again");
                    }
                    else
                    {
                        bool isAValidTransferId = false;
                        foreach (Transfer tranfer in transferList)
                        {
                            if (userInput == tranfer.TransferId)
                            {
                                TransferDetails details = transferService.GetTransferDetails(userId, userInput, authUserInMenu);
                                Console.WriteLine($"Transfer ID: { details.TransferId}");
                                Console.WriteLine($"From: {details.SenderUserName}");
                                Console.WriteLine($"To: {details.ReceiverUserName}");
                                Console.WriteLine($"Type: {details.TransferTypeDescription}");
                                Console.WriteLine($"Status: {details.TransferStatusDescription}");
                                Console.WriteLine($"Amount: {details.Amount}\n");
                                isAValidTransferId = true;
                                System.Threading.Thread.Sleep(2000);
                            }                        
                        }
                        if (!isAValidTransferId)
                        {
                            Console.WriteLine("\nThe transfer ID you have selected does not exist.");
                        }
                    }
                }
                else if (menuSelection == 3)
                {
                    // view your pending requests (optional)
                    int userTransferInput;
                    int pendingTransferInput;
                    string senderUserName = "";
                    Console.WriteLine("\nPending Transfers:");
                    List<Transfer> transferList = transferService.GetTransfers(userId, authUserInMenu);
                    foreach (Transfer transfer in transferList)
                    {
                        if (transfer.TransferStatusId == 1)
                        {
                            senderUserName = transfer.SenderUserName;
                            Console.WriteLine($"Transfer ID: {transfer.TransferId} in the amount of ${transfer.Amount} is pending to be sent from {transfer.SenderUserName} to {transfer.ReceiverUserName}");
                        }
                    }
                    Console.WriteLine();

                    Console.WriteLine("To Approve or Reject a Pending Transfer To your account, Enter the Transfer ID number:");
                    if (!int.TryParse(Console.ReadLine(), out userTransferInput))
                    {
                        Console.WriteLine("Invalid entry, try again");
                    }

                    Console.WriteLine("1: Approve\n2: Reject\n0: Don't approve or reject\n---------\nPlease choose an option: ");
                    if (!int.TryParse(Console.ReadLine(), out pendingTransferInput))
                    {
                        Console.WriteLine("Invalid entry, try again");
                    }
                    //check if the user is allowed to Approve the Transfer
                    bool isAllowedToApprove = false;
                    bool isAValidTransferId = false;
                    Transfer foundTransfer = new Transfer();
                    foreach (Transfer transfer in transferList)
                    {
                        if (transfer.TransferId == userTransferInput)
                        {
                            foundTransfer = transfer;
                            isAValidTransferId = true;
                            string userNameThatIsLoggedIn = authUserInMenu.Username;
                            if (senderUserName == userNameThatIsLoggedIn)
                            {
                               isAllowedToApprove = true;
                            }                                                   
                        }
                    }
                    pendingTransferInput++;  // add one so that the user input matches the proper transter status code
                    int fromId = foundTransfer.AccountFrom;
                    int toId = foundTransfer.AccountTo;
                    decimal amount = foundTransfer.Amount;

                    //check if it is an approve or reject
                    if ((pendingTransferInput == 2 || pendingTransferInput == 3) && isAllowedToApprove)
                    {
                        bool isSuccessful = transferService.UpdateTransferRequest(fromId, toId, amount, userTransferInput, pendingTransferInput, authUserInMenu);
                        if (isSuccessful)
                        {
                            Console.WriteLine("\nRequest Sent!");
                        }
                    }                    
                    if (!isAValidTransferId)
                    {
                        Console.WriteLine("Invalid Transfer ID entry, try again");
                    }
                    else if (!isAllowedToApprove)
                    {
                        Console.WriteLine("You are not allowed to approve your own requests.");
                    }
                }
                else if (menuSelection == 4)
                {
                    // send TE bucks as long as enough TE bucks are in the account
                    GetAllUsers get = new GetAllUsers();
                    List<API_User> allUsers = get.Get(authUserInMenu);
                    int recipientId = 0;
                    consoleService.WriteHeaderForUserList();
                    foreach (API_User AuthenticatedUser in allUsers)
                    {
                        if (AuthenticatedUser.UserId != userId)
                        {
                            Console.WriteLine(AuthenticatedUser.UserId.ToString().PadRight(10) + AuthenticatedUser.Username.ToString().PadRight(10));
                        }
                    }
                    if (!int.TryParse(Console.ReadLine(), out recipientId))
                    {
                        Console.WriteLine("\nInvalid entry, try again");
                    }
                    else
                    {
                        foreach (API_User recipientUser in allUsers)
                        {
                            if (recipientUser.UserId == recipientId)
                            {
                                Console.WriteLine("\nYou have selected to give money to " + " " + recipientUser.Username);
                                Console.WriteLine("Enter the amount you want to transfer or type 0 to exit to menu: ");
                                if (decimal.TryParse(Console.ReadLine(), out decimal result))
                                {
                                    if (result == 0)
                                    {
                                        return 1;  // go back to the beginning of the main menu as a logged in user
                                    }
                                    else
                                    {
                                        bool isSuccessful = transferService.AddNewTransfer(userId, recipientId, result, authUserInMenu);
                                        if (isSuccessful)
                                        {
                                            Console.WriteLine("\nTransfer was successful. $" + result + " was sent to " + recipientUser.Username);
                                        }
                                        else
                                        {
                                            Console.WriteLine("Transfer failed due to insufficient balance.");
                                        }
                                        Console.WriteLine("Select 0 and hit enter to exit to the menu.");
                                        int userSelection = -1;
                                        while (userSelection != 0)
                                        {
                                            int.TryParse(Console.ReadLine(), out int selection);
                                            if (selection == 0)
                                            {
                                                userSelection = 0;
                                                {
                                                    return 1;  // go back to the beginning of the main menu as a logged in user
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (menuSelection == 5)
                {
                    // request TE bucks (optional)
                    GetAllUsers get = new GetAllUsers();
                    List<API_User> allUsers = get.Get(authUserInMenu);
                    int requesteeId = 0;
                    consoleService.WriteHeaderForUserList();
                    foreach (API_User AuthenticatedUser in allUsers)
                    {
                        if (AuthenticatedUser.UserId != userId)
                        {
                            Console.WriteLine(AuthenticatedUser.UserId.ToString().PadRight(10) + AuthenticatedUser.Username.ToString().PadRight(10));
                        }
                    }
                    Console.WriteLine("Who would you like to request TE bucks from?");
                    if (!int.TryParse(Console.ReadLine(), out requesteeId))
                    {
                        Console.WriteLine("\nInvalid entry, try again");
                    }
                    foreach (API_User requestee in allUsers)
                    {
                        if (requestee.UserId == requesteeId)
                        {
                            Console.WriteLine("\nYou have selected to request money from " + " " + requestee.Username);
                            Console.WriteLine("Enter the amount you want to request or type 0 to exit to menu: ");
                            if (decimal.TryParse(Console.ReadLine(), out decimal result))
                            {
                                if (result == 0)
                                {
                                    return 1;  // go back to the beginning of the main menu as a logged in user
                                }
                                else
                                {
                                    bool isSuccessful = transferService.AddNewTransferRequest(requesteeId, userId, result, authUserInMenu);
                                    if (isSuccessful)
                                    {
                                        Console.WriteLine("\nRequest for $" + result + " was sent to " + requestee.Username);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Request failed for some reason.");
                                    }
                                    Console.WriteLine("Select 0 and hit enter to exit to the menu.");
                                    int userSelection = -1;
                                    while (userSelection != 0)
                                    {
                                        int.TryParse(Console.ReadLine(), out int selection);
                                        if (selection == 0)
                                        {
                                            userSelection = 0;
                                            {
                                                return 1;  // go back to the beginning of the main menu as a logged in user
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }



                }
                else if (menuSelection == 6)
                {
                    Console.WriteLine("");
                    UserService.SetLogin(new API_User()); //wipe out previous login info
                }
                else
                {
                    return menuSelection;
                }
                return menuSelection;
            }
            return menuSelection;
        }

        
        private void WriteloginHeader()
        {
            Console.WriteLine("Welcome to TEnmo!");
            Console.WriteLine("1: Login");
            Console.WriteLine("2: Register");
            Console.Write("Please choose an option: ");
        }
        private void WriteMenu()
        {
            Console.WriteLine("\n-----------------------------------------");
            Console.WriteLine("Welcome to TEnmo! Please make a selection: ");
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine("1: View your current balance");
            Console.WriteLine("2: View your past transfers");
            Console.WriteLine("3: View your pending requests");
            Console.WriteLine("4: Send TE bucks");
            Console.WriteLine("5: Request TE bucks");
            Console.WriteLine("6: Log in as different user");
            Console.WriteLine("0: Exit");
            Console.WriteLine("---------");
            Console.Write("Please choose an option: ");
        }
        private void WriteHeaderForUserList()
        {
            Console.WriteLine("\nEnter User Id number for the user you would like to send money to:");
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("User ID".PadRight(10) + "Recipient Name".PadRight(10));
            Console.WriteLine("-----------------------------------");
        }

        private void IntroText()
        {
            Console.WriteLine("TTTTTTTTTTTTTTTTTTTTTTTEEEEEEEEEEEEEEEEEEEEEE");
            Console.WriteLine("T:::::::::::::::::::::TE::::::::::::::::::::E");
            Console.WriteLine("T:::::::::::::::::::::TE::::::::::::::::::::E");
            Console.WriteLine("T:::::TT:::::::TT:::::TEE::::::EEEEEEEEE::::E");
            Console.WriteLine("TTTTTT  T:::::T  TTTTTT  E:::::E       EEEEEEnnnn  nnnnnnnn       mmmmmmm    mmmmmmm      ooooooooooo");
            Console.WriteLine("        T:::::T          E:::::E             n:::nn::::::::nn   mm:::::::m  m:::::::mm  oo:::::::::::oo");
            Console.WriteLine("        T:::::T          E::::::EEEEEEEEEE   n::::::::::::::nn m::::::::::mm::::::::::mo:::::::::::::::o");
            Console.WriteLine("        T:::::T          E:::::::::::::::E   nn:::::::::::::::nm::::::::::::::::::::::mo:::::ooooo:::::o");
            Console.WriteLine("        T:::::T          E:::::::::::::::E     n:::::nnnn:::::nm:::::mmm::::::mmm:::::mo::::o     o::::o");
            Console.WriteLine("        T:::::T          E::::::EEEEEEEEEE     n::::n    n::::nm::::m   m::::m   m::::mo::::o     o::::o");
            Console.WriteLine("        T:::::T          E:::::E               n::::n    n::::nm::::m   m::::m   m::::mo::::o     o::::o");
            Console.WriteLine("        T:::::T          E:::::E       EEEEEE  n::::n    n::::nm::::m   m::::m   m::::mo::::o     o::::o");
            Console.WriteLine("      TT:::::::TT      EE::::::EEEEEEEE:::::E  n::::n    n::::nm::::m   m::::m   m::::mo:::::ooooo:::::o");
            Console.WriteLine("      T:::::::::T      E::::::::::::::::::::E  n::::n    n::::nm::::m   m::::m   m::::mo:::::::::::::::o");
            Console.WriteLine("      T:::::::::T      E::::::::::::::::::::E  n::::n    n::::nm::::m   m::::m   m::::m oo:::::::::::oo");
            Console.WriteLine("      TTTTTTTTTTT      EEEEEEEEEEEEEEEEEEEEEE  nnnnnn    nnnnnnmmmmmm   mmmmmm   mmmmmm   ooooooooooo\n");
        }
    }
}

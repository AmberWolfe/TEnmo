using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using TenmoServer.Models;
using TenmoServer.Security;
using TenmoServer.Security.Models;

namespace TenmoServer.DAO
{
    public class TransfersSqlDAO : ITransferDAO
    {
        private readonly string connectionString;

        public TransfersSqlDAO(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        /// <summary>
        /// Adds a transfer to the database given three inputs passed in as Transfer object:
        /// 1. UserFrom
        /// 2. UserTo
        /// 3. Amount
        /// </summary>
        /// <param name="transferToCreate"></param>
        /// <returns></returns>
        public Transfer CreateTransfer(Transfer transferToCreate)
        {
            int fromUserId = transferToCreate.UserFrom;
            int toUserId = transferToCreate.UserTo;
            decimal amount = transferToCreate.Amount;
            // Look up the account Ids for each User Id first
            int fromAccountId = LookUpUserAccountIdByUserId(fromUserId);
            int toAccountId = LookUpUserAccountIdByUserId(toUserId);
            // create and return the transfer
            return AddTransfer(fromAccountId, toAccountId, amount);
        }

        public Transfer CreateTransferRequest(Transfer transferToCreate)
        {
            int fromUserId = transferToCreate.UserFrom;
            int toUserId = transferToCreate.UserTo;
            decimal amount = transferToCreate.Amount;
            // Look up the account Ids for each User Id first
            int fromAccountId = LookUpUserAccountIdByUserId(fromUserId);
            int toAccountId = LookUpUserAccountIdByUserId(toUserId);
            // create and return the transfer
            return AddTransferRequest(fromAccountId, toAccountId, amount);
        }

        /// <summary>
        /// Retrieves all the transfers for an input User Id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>The transfer data from the transfer table</returns>
        public List<Transfer> GetAllTransfersByUserId(int userId)
        {
            List<Transfer> returnTransfers = new List<Transfer>();
            // first find the account id based on the user id
            string accountId = LookUpAccountIdByUserIdAndReturnString(userId);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();                   
                    // get the list of transfers for the user's account
                    SqlCommand command = new SqlCommand("SELECT DISTINCT transfer_id, transfer_type_id, transfer_status_id, account_from, account_to, amount FROM transfers AS t " +
                        "JOIN accounts AS a ON(" + accountId + "= t.account_from OR " + accountId + "= t.account_to) " +
                        "JOIN users AS u ON u.user_id = a.user_id", connection);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            returnTransfers.Add(ReadTransferFromReader(reader));
                        }
                    }
                }
            }
            catch (SqlException exception)
            {
                throw exception;
            }
            //return returnTransfers;
            List<Transfer> listOfTransfersWithAllDetails = AddTransferDetailsForEachTransfer(returnTransfers);
            return listOfTransfersWithAllDetails;
        }

        public string LookUpAccountIdByUserIdAndReturnString(int userId)
        {
            List<Transfer> returnTransfers = new List<Transfer>();
            string returnAccountId = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand getUserIdCommand = new SqlCommand("SELECT TOP 1 account_id FROM accounts WHERE user_id = @userId", connection);
                    getUserIdCommand.Parameters.AddWithValue("@userId", userId);
                    SqlDataReader readerForId = getUserIdCommand.ExecuteReader();
                    if (readerForId.HasRows && readerForId.Read())
                    {
                        returnAccountId = Convert.ToString(readerForId["account_id"]);
                    }                    
                }
            }
            catch (SqlException exception)
            {
                throw exception;
            }
            return returnAccountId;
        }

        /// <summary>
        /// Helper method to read a transfer
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>a transfer object read from one row of the database</returns>
        private Transfer ReadTransferFromReader(SqlDataReader reader)
        {
            Transfer transferJustRead = new Transfer()
            {
                TransferId = Convert.ToInt32(reader["transfer_id"]),
                TransferTypeId = Convert.ToInt32(reader["transfer_type_id"]),
                TransferStatusId = Convert.ToInt32(reader["transfer_status_id"]),
                AccountFrom = Convert.ToInt32(reader["account_from"]),
                AccountTo = Convert.ToInt32(reader["account_to"]),
                Amount = Convert.ToDecimal(reader["amount"])
            };
            return transferJustRead;
        }

        /// <summary>
        /// Retrieves all transfers in the database
        /// </summary>
        /// <returns>All the data from the transfer table</returns>
        public List<Transfer> GetAllTransfers()
        {
            List<Transfer> returnAllTransfers = new List<Transfer>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                   
                    SqlCommand command = new SqlCommand("SELECT * FROM transfers", connection);
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            returnAllTransfers.Add(ReadTransferFromReader(reader));
                        }
                    }
                }
            }
            catch (SqlException exception)
            {
                throw exception;
            }
            return AddTransferDetailsForEachTransfer(returnAllTransfers);
        }

        /// <summary>
        /// Retrieves the details of a transfer given an input transfer Id
        /// </summary>
        /// <param name="transferId"></param>
        /// <returns>The transfer details</returns>
        public Transfer GetTransfer(int transferId)
        {
            Transfer retrievedTransfer = new Transfer();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand("SELECT t.transfer_id, t.transfer_type_id, t.transfer_status_id, t.account_from, t.account_to, t.amount, ts.transfer_status_desc, tt.transfer_type_desc FROM transfers AS t " +
                        "JOIN transfer_statuses AS ts ON ts.transfer_status_id = t.transfer_status_id " +
                        "JOIN transfer_types AS tt ON tt.transfer_type_id = t.transfer_type_id WHERE(@transferId = t.transfer_id)", connection);
                    command.Parameters.AddWithValue("@transferId", transferId);
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows && reader.Read())
                    {
                        retrievedTransfer.TransferId = Convert.ToInt32(reader["transfer_id"]);
                        retrievedTransfer.TransferTypeId = Convert.ToInt32(reader["transfer_type_id"]);
                        retrievedTransfer.TransferStatusId = Convert.ToInt32(reader["transfer_status_id"]);
                        retrievedTransfer.AccountFrom = Convert.ToInt32(reader["account_from"]);
                        retrievedTransfer.AccountTo = Convert.ToInt32(reader["account_to"]);
                        retrievedTransfer.Amount = Convert.ToDecimal(reader["amount"]);
                        retrievedTransfer.TransferStatusDescription = Convert.ToString(reader["transfer_status_desc"]);      
                        retrievedTransfer.TransferTypeDescription = Convert.ToString(reader["transfer_type_desc"]);   
                    }
                    // close the connection and stop reading
                    connection.Close();
                    retrievedTransfer.SenderUserName = LookUpUserNameByAccountId(retrievedTransfer.AccountFrom);                    
                    retrievedTransfer.ReceiverUserName = LookUpUserNameByAccountId(retrievedTransfer.AccountTo);

                }
            }
            catch (SqlException exception)
            {
                throw exception;
            }
            return retrievedTransfer;
        }

        /// <summary>
        /// Helper method to return the User Name associated with an input Account Id
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns>User Name</returns>
        private string LookUpUserNameByAccountId(int accountId)
        {
            string userName = "";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand command = new SqlCommand("SELECT u.username FROM users As u JOIN accounts AS a ON a.user_id = u.user_id WHERE a.account_id = @accountId", conn);
                    command.Parameters.AddWithValue("@accountID", accountId);
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows && reader.Read())
                    {
                        userName = Convert.ToString(reader["username"]);
                    }
                }
            }
            catch (SqlException exception)
            {
                throw exception;
            }
            return userName;
        }
       
        /// <summary>
        /// Helper method to return the account balance for an input account Id
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns>balance in the account</returns>
        private decimal LookUpUserBalanceByAccountId(int accountId)
        {
            decimal returnUserBalance = 0M;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand command = new SqlCommand("SELECT balance FROM accounts WHERE account_id = @accountId", conn);
                    command.Parameters.AddWithValue("@accountID", accountId);
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows && reader.Read())
                    {
                        returnUserBalance = Convert.ToDecimal(reader["balance"]);
                    }
                }
            }
            catch (SqlException exception)
            {
                throw exception;
            }
            return returnUserBalance;
        }

        /// <summary>
        /// Helper method to retrieve the Account Id for an input User Id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Account Id</returns>
        private int LookUpUserAccountIdByUserId(int userId)
        {
            int accountId = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand command = new SqlCommand("SELECT account_id FROM accounts WHERE user_id = @userId", conn);
                    command.Parameters.AddWithValue("@userID", userId);
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows && reader.Read())
                    {
                        accountId = Convert.ToInt32(reader["account_id"]);
                    }
                }
            }
            catch (SqlException exception)
            {
                throw exception;
            }
            return accountId;
        }

        /// <summary>
        /// Inserts a new transfer into the database
        /// </summary>
        /// <param name="fromAccountId"></param>
        /// <param name="toAccountId"></param>
        /// <param name="amount"></param>
        /// <returns>The added transfer</returns>
        public Transfer AddTransfer(int fromAccountId, int toAccountId, decimal amount)
        {
            int transferId = 0;    
            
            try
            {
                
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("INSERT INTO transfers (transfer_type_id, transfer_status_id, account_from, account_to, amount) VALUES(2, 1, @fromAccountId, @toAccountId, @amount)", conn);
                    cmd.Parameters.AddWithValue("@fromAccountId", fromAccountId);
                    cmd.Parameters.AddWithValue("@toAccountId", toAccountId);
                    cmd.Parameters.AddWithValue("@amount", amount);
                    cmd.ExecuteNonQuery();

                    // pull out the new transfer_id
                    cmd = new SqlCommand("SELECT @@IDENTITY", conn);
                    transferId = Convert.ToInt32(cmd.ExecuteScalar());

                    // update status from pending to approved and then change the balances of both accounts, but only if the account_from balance is greater than amount
                    // first get the from user id

                    conn.Close();
                    conn.Open();

                    decimal fromUserBalance = LookUpUserBalanceByAccountId(fromAccountId);

                    // check that there is enough TE bucks in the account and also that the amount is positive, if not then do not execute the transfer and it will remain pending
                    int transferStatusId = 2;  // default to an approved transfer
                    // if the balance is too low or the amount is negative the transfer is rejected
                    if (fromUserBalance < amount || amount < 0M)
                    {
                        transferStatusId = 3;
                        amount = 0;
                    }
                        cmd = new SqlCommand("BEGIN TRANSACTION UPDATE accounts SET balance = (balance - @amount) WHERE account_id = @fromAccountId; " +
                            "UPDATE accounts SET balance = (balance + @amount) WHERE account_id = @toAccountId; " +
                            "UPDATE transfers SET transfer_status_id = @transferStatusId WHERE transfer_id = @transferId; COMMIT", conn);
                        cmd.Parameters.AddWithValue("@fromAccountId", fromAccountId);
                        cmd.Parameters.AddWithValue("@toAccountId", toAccountId);
                        cmd.Parameters.AddWithValue("@amount", amount);
                        cmd.Parameters.AddWithValue("@transferId", transferId);
                        cmd.Parameters.AddWithValue("@transferStatusId", transferStatusId);
                        cmd.ExecuteNonQuery();
                    if (amount == 0)
                    {
                        transferId = 0;  // if the transfer was not approved then return a null transfer
                    }
                }          
            }
            catch (SqlException exception)
            {
                 throw exception;
            }
            return GetTransfer(transferId);
        }

        /// <summary>
        /// Adds a transfer as a request with an initial status of pending
        /// </summary>
        /// <param name="fromAccountId"></param>
        /// <param name="toAccountId"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public Transfer AddTransferRequest(int fromAccountId, int toAccountId, decimal amount)
        {
            int transferId = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("INSERT INTO transfers (transfer_type_id, transfer_status_id, account_from, account_to, amount) VALUES(1, 1, @fromAccountId, @toAccountId, @amount)", conn);
                    cmd.Parameters.AddWithValue("@fromAccountId", fromAccountId);
                    cmd.Parameters.AddWithValue("@toAccountId", toAccountId);
                    cmd.Parameters.AddWithValue("@amount", amount);
                    cmd.ExecuteNonQuery();

                    // pull out the new transfer_id
                    cmd = new SqlCommand("SELECT @@IDENTITY", conn);
                    transferId = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (SqlException exception)
            {
                throw exception;
            }
            return GetTransfer(transferId);
        }
        /// <summary>
        /// Helper method to all the Transfer details to a transfer
        /// </summary>
        /// <param name="inputTransfer"></param>
        /// <returns>A transfer having all transfer details</returns>
        private Transfer RetrieveAllTransferDetails(Transfer inputTransfer)
        {
            int transferId = inputTransfer.TransferId;
            Transfer returnTransferWithAllDetails = GetTransfer(transferId);
            return returnTransferWithAllDetails;
        }
        /// <summary>
        /// Helper method to add all the transfer details for each transfer in a list of transfers
        /// </summary>
        /// <param name="inputListOfTransfers"></param>
        /// <returns>A list of transfers with all transfer details for each transfer</returns>
        private List<Transfer> AddTransferDetailsForEachTransfer(List<Transfer> inputListOfTransfers)
        {
            List<Transfer> returnListOfTransfers = new List<Transfer>();
            foreach (Transfer t in inputListOfTransfers)
            {
                returnListOfTransfers.Add(RetrieveAllTransferDetails(t));
            }
            return returnListOfTransfers;
        }

        public Transfer UpdateTransfer(int transferId, Transfer transferToUpdate)
        {
            bool isRejected = false;
            decimal amount = transferToUpdate.Amount;
            int transferStatusId = transferToUpdate.TransferStatusId;
            int fromAccountId = transferToUpdate.AccountFrom; //LookUpUserAccountIdByUserId(transferToUpdate.AccountFrom);  TODO delete these
            int toAccountId = transferToUpdate.AccountTo; //LookUpUserAccountIdByUserId(transferToUpdate.AccountTo);
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    decimal fromUserBalance = LookUpUserBalanceByAccountId(transferToUpdate.AccountFrom);
                    if (transferStatusId == 3)
                    {
                        isRejected = true;
                        amount = 0;
                    }

                    // check that there is enough TE bucks in the account and also that the amount is positive, if not then do not execute the transfer and it will remain pending
                    if ((fromUserBalance > amount && amount > 0M) || isRejected)
                    {
                        SqlCommand cmd = new SqlCommand("BEGIN TRANSACTION UPDATE accounts SET balance = (balance - @amount) WHERE account_id = @fromAccountId; " +
                            "UPDATE accounts SET balance = (balance + @amount) WHERE account_id = @toAccountId; " +
                            "UPDATE transfers SET transfer_status_id = @transferStatusId WHERE transfer_id = @transferId; COMMIT", conn);
                        cmd.Parameters.AddWithValue("@fromAccountId", fromAccountId);
                        cmd.Parameters.AddWithValue("@toAccountId", toAccountId);
                        cmd.Parameters.AddWithValue("@amount", amount);
                        cmd.Parameters.AddWithValue("@transferId", transferId);
                        cmd.Parameters.AddWithValue("@transferStatusId", transferStatusId);
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        transferId = 0;
                    }
                }
            }
            catch (SqlException exception)
            {
                throw exception;
            }
            return GetTransfer(transferId);





        }
    }
}

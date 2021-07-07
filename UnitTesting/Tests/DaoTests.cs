using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using TenmoServer.Models;

namespace TenmoServer.Tests
{
    public class DaoTests
    {
        protected Transfer testTransfer { get; set; } = new Transfer();
        protected string connectionString = @"Server=.\SQLEXPRESS;Database=tenmo;Trusted_Connection=True;";
        protected TransactionScope transaction;
        
        [TestInitialize]
        public void InitializeTest()
        {
            transaction = new TransactionScope();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    // check for first transfer
                    SqlCommand command = new SqlCommand("SELECT * FROM transfers WHERE transfer_id = @transferId", connection);
                    command.Parameters.AddWithValue("@transferId", 1);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        testTransfer.TransferId = 1;
                        testTransfer.TransferTypeId = Convert.ToInt32(reader["transfer_type_id"]);
                        testTransfer.TransferStatusId = Convert.ToInt32(reader["transfer_status_id"]);
                        testTransfer.AccountFrom = Convert.ToInt32(reader["account_from"]);
                        testTransfer.AccountTo = Convert.ToInt32(reader["account_to"]);
                        testTransfer.Amount = Convert.ToDecimal(reader["amount"]);
                    }
                    // if not there then create a first transfer
                    else
                    {
                        reader.Close();
                        command = new SqlCommand("INSERT INTO transfers (transfer_type_id, transfer_status_id, account_from, account_to, amount) OUTPUT INSERTED.transfer_id VALUES(2, 2, 1, 2, 250.00);", connection);
                        testTransfer.TransferId = Convert.ToInt32(command.ExecuteScalar());
                        testTransfer.TransferTypeId = 2;
                        testTransfer.TransferStatusId = 2;
                        testTransfer.AccountFrom = 1;
                        testTransfer.AccountTo = 2;
                        testTransfer.Amount = 250.00M;
                    }
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        [TestCleanup]
        public void CleanUp()
        {
            transaction.Dispose();
        }
    }
}

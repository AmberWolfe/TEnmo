using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using TenmoServer.DAO;
using TenmoServer.Models;
using TenmoServer.Tests;

namespace UnitTesting.Tests
{
    [TestClass]
    public class TransferDaoTests : DaoTests
    {
        [TestMethod]
        public void GetTransferTestForRetrievingOneTransfer()
        {
            TransfersSqlDAO transferDao = new TransfersSqlDAO(connectionString);

            Transfer transfer = transferDao.GetTransfer(testTransfer.TransferId);

            AssertTransfersMatch(testTransfer, transfer);
        }

        private void AssertTransfersMatch(Transfer expected, Transfer actual)
        {
            Assert.AreEqual(expected.TransferId, actual.TransferId);
            Assert.AreEqual(expected.TransferTypeId, actual.TransferTypeId);
            Assert.AreEqual(expected.TransferStatusId, actual.TransferStatusId);
            Assert.AreEqual(expected.AccountFrom, actual.AccountFrom);
            Assert.AreEqual(expected.AccountTo, actual.AccountTo);
            Assert.AreEqual(expected.Amount, actual.Amount);
        }

    }
}

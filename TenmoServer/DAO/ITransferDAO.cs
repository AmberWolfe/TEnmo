using System.Collections.Generic;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface ITransferDAO
    {
        Transfer AddTransfer(int fromId, int toId, decimal amount);
        Transfer GetTransfer(int transferId);

        List<Transfer> GetAllTransfersByUserId(int userId);

        List<Transfer> GetAllTransfers();

        Transfer CreateTransfer(Transfer transferToCreate);

        Transfer CreateTransferRequest(Transfer transferRequestToCreate);

        Transfer UpdateTransfer(int transferId, Transfer transferToUpdate);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Models
{
    public class Transfer
    {
        public int TransferId { get; set; }
        public int TransferTypeId { get; set; }
        public int TransferStatusId { get; set; }
        public int AccountFrom { get; set; }
        public int UserFrom { get; set; }
        public int AccountTo { get; set; }
        public int UserTo { get; set; }
        public decimal Amount { get; set; }
        public string TransferStatusDescription { get; set; }      // from the transfer_status table
        public string TransferTypeDescription { get; set; }    // from the transfer_types table
        public string SenderUserName { get; set; }
        public string ReceiverUserName { get; set; }
    }
}

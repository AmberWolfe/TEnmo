using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClient.Data
{
    public class Transfer
    {
        public int TransferId { get; set; }
        public int UserFrom { get; set; }
        public int UserTo { get; set; }
        public decimal Amount { get; set; }
        public string SenderUserName { get; set; }
        public string ReceiverUserName { get; set; }

        public int TransferStatusId { get; set; }
        public int AccountFrom {get; set;}
        public int AccountTo { get; set;}
    }

    public class TransferDetails
    {
        public int TransferId { get; set; }
        public string TransferStatusDescription { get; set; }
        public string TransferTypeDescription { get; set; }
        public string TransferStatus { get; set; }
        public int AccountFrom { get; set; }
        public int SenderUserId { get; set; }
        public string SenderUserName { get; set; }
        public int AccountTo { get; set; }
        public int ReceiverUserId { get; set; }
        public string ReceiverUserName { get; set; }
        public decimal Amount { get; set; }

    }
}

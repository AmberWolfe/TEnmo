using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Text;
using TenmoClient.Data;

namespace TenmoClient
{
    public class TransferService
    {
        string API_URL = "https://localhost:44315/transfer";
        RestClient client = new RestClient();
        ClientExceptions exceptions = new ClientExceptions();

        public List<Transfer> GetTransfers(int userId, API_User user) 
        {
            RestRequest request = new RestRequest(API_URL + "/" + userId);
            if (!string.IsNullOrWhiteSpace(user.Token))
            {
                client.Authenticator = new JwtAuthenticator(user.Token);
            }
            IRestResponse<List<Transfer>> response = client.Get<List<Transfer>>(request);
            exceptions.ExceptionCheck(response);
            return response.Data;
        }

        public bool AddNewTransfer(int userFrom, int userTo, decimal amount, API_User user)
        {
                Transfer newTransfer = new Transfer()
                {
                    UserFrom = userFrom,
                    UserTo = userTo,
                    Amount = amount
                };
                RestRequest request = new RestRequest(API_URL);
                request.AddJsonBody(newTransfer);
            if (!string.IsNullOrWhiteSpace(user.Token))
            {
                client.Authenticator = new JwtAuthenticator(user.Token);
            }
            IRestResponse<Transfer> response = client.Post<Transfer>(request);                
            if ((int)response.StatusCode == 400)
            {
                return false;
            }
            exceptions.ExceptionCheck(response);
            return true;
        }

        public TransferDetails GetTransferDetails(int userId, int transferId, API_User user)
        {
            RestRequest request = new RestRequest(API_URL + "/" + userId + "/" + transferId + "/details");
            if (!string.IsNullOrWhiteSpace(user.Token))
            {
                client.Authenticator = new JwtAuthenticator(user.Token);
            }
            IRestResponse<TransferDetails> response = client.Get<TransferDetails>(request);
            exceptions.Equals(response);
            return response.Data;
        }

        public bool AddNewTransferRequest(int userFrom, int userTo, decimal amount, API_User user)
        {
            Transfer newTransfer = new Transfer()
            {
                UserFrom = userFrom,
                UserTo = userTo,
                Amount = amount
            };
            RestRequest request = new RestRequest(API_URL + "/request");
            request.AddJsonBody(newTransfer);
            if (!string.IsNullOrWhiteSpace(user.Token))
            {
                client.Authenticator = new JwtAuthenticator(user.Token);
            }
            IRestResponse<Transfer> response = client.Post<Transfer>(request);
            if ((int)response.StatusCode  == 400)
            {
                return false;
            }
            exceptions.ExceptionCheck(response);
            return true;
        }

        public bool UpdateTransferRequest(int accountFrom, int accountTo, decimal amount, int transferID, int transferStatus, API_User user)
        {
            Transfer newTransfer = new Transfer()
            {
                AccountFrom = accountFrom,
                AccountTo = accountTo,
                Amount = amount,
                TransferId = transferID,
                TransferStatusId = transferStatus
            };
            RestRequest request = new RestRequest(API_URL + "/" + transferID);
            request.AddJsonBody(newTransfer);
            if (!string.IsNullOrWhiteSpace(user.Token))
            {
                client.Authenticator = new JwtAuthenticator(user.Token);
            }
            IRestResponse<Transfer> response = client.Put<Transfer>(request);
            exceptions.ExceptionCheck(response);
            return true;
        }
    }
}

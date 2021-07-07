using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Text;
using TenmoClient.Data;

namespace TenmoClient
{
    public class BalanceService
    {
        string API_URL = "https://localhost:44315/balance";
        RestClient client = new RestClient();
        ClientExceptions exceptions = new ClientExceptions();
        Account account = new Account();
        Transfer transfer = new Transfer();

        public decimal GetBalance(int userId, API_User user)
        {
            RestRequest request = new RestRequest(API_URL + "/" + userId);
            if (!string.IsNullOrWhiteSpace(user.Token))
            {
                client.Authenticator = new JwtAuthenticator(user.Token);
            }
            IRestResponse response = client.Get(request);
            exceptions.ExceptionCheck(response);
            string balance = response.Content;
            return decimal.Parse(balance);
        }
    }
}

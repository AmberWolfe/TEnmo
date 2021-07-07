using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Text;
using TenmoClient.Data;

namespace TenmoClient
{
    public class GetAllUsers
    {
        string API_URL = "https://localhost:44315/user";
        RestClient client = new RestClient();
        ClientExceptions exceptions = new ClientExceptions();
        public List<API_User> Get(API_User user)
        {
            RestRequest request = new RestRequest(API_URL);
            if (!string.IsNullOrWhiteSpace(user.Token))
            {
                client.Authenticator = new JwtAuthenticator(user.Token);
            }
            IRestResponse<List<API_User>> response = client.Get<List<API_User>>(request);
            exceptions.ExceptionCheck(response);
            return response.Data;
        }
    }
}

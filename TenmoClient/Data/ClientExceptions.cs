using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClient.Data
{
    public class ClientExceptions
    {
        //RestResponse response = new RestResponse();
        public void ExceptionCheck(IRestResponse response)
        {
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new Exception("The server could not be reached.");
            }
            if (!response.IsSuccessful)
            {
                if ((int)response.StatusCode == 401)
                {
                    throw new Exception("401: Unauthorized");
                }
                else if ((int)response.StatusCode == 403)
                {
                    throw new Exception("403: Forbidden");
                }
                else if ((int)response.StatusCode == 404)
                {
                    throw new Exception("404: Not Found");
                }
                else
                {
                    throw new Exception(response.StatusCode.ToString());
                }
            }
        }
    }
}

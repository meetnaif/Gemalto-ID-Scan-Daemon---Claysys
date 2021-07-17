using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using RestSharp;

namespace Gemalto_ID_Scan_Daemon___Claysys.Connectors
{
    public class IDScan_APIConnector
    {
        public static string IDScanAPI(string Base64Bitmap)
        {
            var APIURL = ConfigurationManager.AppSettings.Get("IDScan_LicenseParseAPI_URL");
            var AuthKey = ConfigurationManager.AppSettings.Get("AuthKey");
            var client = new RestClient(APIURL);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Cookie", "ASP.NET_SessionId=py0xanna1ijnv5tl1sxng11f");
            var body = @"{""authKey"":""AUTH_KEY"",""data"":""REQ_BODY""} ";
            body = body.Replace("AUTH_KEY", AuthKey).Replace("REQ_BODY", Base64Bitmap);
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response.Content;
        }
    }
}

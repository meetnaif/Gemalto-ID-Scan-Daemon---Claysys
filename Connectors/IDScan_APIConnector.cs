using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using RestSharp;
using Serilog;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Gemalto_ID_Scan_Daemon___Claysys.Connectors
{
    public class IDScan_APIConnector
    {
        public static string IDScanAPI(string Base64Bitmap)
        {
            //ServicePointManager.ServerCertificateValidationCallback +=(sender, certificate, chain, sslPolicyErrors) => true;
            //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            Log.Information("ID Scan API called.");
            var emptyresponse = "{\"ParseImageResult\":{\"DriverLicense\":null,\"ErrorMessage\":\"Cannot Parse ID\",\"Reference\":null,\"Success\":false,\"ValidationCode\":null}}";
            var APIURL = ConfigurationManager.AppSettings.Get("IDScan_LicenseParseAPI_URL");
            var AuthKey = Decryptor.DecryptText(ConfigurationManager.AppSettings.Get("AuthKey"));
            var client = new RestClient(APIURL);
            //client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            try
            {
                client.Timeout = 50000;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var body = @"{""authKey"":""AUTH_KEY"",""data"":""REQ_BODY""} ";
                body = body.Replace("AUTH_KEY", AuthKey).Replace("REQ_BODY", Base64Bitmap);
                //Log.Information(body);
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                if ((int)response.StatusCode == 500)
                {
                    Log.Information("IDScan API call failed.");
                    return emptyresponse;
                }
                //Log.Information("API Response Content");
                //Log.Information(Convert.ToString(response.Content));
                Log.Information("API Status");
                Log.Information(Convert.ToString(response.StatusCode));
                Log.Information("API Status Description");
                Log.Information(Convert.ToString(response.StatusDescription));
                Log.Information("API Headers");
                Log.Information(Convert.ToString(response.Headers));
                Log.Information("API Error Exception");
                Log.Information(Convert.ToString(response.ErrorException));
                Log.Information("API Error Message");
                Log.Information(Convert.ToString(response.ErrorMessage));
                Log.Information("API Protocol Version");
                Log.Information(Convert.ToString(response.ProtocolVersion));
                Log.Information("API Request");
                Log.Information(Convert.ToString(response.Request));
                Log.Information("API Response Status");
                Log.Information(Convert.ToString(response.ResponseStatus));
                Log.Information("API Server");
                Log.Information(Convert.ToString(response.Server));
                Log.Information("IDScan API call completed.");
                return response.Content;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error");
            }
            return emptyresponse;
        }
    }
}

using Gemalto_ID_Scan_Daemon___Claysys.Model;
using ManageMeeting.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Gemalto_ID_Scan_Daemon___Claysys.Connectors
{
    public class ScannerResponse
    {
        public string Status { get; set; }
        public string Id { get; set; }
        public string Description { get; set; }
    }
    public class DBConnector
    {
        public static ScannerResponse GetAuthorizeToken(DrivingLicense.Root obj, IDCardImage card)
        {
            // Initialization.
            var DBAPIURL = ConfigurationManager.AppSettings.Get("DBAPI_Url");
            var DBAPICLIENTID = ConfigurationManager.AppSettings.Get("DBAPI_ClientID");
            var DBAPICLIENTSECRET = ConfigurationManager.AppSettings.Get("DBAPI_ClientSecret");
            string responseObj = string.Empty;
            ScannerResponse scannerResponse = new ScannerResponse();
            try
            {
                // Posting.  
                using (var client = new HttpClient())
                {
                    // Setting Base address.  
                    client.BaseAddress = new Uri(DBAPIURL);
                    TokenRequest scanInfo = new TokenRequest() { ClientId = DBAPICLIENTID, ClientSecret = DBAPICLIENTSECRET };

                    var response = client.PostAsJsonAsync("api/Connect/GenerateToken", scanInfo).Result;
                    response.EnsureSuccessStatusCode();
                    string result = response.Content.ReadAsStringAsync().Result;
                    AutharizedResponse<TokenResponse> tokenResponse = JsonConvert.DeserializeObject<AutharizedResponse<TokenResponse>>(result);
                    if (tokenResponse.StatusCode == "500")
                    {
                        scannerResponse = new ScannerResponse() { Status = "Error", Description = tokenResponse.ErrorMessage };
                        //Log Eroor
                    }
                    else
                    {
                        client.DefaultRequestHeaders.Authorization
                         = new AuthenticationHeaderValue("Bearer", tokenResponse.Data.Token);
                        ScannerInfoRequest scannerInfoRequest = new ScannerInfoRequest() {
                            IDType = obj.ParseImageResult.DriverLicense.DocumentType,
                            FName = obj.ParseImageResult.DriverLicense.FirstName,
                            MName = obj.ParseImageResult.DriverLicense.MiddleName,
                            LName = obj.ParseImageResult.DriverLicense.LastName,
                            SA1 = obj.ParseImageResult.DriverLicense.Address1,
                            SA2 = obj.ParseImageResult.DriverLicense.Address2,
                            BackImage = card.ImageBack,
                            FrontImage = card.ImageFront,
                            Gender = obj.ParseImageResult.DriverLicense.Gender,
                            DOB = DateTime.Parse(obj.ParseImageResult.DriverLicense.Birthdate),
                            IssueDate = DateTime.Parse(obj.ParseImageResult.DriverLicense.IssueDate),
                            StateOfIssue = obj.ParseImageResult.DriverLicense.IssuedBy,
                            ExpDate = obj.ParseImageResult.DriverLicense.ExpirationDate,
                            JumioRefID = obj.ParseImageResult.Reference,
                            ScanInfo = "Gemalto Scanner",
                            CreatedBy = Environment.UserName,
                            CreatedOn = DateTime.Now.ToString(),
                            AppFormID = null,
                            ImageName = obj.ParseImageResult.DriverLicense.DocumentType+ "_" +obj.ParseImageResult.Reference,
                            IDNumber = obj.ParseImageResult.DriverLicense.LicenseNumber,
                            City = obj.ParseImageResult.DriverLicense.City
                        };
                        response = client.PostAsJsonAsync("api/Connect/InsertScannerInfo", scannerInfoRequest).Result;
                        response.EnsureSuccessStatusCode();
                        result = response.Content.ReadAsStringAsync().Result;
                        APIResponseData<ScannerInfoResponse> scannerInfoResponse = JsonConvert.DeserializeObject<APIResponseData<ScannerInfoResponse>>(result);
                        if (scannerInfoResponse.StatusCode == "500")
                        {
                            scannerResponse = new ScannerResponse() { Status = "Error", Description = scannerInfoResponse.ErrorMessage };

                        }
                        else
                        {
                            scannerResponse = new ScannerResponse() { Status = "success", Id = scannerInfoResponse.Data.Id };

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                scannerResponse = new ScannerResponse() { Status = "Error", Description = ex.Message };
                //Log Eroor
                throw ex;
            }



            return scannerResponse;
        }
    }

}

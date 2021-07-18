using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Gemalto_ID_Scan_Daemon___Claysys.Model;
using Gemalto_ID_Scan_Daemon___Claysys.Connectors;
using Newtonsoft.Json;
using ManageMeeting.Models;
using Serilog;
using System.IO;
using Serilog.Events;
using System.Web.Hosting;

namespace Gemalto_ID_Scan_Daemon___Claysys
{
    
    public class ValuesController : ApiController
    {
        public static IDCardImage card;
        public static DrivingLicense.Root deserializedObj;
        public static bool IsSuccess;

        /// <summary>
        /// GetScannedData updates DB with Scanned ID data from the Scanner
        /// </summary>
        /// <returns>Status JSON response along with Id, status and Description</returns>
        public string GetScannedData() 
        {
            //System.Diagnostics.Debugger.Launch();
            Log.Information("GetScannedData API Called.");
            ScannerResponse resp = new ScannerResponse() { Status = "", Description = "", Id = null};
            try
            {
                if (Gemalto_ScanIDCard())
                {
                    IDScanAPICall();
                    if (IsSuccess)
                    {
                        resp = DBConnector.GetAuthorizeToken(deserializedObj, card);
                    }
                    else
                    {
                        resp = new ScannerResponse() { Status = "Fail", Description = "Cannot Parse ID", Id = null };
                    }
                }
                else
                {
                    resp = new ScannerResponse() { Status = "Fail", Description = "Cannot initiate Scanner", Id = null };
                }
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Error");
            }
            string output = JsonConvert.SerializeObject(resp);
            return output;
            
        }

        /// <summary>
        /// CheckForSuccess checks whether a Scan was success or Fail, used mostly to find the Front and 
        /// pass it to the IDScan API
        /// </summary>
        /// <param name="JSONResponse"></param>
        /// <returns>Returns whether request was success or fail</returns>
        public static bool CheckForSuccess(string JSONResponse)
        {
            DrivingLicense.Root deserialize = JsonConvert.DeserializeObject<DrivingLicense.Root>(JSONResponse);
            return deserialize.ParseImageResult.Success;
        }

        /// Gemalto_ScanIDCard Initialises, Scan and Shutdown the Gemalto Scanner
        /// </summary>
        /// <returns>Returns whether the scanning was a success or not</returns>
        public static bool Gemalto_ScanIDCard()
        {

            try
            {
                Gemalto_SDKConnector.Initialise();
                Gemalto_SDKConnector.WaitForDocument();
                card = Gemalto_SDKConnector.ReadDocument();
                Gemalto_SDKConnector.Shutdown();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error");
                return false;
            }
            return true;
        }
        /// <summary>
        /// IDScanAPICall Calls the IDScan.net API and check whether the request was a success or not, If success,
        /// it will initialize the DriverLicense object with data read from the image passed to the IDScanAPI
        /// </summary>
        public static void IDScanAPICall()
        {
            
            string resp = String.Empty;
            try
            {
                resp = IDScan_APIConnector.IDScanAPI(card.ImageFront); //checking Front side image for data
                //resp = File.ReadAllText("C:\\Users\\naif\\Desktop\\response.txt");    //for test
                IsSuccess = CheckForSuccess(resp);
                if (!IsSuccess)
                {
                    resp = IDScan_APIConnector.IDScanAPI(card.ImageBack); //checking Back side image for data if Front side image cant be parsed
                    IsSuccess = CheckForSuccess(resp);
                }
                
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error");
            }
            deserializedObj = JsonConvert.DeserializeObject<DrivingLicense.Root>(resp);
            
        }

        

    }
}

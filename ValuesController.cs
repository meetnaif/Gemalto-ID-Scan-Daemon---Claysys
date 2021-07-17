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

namespace Gemalto_ID_Scan_Daemon___Claysys
{
    
    public class ValuesController : ApiController
    {
        public static IDCardImage card;
        public static DrivingLicense.Root deserializedObj;
        public string GetScannedData()
        {
            
            System.Diagnostics.Debugger.Launch();
            ScannerResponse resp = new ScannerResponse();
            if (Gemalto_ScanIDCard())
            {
                IDScanAPICall();
                resp = DBConnector.GetAuthorizeToken(deserializedObj, card);
            }
            else
            {

            }
            string output = JsonConvert.SerializeObject(resp);
            return output;
            
        }
        public static bool CheckForSuccess(string JSONResponse)
        {
            DrivingLicense.Root deserialize = JsonConvert.DeserializeObject<DrivingLicense.Root>(JSONResponse);
            if(deserialize.ParseImageResult.Success.Equals("true"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool Gemalto_ScanIDCard()
        {

            try
            {
                Gemalto_SDKConnector.Initialise();
                Gemalto_SDKConnector.WaitForDocument();
                card = Gemalto_SDKConnector.ReadDocument();
                Gemalto_SDKConnector.Shutdown();
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public static void IDScanAPICall()
        {
            string resp = String.Empty;
            try
            {
                if (CheckForSuccess(IDScan_APIConnector.IDScanAPI(card.ImageFront)) == true)
                {
                    resp = IDScan_APIConnector.IDScanAPI(card.ImageFront);
                }
                else
                {
                    resp = IDScan_APIConnector.IDScanAPI(card.ImageBack);
                }
            }
            catch (Exception e)
            {

            }
            deserializedObj = JsonConvert.DeserializeObject<DrivingLicense.Root>(resp);
            
        }

        

    }
}

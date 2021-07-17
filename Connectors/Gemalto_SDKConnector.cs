using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemalto_ID_Scan_Daemon___Claysys.Model;

namespace Gemalto_ID_Scan_Daemon___Claysys.Connectors
{
    public class Gemalto_SDKConnector
    {
        public static void Initialise()
        {

            MMM.Readers.FullPage.Reader.EnableLogging(
                true,
                3,
                -1,
                "ClasysTestApp.log"
            );

            MMM.Readers.ErrorCode result = MMM.Readers.FullPage.Reader.Initialise(
                null,
                null,
                null,
                null,
                true,
                false
            );

            if (result != MMM.Readers.ErrorCode.NO_ERROR_OCCURRED)
            {

                Console.WriteLine("An error occured during initialisation - {0}");
            }

        }

        public static void WaitForDocument()
        {
            MMM.Readers.ErrorCode result =
                MMM.Readers.FullPage.Reader.WaitForDocumentOnWindow(10000);

            if (result == MMM.Readers.ErrorCode.NO_ERROR_OCCURRED)
            {
                Console.WriteLine("Found a document");
            }
            else if (result == MMM.Readers.ErrorCode.ERROR_TIMED_OUT)
            {
                Console.WriteLine("Timed out waiting for document");
            }
            else
            {
                Console.WriteLine("An error occured - {0}");
            }
        }

        public static Model.IDCardImage ReadDocument()
        {
            IDCardImage card = new Model.IDCardImage();

            MMM.Readers.ErrorCode result = MMM.Readers.FullPage.Reader.ReadDocument();
            if (result != MMM.Readers.ErrorCode.NO_ERROR_OCCURRED)
            {
                Console.WriteLine("An error occurred reading the document - {0}");
            }
            else
            {
                object data = null;

                result = MMM.Readers.FullPage.Reader.GetData(
                    MMM.Readers.FullPage.DataType.CD_IMAGEVIS,
                    ref data
                );


                if (result == MMM.Readers.ErrorCode.NO_ERROR_OCCURRED)
                {
                    try
                    {
                        card.ImageFront = ToBase64String((Bitmap)data, ImageFormat.Jpeg);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                result = MMM.Readers.FullPage.Reader.GetData(
                    MMM.Readers.FullPage.DataType.CD_IMAGEVISREAR,
                    ref data
                );


                if (result == MMM.Readers.ErrorCode.NO_ERROR_OCCURRED)
                {
                    try
                    {
                        card.ImageBack = ToBase64String((Bitmap)data, ImageFormat.Jpeg);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

            }

            return card;
        }

        public static void Shutdown()
        {
            MMM.Readers.FullPage.Reader.Shutdown();

        }

        public static string ToBase64String(Bitmap bmp, ImageFormat imageFormat)
        {
            string base64String = string.Empty;


            MemoryStream memoryStream = new MemoryStream();
            bmp.Save(memoryStream, imageFormat);


            memoryStream.Position = 0;
            byte[] byteBuffer = memoryStream.ToArray();


            memoryStream.Close();


            base64String = Convert.ToBase64String(byteBuffer);
            byteBuffer = null;


            return base64String;
        }
    }
}

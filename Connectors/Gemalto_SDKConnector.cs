using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemalto_ID_Scan_Daemon___Claysys.Model;
using Serilog;

namespace Gemalto_ID_Scan_Daemon___Claysys.Connectors
{
    public class Gemalto_SDKConnector
    {

        public static void Initialise()
        {
            try
            {
                MMM.Readers.FullPage.Reader.EnableLogging(
                    true,
                    3,
                    -1,
                    "ClasysGemaltoService.log"
                );
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error");
            }

            try
            {
                Log.Information("Gemalto Scanner Initialization.");

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
                    Log.Error("An error occured during initialisation - {0}");
                }
                Log.Information("Gemalto Scanner Initialized.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error");

            }

        }

        public static void WaitForDocument()
        {
            try
            {
                Log.Information("Gemalto Scanner waiting for Document.");
                MMM.Readers.ErrorCode result =
                    MMM.Readers.FullPage.Reader.WaitForDocumentOnWindow(50000);

                if (result == MMM.Readers.ErrorCode.NO_ERROR_OCCURRED)
                {
                    Log.Information("Found a document.");
                }
                else if (result == MMM.Readers.ErrorCode.ERROR_TIMED_OUT)
                {
                    Log.Information("Timed out waiting for document.");
                }
                else
                {
                    Log.Error("An error occured - {0}");
                }
                Log.Information("Gemalto Scanner completed scanning.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error");
            }
        }

        public static Model.IDCardImage ReadDocument()
        {
            IDCardImage card = new Model.IDCardImage();
            try
            {
                Log.Information("Gemalto Scanner reading document Data.");
                MMM.Readers.ErrorCode result = MMM.Readers.FullPage.Reader.ReadDocument();
                if (result != MMM.Readers.ErrorCode.NO_ERROR_OCCURRED)
                {
                    Log.Error("An error occurred reading the document - {0}");
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
                            //SaveImage((Bitmap)data);//only for test
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, "Error");
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
                            //SaveImage((Bitmap)data);//only for test
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, "Error");
                        }
                    }

                }
                Log.Information("Gemalto Scanner completed reading data.");
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Error");
            }

            return card;
        }

        public static void Shutdown()
        {
            try
            {
                Log.Information("Gemalto Scanner Shutdown.");
                MMM.Readers.FullPage.Reader.Shutdown();
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Error");
            }

        }

        public static string ToBase64String(Bitmap bmp, ImageFormat imageFormat)
        {
            string base64String = string.Empty;

            try
            {
                MemoryStream memoryStream = new MemoryStream();
                bmp.Save(memoryStream, imageFormat);

                memoryStream.Position = 0;
                byte[] byteBuffer = memoryStream.ToArray();

                memoryStream.Close();

                base64String = Convert.ToBase64String(byteBuffer);
                byteBuffer = null;
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Error");
            }
            return base64String;
        }
        public static void SaveImage(Bitmap bmp)
        {
            
            string path = "C:\\" + Guid.NewGuid()+".jpg";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            try
            {
                bmp.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "SaveError");
                throw ex;
            }
        }
    }
}

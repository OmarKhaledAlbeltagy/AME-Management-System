using AMEKSA.Context;
using AMEKSA.Entities;
using QRCoder;
using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using AMEKSA.Models;
using System.Linq;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Org.BouncyCastle.Asn1.Ess;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
    public class AccountDeviceRep:IAccountDeviceRep
    {

        private readonly string SENDER = "XXXXXXXXXXXXXX";
        private readonly string APIKEY = "XXXXXXXXXXXXXXXXXXXXX";
        private readonly DbContainer db;
        private readonly ITimeRep ti;

        public AccountDeviceRep(DbContainer db, ITimeRep ti)
        {
            this.db = db;
            this.ti = ti;
        }

        public int AddNewDevice(AccountDevices obj)
        {
            if (obj.Warranty != null)
            {
                DateTime w = (DateTime)obj.Warranty;

                obj.Warranty = new DateTime(w.Year, w.Month, w.Day, 23, 59, 59);
            }
            if (obj.ServiceContract != null)
            {
                DateTime c = (DateTime)obj.ServiceContract;

                obj.ServiceContract = new DateTime(c.Year, c.Month, c.Day, 23, 59, 59);
            }

            db.accountDevices.Add(obj);
            db.SaveChanges();


            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode("https://maintenance.{{DashboardURL}}?"+obj.guid, QRCodeGenerator.ECCLevel.H);
            var qrCodeBitmap = new QRCode(qrCodeData).GetGraphic(60);

            var logoImage = Image.FromFile(Directory.GetCurrentDirectory() + "/wwwroot/logo/logotrans.png");
            var logoWidth = qrCodeBitmap.Width / 4;
            var logoHeight = qrCodeBitmap.Height / 8;
            var logoResized = new Bitmap(logoImage, logoWidth, logoHeight);

            var logoX = (qrCodeBitmap.Width - logoWidth) / 2;
            var logoY = (qrCodeBitmap.Height - logoHeight) / 2;

            using var graphics = Graphics.FromImage(qrCodeBitmap);
            graphics.DrawImage(logoResized, logoX, logoY, logoWidth, logoHeight);

            qrCodeBitmap.Save(Directory.GetCurrentDirectory() + "/wwwroot/DevicesQR/"+obj.Id+".png", ImageFormat.Png);

            return obj.Id;
        }

        public bool EditDevice(AccountDevices obj)
        {
            AccountDevices x = db.accountDevices.Find(obj.Id);
            x.SerialNumber = obj.SerialNumber;
            x.Warranty = obj.Warranty;
            x.ServiceContract = obj.ServiceContract;
            x.AccountId = obj.AccountId;
            x.ProductId = obj.ProductId;
            x.Model = obj.Model;
            x.IsEmpty = false;
            db.SaveChanges();
            return true;
        }

        public AccountDevices GetAccountDeviceById(int id)
        {
            return db.accountDevices.Find(id);
        }

        public List<AccountDeviceModel> GetAllDevices()
        {
            var list = db.accountDevices.Where(a=>a.IsEmpty == false).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = b.AccountName,
                ProductId = a.ProductId,
                Model = a.Model,
                SerialNumber = a.SerialNumber,
                guid = a.guid,
                Warranty = a.Warranty,
                ServiceContract = a.ServiceContract,
                DistrictId = b.DistrictId
            }).Join(db.product, a => a.ProductId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                ProductId = a.ProductId,
                ProductName = b.ProductName,
                Model = a.Model,
                SerialNumber = a.SerialNumber,
                guid = a.guid,
                Warranty = a.Warranty,
                ServiceContract = a.ServiceContract,
                DistrictId = a.DistrictId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                ProductId = a.ProductId,
                ProductName = a.ProductName,
                Model = a.Model,
                SerialNumber = a.SerialNumber,
                guid = a.guid,
                Warranty = a.Warranty,
                ServiceContract = a.ServiceContract,
                CityId = b.CityId
            }).Join(db.city, a => a.CityId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                ProductId = a.ProductId,
                ProductName = a.ProductName,
                Model = a.Model,
                SerialNumber = a.SerialNumber,
                guid = a.guid,
                Warranty = a.Warranty,
                ServiceContract = a.ServiceContract,
                CityName = b.CityName
            }).ToList();




            var listt = db.accountDevices.Where(a => a.IsEmpty == true).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = b.AccountName,
                ProductId = a.ProductId,
                Model = a.Model,
                SerialNumber = a.SerialNumber,
                guid = a.guid,
                Warranty = a.Warranty,
                ServiceContract = a.ServiceContract,
                DistrictId = b.DistrictId
            }).Join(db.product, a => a.ProductId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                ProductId = a.ProductId,
                ProductName = b.ProductName,
                Model = a.Model,
                SerialNumber = a.SerialNumber,
                guid = a.guid,
                Warranty = a.Warranty,
                ServiceContract = a.ServiceContract,
                DistrictId = a.DistrictId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                ProductId = a.ProductId,
                ProductName = a.ProductName,
                Model = a.Model,
                SerialNumber = a.SerialNumber,
                guid = a.guid,
                Warranty = a.Warranty,
                ServiceContract = a.ServiceContract,
                CityId = b.CityId
            }).Join(db.city, a => a.CityId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                ProductId = a.ProductId,
                ProductName = a.ProductName,
                Model = a.Model,
                SerialNumber = a.SerialNumber,
                guid = a.guid,
                Warranty = a.Warranty,
                ServiceContract = a.ServiceContract,
                CityName = b.CityName
            }).ToList();


            List<AccountDeviceModel> res = new List<AccountDeviceModel>();

            foreach (var item in list)
            {
                AccountDeviceModel obj = new AccountDeviceModel();
                obj.Id = item.Id;
                obj.AccountId = item.AccountId;
                obj.AccountName = item.AccountName;
                obj.ProductId = item.ProductId;
                obj.ProductName = item.ProductName;
                obj.Model = item.Model;
                obj.SerialNumber = item.SerialNumber;
                obj.CityName = item.CityName;
                if (item.Warranty != null)
                {
                    DateTime w = (DateTime)item.Warranty;
                    obj.Warranty = w.ToString("dd MMMM yyyy");
                }
                if (item.ServiceContract != null)
                {
                    DateTime s = (DateTime)item.ServiceContract;
                    obj.ServiceContract = s.ToString("dd MMMM yyyy");
                }
                res.Add(obj);
            }

            foreach (var item in listt)
            {
                AccountDeviceModel obj = new AccountDeviceModel();
                obj.Id = item.Id;
                obj.AccountId = item.AccountId;
                obj.AccountName = item.AccountName;
                obj.ProductId = item.ProductId;
                obj.ProductName = item.ProductName;
                obj.Model = item.Model;
                obj.SerialNumber = item.SerialNumber;
                obj.CityName = item.CityName;
                if (item.Warranty != null)
                {
                    DateTime w = (DateTime)item.Warranty;
                    obj.Warranty = w.ToString("dd MMMM yyyy");
                }
                if (item.ServiceContract != null)
                {
                    DateTime s = (DateTime)item.ServiceContract;
                    obj.ServiceContract = s.ToString("dd MMMM yyyy");
                }
                res.Add(obj);
            }

            return res;

        }

        public List<MaintenanceRequestsModel> GetUpcomingMaintenanceRequests()
        {
            DateTime now = ti.GetCurrentTime();

            var x = db.maintenanceRequest.Where(a => a.Done == false)
                .Join(db.accountDevices, a => a.AccountDevicesId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    FullName = a.FullName,
                    AccountId = b.AccountId,
                    ProductId = b.ProductId,
                    Model = b.Model,
                    SerialNumber = b.SerialNumber,
                    MobileNumber = a.MobileNumber,
                    LandLineNumber = a.LandLineNumber,
                    Email = a.Email,
                    brief = a.brief,
                    RequestDateTime = a.RequestDateTime,
                    d = a.RequestDateTime,
                    Warranty = b.Warranty,
                    ServiceContract = b.ServiceContract,
                    guid = a.guid,
                    EngineerName = a.EngineerName
                }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    FullName = a.FullName,
                    AccountName = b.AccountName,
                    ProductId = a.ProductId,
                    Model = a.Model,
                    SerialNumber = a.SerialNumber,
                    MobileNumber = a.MobileNumber,
                    LandLineNumber = a.LandLineNumber,
                    Email = a.Email,
                    brief = a.brief,
                    RequestDateTime = a.RequestDateTime,
                    d = a.RequestDateTime,
                    Warranty = a.Warranty,
                    ServiceContract = a.ServiceContract,
                    guid = a.guid,
                    EngineerName = a.EngineerName
                }).Join(db.product, a => a.ProductId, b => b.Id, (a, b) => new 
                {
                    Id = a.Id,
                    FullName = a.FullName,
                    AccountName = a.AccountName,
                    ProductName = b.ProductName,
                    Model = a.Model,
                    SerialNumber = a.SerialNumber,
                    MobileNumber = a.MobileNumber,
                    LandLineNumber = a.LandLineNumber,
                    Email = a.Email,
                    brief = a.brief,
                    RequestDateTime = a.RequestDateTime.ToString("dd MMMM yyyy hh:mm tt"),
                    d = a.RequestDateTime,
                    Warranty = a.Warranty,
                    ServiceContract = a.ServiceContract,
                    guid = a.guid,
                    EngineerName = a.EngineerName
                }).OrderBy(a=>a.d).ToList();

            List<MaintenanceRequestsModel> res = new List<MaintenanceRequestsModel>();

            foreach (var item in x)
            {
                MaintenanceRequestsModel obj = new MaintenanceRequestsModel();
                obj.Id = item.Id;
                obj.FullName = item.FullName;
                obj.AccountName = item.AccountName;
                obj.ProductName = item.ProductName;
                obj.Model = item.Model;
                obj.SerialNumber = item.SerialNumber;
                obj.MobileNumber = item.MobileNumber;
                obj.LandLineNumber = item.LandLineNumber;
                obj.Email = item.Email;
                obj.brief = item.brief;
                obj.RequestDateTime = item.RequestDateTime;
                obj.d = item.d;
                obj.guid = item.guid;
                obj.EngineerName = item.EngineerName;
                if (item.Warranty != null)
                {
                    DateTime w = (DateTime)item.Warranty;
                    obj.Warranty = w.ToString("dd MMMM yyyy");
                    if (w.Date >= now.Date)
                    {
                        obj.UnderWarranty = true;
                    }
                }

                if (item.ServiceContract != null)
                {
                    DateTime s = (DateTime)item.ServiceContract;
                    obj.Contract = s.ToString("dd MMMM yyyy");
                    if (s.Date >= now.Date)
                    {
                        obj.UnderContract = true;
                    }
                }


                res.Add(obj);

            }


            return res;
        }

        public List<TrainingRequestModel> GetUpcomingTrainingRequests()
        {
            DateTime now = ti.GetCurrentTime();

            List<TrainingRequestModel> res = db.trainingRequest.Where(a => a.Done == false)
               .Join(db.accountDevices, a => a.AccountDevicesId, b => b.Id, (a, b) => new
               {
                   Id = a.Id,
                   FullName = a.FullName,
                   AccountId = b.AccountId,
                   ProductId = b.ProductId,
                   Model = b.Model,
                   SerialNumber = b.SerialNumber,
                   MobileNumber = a.MobileNumber,
                   LandLineNumber = a.LandLineNumber,
                   Email = a.Email,
                   RequestedDate = a.RequestedDate,
                   RequestDateTime = a.RequestDateTime,
                   d = a.RequestDateTime,
                   Warranty = b.Warranty,
                   ServiceContract = b.ServiceContract,
                   guid = a.guid,
                   EngineerName = a.EngineerName
               }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
               {
                   Id = a.Id,
                   FullName = a.FullName,
                   AccountName = b.AccountName,
                   ProductId = a.ProductId,
                   Model = a.Model,
                   SerialNumber = a.SerialNumber,
                   MobileNumber = a.MobileNumber,
                   LandLineNumber = a.LandLineNumber,
                   Email = a.Email,
                   RequestedDate = a.RequestedDate,
                   RequestDateTime = a.RequestDateTime,
                   d = a.RequestDateTime,
                   Warranty = a.Warranty,
                   ServiceContract = a.ServiceContract,
                   guid = a.guid,
                   EngineerName = a.EngineerName
               }).Join(db.product, a => a.ProductId, b => b.Id, (a, b) => new TrainingRequestModel
               {
                   Id = a.Id,
                   FullName = a.FullName,
                   AccountName = a.AccountName,
                   ProductName = b.ProductName,
                   Model = a.Model,
                   SerialNumber = a.SerialNumber,
                   MobileNumber = a.MobileNumber,
                   LandLineNumber = a.LandLineNumber,
                   Email = a.Email,
                   RequestedDate = a.RequestedDate.ToString("hh MMMM yyyy"),
                   RequestDateTime = a.RequestDateTime.ToString("dd MMMM yyyy hh:mm tt"),
                   d = a.RequestedDate,
                   guid = a.guid,
                   EngineerName = a.EngineerName
               }).OrderBy(a => a.d).ToList();


            foreach (var item in res)
            {
                if (item.d < now)
                {
                    item.NotDoneOnTime = true;
                }
            }


            return res;
        }

        public dynamic GetMaintenanceVisitDetailsRep(string g)
        {
            Guid gu = new Guid(g);

            MaintenanceRequest obj = db.maintenanceRequest.Where(a => a.guid == gu).FirstOrDefault();

            if (obj != null && obj.Done == false) 
            {
                DateTime now = ti.GetCurrentTime();
                var x = db.accountDevices.Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountId = a.AccountId,
                    AccountName = b.AccountName,
                    ProductId = a.ProductId,
                    Model = a.Model,
                    SerialNumber = a.SerialNumber,
                    guid = a.guid,
                    Warranty = a.Warranty,
                    ServiceContract = a.ServiceContract
                }).Join(db.product, a => a.ProductId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountId = a.AccountId,
                    AccountName = a.AccountName,
                    ProductId = a.ProductId,
                    ProductName = b.ProductName,
                    Model = a.Model,
                    SerialNumber = a.SerialNumber,
                    guid = a.guid,
                    Warranty = a.Warranty,
                    ServiceContract = a.ServiceContract
                }).Join(db.maintenanceRequest.Where(a => a.guid == gu), a => a.Id, b => b.AccountDevicesId, (a, b) => new
                {
                    AccountDeviceId = a.Id,
                    AccountId = a.AccountId,
                    AccountName = a.AccountName,
                    ProductId = a.ProductId,
                    ProductName = a.ProductName,
                    Model = a.Model,
                    SerialNumber = a.SerialNumber,
                    guid = a.guid,
                    Warranty = a.Warranty,
                    ServiceContract = a.ServiceContract,

                    RequestId = b.Id,
                    FullName = b.FullName,
                    MobileNumber = b.MobileNumber,
                    LandLineNumber = b.LandLineNumber,
                    Email = b.Email,
                    brief = b.brief,
                    RequestDateTime = b.RequestDateTime.ToString("dd MMMM yyyy hh:mm tt")


                }).FirstOrDefault();


                VisitDetailsModel res = new VisitDetailsModel();
                res.AccountDeviceId = x.AccountDeviceId;
                res.AccountName = x.AccountName;
                res.ProductName = x.ProductName;
                res.Model = x.Model;
                res.SerialNumber = x.SerialNumber;
                if (x.Warranty != null)
                {
                    DateTime w = (DateTime)x.Warranty;
                    res.WarrantyDate = w.ToString("dd MMMM yyyy");
                    if (w.Date >= now.Date)
                    {
                        res.warranty = true;
                    }
                    else
                    {
                        res.warranty = false;
                    }

                }
                else
                {
                    res.WarrantyDate = null;

                }
                if (x.ServiceContract != null)
                {
                    DateTime s = (DateTime)x.ServiceContract;
                    res.ServiceContract = s.ToString("dd MMMM yyyy");
                    if (s.Date >= now.Date)
                    {
                        res.Contract = true;
                    }
                    else
                    {
                        res.Contract = false;
                    }
                }
                else
                {
                    res.ServiceContract = null;

                }

                res.RequestId = x.RequestId;
                res.FullName = x.FullName;
                res.MobileNumber = x.MobileNumber;
                res.LandLineNumber = x.LandLineNumber;
                res.Email = x.Email;
                res.brief = x.brief;
                res.RequestDateTime = x.RequestDateTime;

                return res;

            }
            else
            {
                return false;
            }

        }

        public async Task<dynamic> MantenanceRequest(MaintenanceRequest obj)
        {
            
            DateTime now = ti.GetCurrentTime();

            MaintenanceRequest check = db.maintenanceRequest.Where(a => a.AccountDevicesId == obj.AccountDevicesId && a.Done == false).OrderBy(a=>a.RequestDateTime).LastOrDefault();
         
            if (check == null || check.RequestDateTime.AddHours(24) <= now)
            {

                Guid g = Guid.NewGuid();
                string removed = obj.MobileNumber.Remove(0,1);
                obj.RequestDateTime = now;
                string whatsapp = "966" + removed;
                obj.MobileNumber = "00966" + removed;
                obj.guid = g;
                db.maintenanceRequest.Add(obj);
                db.SaveChanges();


                string productname = db.product.Find(db.accountDevices.Find(obj.AccountDevicesId).ProductId).ProductName;
                string serialnumber = db.accountDevices.Find(obj.AccountDevicesId).SerialNumber;
                string accountname = db.account.Find(db.accountDevices.Find(obj.AccountDevicesId).AccountId).AccountName;

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://zjkn6w.api.infobip.com");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("App", APIKEY);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string message = $@"
            {{
                ""messages"": [
                {{
                    ""from"": ""{SENDER}"",
                    ""to"": ""{whatsapp}"",
                    ""content"": {{
                    ""templateName"": ""maintenance"",
                    ""templateData"": {{
                        ""body"": {{
                        ""placeholders"": [
                            ""{productname}"",
                            ""{productname}"",
                            ""{serialnumber}""
                        ]
                        }}
                        
                    }},
                    ""language"": ""ar""
                }}
                }}
            ]
            }}";
                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, "/whatsapp/1/message/template");
                httpRequest.Content = new StringContent(message, Encoding.UTF8, "application/json");

                var response = await client.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();



                string messagee = $@"
            {{
                ""messages"": [
                {{
                    ""from"": ""{SENDER}"",
                    ""to"": ""966567620076"",
                    ""content"": {{
                    ""templateName"": ""maintenancerequested"",
                    ""templateData"": {{
                        ""body"": {{
                        ""placeholders"": [
                            ""{productname}"",
                            ""{accountname}""
                        ]
                        }}
                        
                    }},
                    ""language"": ""ar""
                }}
                }}
            ]
            }}";


                HttpRequestMessage httpRequestt = new HttpRequestMessage(HttpMethod.Post, "/whatsapp/1/message/template");
                httpRequestt.Content = new StringContent(messagee, Encoding.UTF8, "application/json");

                var responsee = await client.SendAsync(httpRequestt);
                var responseContentt = await responsee.Content.ReadAsStringAsync();

                MaintenanceRequest xx = db.maintenanceRequest.Find(obj.Id);

                xx.ClientResponse = responseContent;
                xx.ManagementResponse = responseContentt;
                db.SaveChanges();
                return true;
            }

            else
            {
                return false;
            }




           
        }

        public bool PrepareBulkQR(ListOfIdsModel obj)
        {

            List<AccountDevices> ad = db.accountDevices.Where(a=>a.bulk == true).ToList();

            foreach (var item in ad)
            {
                item.bulk = false;
            }
            db.SaveChanges();
            
            foreach (var item in obj.Ids)
            {
                AccountDevices x = db.accountDevices.Find(item);
                x.bulk = true;
            }
            db.SaveChanges();
            return true;

        }

        public dynamic ScanQrCode(string guid)
        {
            DateTime now = ti.GetCurrentTime();
            try
            {
                Guid g = new Guid(guid);
                var x = db.accountDevices.Where(a => a.guid == g).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountId = a.AccountId,
                    AccountName = b.AccountName,
                    ProductId = a.ProductId,
                    Model = a.Model,
                    SerialNumber = a.SerialNumber,
                    guid = a.guid,
                    Warranty = a.Warranty,
                    ServiceContract = a.ServiceContract
                }).Join(db.product, a => a.ProductId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountId = a.AccountId,
                    AccountName = a.AccountName,
                    ProductId = a.ProductId,
                    ProductName = b.ProductName,
                    Model = a.Model,
                    SerialNumber = a.SerialNumber,
                    guid = a.guid,
                    Warranty = a.Warranty,
                    ServiceContract = a.ServiceContract
                }).FirstOrDefault();

                ScanDeviceQrCodeModel res = new ScanDeviceQrCodeModel();
                res.Id = x.Id;
                res.AccountName = x.AccountName;
                res.ProductName = x.ProductName;
                res.Model = x.Model;
                res.SerialNumber = x.SerialNumber;
                res.Now = now.AddDays(1);
                if (x.Warranty != null)
                {
                    DateTime w = (DateTime)x.Warranty;
                    res.WarrantyDate = w.ToString("dd MMMM yyyy");
                    if (w.Date >= now.Date)
                    {
                        res.warranty = true;
                    }
                    else
                    {
                        res.warranty = false;
                    }

                }
                else
                {
                    res.WarrantyDate = null;

                }
                if (x.ServiceContract != null)
                {
                    DateTime s = (DateTime)x.ServiceContract;
                    res.ServiceContract = s.ToString("dd MMMM yyyy");
                    if (s.Date >= now.Date)
                    {
                        res.Contract = true;
                    }
                    else
                    {
                        res.Contract = false;
                    }
                }
                else
                {
                    res.ServiceContract = null;

                }

                return res;
            }
            catch (Exception ex)
            {
                return false;
            }
            

            
        }

        public bool SubmitMaintenanceVisit(SubmitTrainingVisitModel obj)
        {
            DateTime now = ti.GetCurrentTime();
            MaintenanceRequest m = db.maintenanceRequest.Find(obj.RequestId);
            m.VisitDate = now;
            m.VisitReport = obj.VisitReport;
            m.Done = true;
            db.SaveChanges();
            return true;
        }

        public bool SubmitTrainingVisit(SubmitTrainingVisitModel obj)
        {
            DateTime now = ti.GetCurrentTime();
            TrainingRequest m = db.trainingRequest.Find(obj.RequestId);
            m.VisitDate = now;
            m.VisitReport = obj.VisitReport;
            m.Done = true;
            db.SaveChanges();
            return true;
        }

        public async Task<bool> TrainingRequest(TrainingRequest obj)
        {
            
            DateTime now = ti.GetCurrentTime();

            TrainingRequest check = db.trainingRequest.Where(a => a.AccountDevicesId == obj.AccountDevicesId && a.Done == false).OrderBy(a => a.RequestDateTime).LastOrDefault();

            if (check == null || check.RequestDateTime.AddHours(24) <= now)
            {
                Guid g = Guid.NewGuid();
                string removed = obj.MobileNumber.Remove(0,1);
                obj.RequestDateTime = now;
                string whatsapp = "966" + removed;
                obj.MobileNumber = "00966" + removed;
                obj.guid = g;
                db.trainingRequest.Add(obj);
                db.SaveChanges();


                string productname = db.product.Find(db.accountDevices.Find(obj.AccountDevicesId).ProductId).ProductName;
                string serialnumber = db.accountDevices.Find(obj.AccountDevicesId).SerialNumber;
                string accountname = db.account.Find(db.accountDevices.Find(obj.AccountDevicesId).AccountId).AccountName;

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://zjkn6w.api.infobip.com");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("App", APIKEY);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string message = $@"
            {{
                ""messages"": [
                {{
                    ""from"": ""{SENDER}"",
                    ""to"": ""{whatsapp}"",
                    ""content"": {{
                    ""templateName"": ""training"",
                    ""templateData"": {{
                        ""body"": {{
                        ""placeholders"": [
                            ""{productname}"",
                            ""{productname}"",
                            ""{serialnumber}""
                        ]
                        }}
                        
                    }},
                    ""language"": ""ar""
                }}
                }}
            ]
            }}";
                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, "/whatsapp/1/message/template");
                httpRequest.Content = new StringContent(message, Encoding.UTF8, "application/json");

                var response = await client.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();

                string messagee = $@"
            {{
                ""messages"": [
                {{
                    ""from"": ""{SENDER}"",
                    ""to"": ""966567620076"",
                    ""content"": {{
                    ""templateName"": ""maintenancerequested"",
                    ""templateData"": {{
                        ""body"": {{
                        ""placeholders"": [
                            ""{productname}"",
                            ""{accountname}""
                        ]
                        }}
                        
                    }},
                    ""language"": ""ar""
                }}
                }}
            ]
            }}";




                HttpRequestMessage httpRequestt = new HttpRequestMessage(HttpMethod.Post, "/whatsapp/1/message/template");
                httpRequestt.Content = new StringContent(messagee, Encoding.UTF8, "application/json");

                var responsee = await client.SendAsync(httpRequestt);
                var responseContentt = await responsee.Content.ReadAsStringAsync();

                obj.ClientResponse = responseContent;
                obj.ManagementResponse = responseContentt;

                db.SaveChanges();

                return true;
            }

            else
            {
                return false;
            }
        }

        public dynamic GetTrainingVisitDetailsRep(string g)
        {
            Guid gu = new Guid(g);

            TrainingRequest obj = db.trainingRequest.Where(a => a.guid == gu).FirstOrDefault();

            if (obj != null && obj.Done == false)
            {
                DateTime now = ti.GetCurrentTime();
                var x = db.accountDevices.Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountId = a.AccountId,
                    AccountName = b.AccountName,
                    ProductId = a.ProductId,
                    Model = a.Model,
                    SerialNumber = a.SerialNumber,
                    guid = a.guid,
                    Warranty = a.Warranty,
                    ServiceContract = a.ServiceContract
                }).Join(db.product, a => a.ProductId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountId = a.AccountId,
                    AccountName = a.AccountName,
                    ProductId = a.ProductId,
                    ProductName = b.ProductName,
                    Model = a.Model,
                    SerialNumber = a.SerialNumber,
                    guid = a.guid,
                    Warranty = a.Warranty,
                    ServiceContract = a.ServiceContract
                }).Join(db.trainingRequest.Where(a => a.guid == gu), a => a.Id, b => b.AccountDevicesId, (a, b) => new
                {
                    AccountDeviceId = a.Id,
                    AccountId = a.AccountId,
                    AccountName = a.AccountName,
                    ProductId = a.ProductId,
                    ProductName = a.ProductName,
                    Model = a.Model,
                    SerialNumber = a.SerialNumber,
                    guid = a.guid,
                    Warranty = a.Warranty,
                    ServiceContract = a.ServiceContract,

                    RequestId = b.Id,
                    FullName = b.FullName,
                    MobileNumber = b.MobileNumber,
                    LandLineNumber = b.LandLineNumber,
                    Email = b.Email,
                    RequestedDate = b.RequestedDate.ToString("dd MMMM yyyy"),
                    RequestDateTime = b.RequestDateTime.ToString("dd MMMM yyyy hh:mm tt")

                }).FirstOrDefault();


                VisitDetailsModel res = new VisitDetailsModel();
                res.AccountDeviceId = x.AccountDeviceId;
                res.AccountName = x.AccountName;
                res.ProductName = x.ProductName;
                res.Model = x.Model;
                res.SerialNumber = x.SerialNumber;
                if (x.Warranty != null)
                {
                    DateTime w = (DateTime)x.Warranty;
                    res.WarrantyDate = w.ToString("dd MMMM yyyy");
                    if (w.Date >= now.Date)
                    {
                        res.warranty = true;
                    }
                    else
                    {
                        res.warranty = false;
                    }

                }
                else
                {
                    res.WarrantyDate = null;

                }
                if (x.ServiceContract != null)
                {
                    DateTime s = (DateTime)x.ServiceContract;
                    res.ServiceContract = s.ToString("dd MMMM yyyy");
                    if (s.Date >= now.Date)
                    {
                        res.Contract = true;
                    }
                    else
                    {
                        res.Contract = false;
                    }
                }
                else
                {
                    res.ServiceContract = null;

                }

                res.RequestId = x.RequestId;
                res.FullName = x.FullName;
                res.MobileNumber = x.MobileNumber;
                res.LandLineNumber = x.LandLineNumber;
                res.Email = x.Email;
                res.RequestedDate = x.RequestedDate;
                res.RequestDateTime = x.RequestDateTime;
                return res;

            }
            else
            {
                return false;
            }
        }

        public List<ServiceExcelModel> MaintenanceReportByMonth(int month, int year)
        {
            int days = DateTime.DaysInMonth(year, month);
            DateTime start = new DateTime(year, month, 1);
            DateTime end = new DateTime(year, month, days);

            var x = db.maintenanceRequest.Where(a => a.RequestDateTime >= start && a.RequestDateTime <= end || a.VisitDate >= start && a.VisitDate <= end).Join(db.accountDevices, a => a.AccountDevicesId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                FullName = a.FullName,
                AccountId = b.AccountId,
                ProductId = b.ProductId,
                MobileNumber = a.MobileNumber,
                brief = a.brief,
                RequestDateTime = a.RequestDateTime.ToString("dd MMMM yyyy - hh:mm tt"),
                d = a.RequestDateTime,
                Done = a.Done,
                VisitDate = a.VisitDate,
                VisitReport = a.VisitReport
            }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                FullName = a.FullName,
                AccountName = b.AccountName,
                ProductId = a.ProductId,
                MobileNumber = a.MobileNumber,
                brief = a.brief,
                RequestDateTime = a.RequestDateTime,
                d = a.d,
                Done = a.Done,
                VisitDate = a.VisitDate,
                VisitReport = a.VisitReport
            }).Join(db.product, a => a.ProductId, b => b.Id, (a, b) => new
            {
                FullName = a.FullName,
                AccountName = a.AccountName,
                ProductName = b.ProductName,
                MobileNumber = a.MobileNumber,
                brief = a.brief,
                RequestDateTime = a.RequestDateTime,
                d = a.d,
                Done = a.Done,
                VisitDate = a.VisitDate,
                VisitReport = a.VisitReport
            });

            List<ServiceExcelModel> res = new List<ServiceExcelModel>();

            foreach (var item in x)
            {
                ServiceExcelModel obj = new ServiceExcelModel();
                obj.FullName = item.FullName;
                obj.AccountName = item.AccountName;
                obj.ProductName = item.ProductName;
                obj.MobileNumber = item.MobileNumber;
                obj.brief = item.brief;
                obj.RequestDateTime = item.RequestDateTime;
                obj.d = item.d;
                obj.Done = item.Done;

                if (item.Done == true)
                {
                    obj.VisitDate = item.VisitDate.ToString("dd MMMM yyyy - hh:mm tt");
                    obj.VisitReport = item.VisitReport;
                }
                else 
                {
                    obj.VisitDate = "";
                    obj.VisitReport = "";
                }

                res.Add(obj);

            }

            return res.OrderBy(a => a.RequestDateTime).ToList();

        }

        public List<ServiceExcelModel> TrainingReportByMonth(int month, int year)
        {
            int days = DateTime.DaysInMonth(year, month);
            DateTime start = new DateTime(year, month, 1);
            DateTime end = new DateTime(year, month, days);

            var x = db.trainingRequest.Where(a => a.RequestDateTime >= start && a.RequestDateTime <= end || a.VisitDate >= start && a.VisitDate <= end).Join(db.accountDevices, a => a.AccountDevicesId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                FullName = a.FullName,
                AccountId = b.AccountId,
                ProductId = b.ProductId,
                MobileNumber = a.MobileNumber,
                RequestedDate = a.RequestedDate.ToString("dd - MMMM - yyyy"),
                RequestDateTime = a.RequestDateTime.ToString("dd MMMM yyyy - hh:mm tt"),
                d = a.RequestDateTime,
                Done = a.Done,
                VisitDate = a.VisitDate,
                VisitReport = a.VisitReport
            }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                FullName = a.FullName,
                AccountName = b.AccountName,
                ProductId = a.ProductId,
                MobileNumber = a.MobileNumber,
                RequestedDate = a.RequestedDate,
                RequestDateTime = a.RequestDateTime,
                d = a.d,
                Done = a.Done,
                VisitDate = a.VisitDate,
                VisitReport = a.VisitReport
            }).Join(db.product, a => a.ProductId, b => b.Id, (a, b) => new
            {
                FullName = a.FullName,
                AccountName = a.AccountName,
                ProductName = b.ProductName,
                MobileNumber = a.MobileNumber,
                RequestedDate = a.RequestedDate,
                RequestDateTime = a.RequestDateTime,
                d = a.d,
                Done = a.Done,
                VisitDate = a.VisitDate,
                VisitReport = a.VisitReport
            });

            List<ServiceExcelModel> res = new List<ServiceExcelModel>();

            foreach (var item in x)
            {
                ServiceExcelModel obj = new ServiceExcelModel();
                obj.FullName = item.FullName;
                obj.AccountName = item.AccountName;
                obj.ProductName = item.ProductName;
                obj.MobileNumber = item.MobileNumber;
                obj.RequestedDate = item.RequestedDate;
                obj.RequestDateTime = item.RequestDateTime;
                obj.d = item.d;
                obj.Done = item.Done;

                if (item.Done == true)
                {
                    obj.VisitDate = item.VisitDate.ToString("dd MMMM yyyy - hh:mm tt");
                    obj.VisitReport = item.VisitReport;
                }
                else
                {
                    obj.VisitDate = "";
                    obj.VisitReport = "";
                }

                res.Add(obj);

            }

            return res.OrderBy(a => a.RequestDateTime).ToList();
        }

        public bool AssignMaintenanceEngineer(RequestEngineer obj)
        {
            MaintenanceRequest m = db.maintenanceRequest.Find(obj.Id);
            m.EngineerName = obj.EngineerName;
            db.SaveChanges();
            return true;
        }

        public bool AssignTrainingEngineer(RequestEngineer obj)
        {
            TrainingRequest m = db.trainingRequest.Find(obj.Id);
            m.EngineerName = obj.EngineerName;
            db.SaveChanges();
            return true;
        }

        public List<SimpleDeviceModel> GetDevicesByAccountId(int Id)
        {
            List<SimpleDeviceModel> res = db.accountDevices.Where(a=>a.AccountId == Id).Join(db.product, a => a.ProductId, b => b.Id, (a, b) => new SimpleDeviceModel 
            {
                DeviceName = b.ProductName,
                SerialNumber = a.SerialNumber
            }).ToList();

            return res;
        }
    }
}

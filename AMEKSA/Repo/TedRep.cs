using AMEKSA.Context;
using AMEKSA.Entities;
using AMEKSA.Models;
using Microsoft.AspNetCore.Http;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace AMEKSA.Repo
{
    public class TedRep:ITedRep
    {
        private readonly DbContainer db;
        private readonly ITimeRep ti;

        public TedRep(DbContainer db, ITimeRep ti)
        {
            this.db = db;
            this.ti = ti;
        }





        public bool Accept(TedRegisterModel r)
        {
            TED t = db.ted.Find(r.Id);
            t.IsComing = true;
            t.IsNotComing = false;
            t.Date = ti.GetCurrentTime();
            t.Email = r.Email;
            t.PhoneNumber = r.PhoneNumber;
            
            db.SaveChanges();

            QRCodeGenerator qr = new QRCodeGenerator();
            Bitmap qh = QRCodeHelper.GetQRCode("https://{{DashboardURL}}/Organizer/LongCode.html?" + t.guidd.ToString(), 16, Color.Black, Color.White, QRCodeGenerator.ECCLevel.H);
            qh.Save(Directory.GetCurrentDirectory() + "/image/"+t.Id+".png", ImageFormat.Png);
            string body = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional //EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\r\n<html xmlns=\"http://www.w3.org/1999/xhtml\" xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:o=\"urn:schemas-microsoft-com:office:office\">\r\n<head>\r\n    <!--[if gte mso 9]>\r\n    <xml>\r\n      <o:OfficeDocumentSettings>\r\n        <o:AllowPNG/>\r\n        <o:PixelsPerInch>96</o:PixelsPerInch>\r\n      </o:OfficeDocumentSettings>\r\n    </xml>\r\n    <![endif]-->\r\n    <meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\">\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\r\n    <meta name=\"x-apple-disable-message-reformatting\">\r\n    <!--[if !mso]><!-->\r\n    <meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\"><!--<![endif]-->\r\n    <title></title>\r\n\r\n    <style type=\"text/css\">\r\n        @media only screen and (min-width: 620px) {\r\n            .u-row {\r\n                width: 600px !important;\r\n            }\r\n\r\n                .u-row .u-col {\r\n                    vertical-align: top;\r\n                }\r\n\r\n                .u-row .u-col-50 {\r\n                    width: 300px !important;\r\n                }\r\n\r\n                .u-row .u-col-100 {\r\n                    width: 600px !important;\r\n                }\r\n        }\r\n\r\n        @media (max-width: 620px) {\r\n            .u-row-container {\r\n                max-width: 100% !important;\r\n                padding-left: 0px !important;\r\n                padding-right: 0px !important;\r\n            }\r\n\r\n            .u-row .u-col {\r\n                min-width: 320px !important;\r\n                max-width: 100% !important;\r\n                display: block !important;\r\n            }\r\n\r\n            .u-row {\r\n                width: calc(100% - 40px) !important;\r\n            }\r\n\r\n            .u-col {\r\n                width: 100% !important;\r\n            }\r\n\r\n                .u-col > div {\r\n                    margin: 0 auto;\r\n                }\r\n        }\r\n\r\n        body {\r\n            margin: 0;\r\n            padding: 0;\r\n        }\r\n\r\n        table,\r\n        tr,\r\n        td {\r\n            vertical-align: top;\r\n            border-collapse: collapse;\r\n        }\r\n\r\n        p {\r\n            margin: 0;\r\n        }\r\n\r\n        .ie-container table,\r\n        .mso-container table {\r\n            table-layout: fixed;\r\n        }\r\n\r\n        * {\r\n            line-height: inherit;\r\n        }\r\n\r\n        a[x-apple-data-detectors='true'] {\r\n            color: inherit !important;\r\n            text-decoration: none !important;\r\n        }\r\n\r\n        table, td {\r\n            color: #000000;\r\n        }\r\n\r\n        #u_body a {\r\n            color: #0000ee;\r\n            text-decoration: underline;\r\n        }\r\n\r\n        #u_content_text_4 a {\r\n            color: #f1c40f;\r\n        }\r\n\r\n        @media (max-width: 480px) {\r\n            /* #u_content_image_1 .v-src-width {\r\n                width: auto !important;\r\n            }\r\n\r\n            #u_content_image_1 .v-src-max-width {\r\n                max-width: 25% !important;\r\n            } */\r\n\r\n            #u_content_text_3 .v-container-padding-padding {\r\n                padding: 10px 20px 20px !important;\r\n            }\r\n\r\n            #u_content_button_1 .v-size-width {\r\n                width: 65% !important;\r\n            }\r\n\r\n            #u_content_text_2 .v-container-padding-padding {\r\n                padding: 20px 20px 60px !important;\r\n            }\r\n\r\n            #u_content_text_4 .v-container-padding-padding {\r\n                padding: 60px 20px !important;\r\n            }\r\n\r\n            #u_content_heading_2 .v-container-padding-padding {\r\n                padding: 30px 10px 0px !important;\r\n            }\r\n\r\n            #u_content_heading_2 .v-text-align {\r\n                text-align: center !important;\r\n            }\r\n\r\n            #u_content_social_1 .v-container-padding-padding {\r\n                padding: 10px 10px 10px 98px !important;\r\n            }\r\n\r\n            #u_content_text_5 .v-container-padding-padding {\r\n                padding: 10px 20px 30px !important;\r\n            }\r\n\r\n            #u_content_text_5 .v-text-align {\r\n                text-align: center !important;\r\n            }\r\n        }\r\n    </style>\r\n\r\n\r\n\r\n    <!--[if !mso]><!-->\r\n    <link href=\"https://fonts.googleapis.com/css?family=Open+Sans:400,700&display=swap\" rel=\"stylesheet\" type=\"text/css\">\r\n    <link href=\"https://fonts.googleapis.com/css?family=Rubik:400,700&display=swap\" rel=\"stylesheet\" type=\"text/css\"><!--<![endif]-->\r\n\r\n</head>\r\n\r\n<body class=\"clean-body u_body\" style=\"margin: 0;padding: 0;-webkit-text-size-adjust: 100%;background-color: #000000;color: #000000\">\r\n    <!--[if IE]><div class=\"ie-container\"><![endif]-->\r\n    <!--[if mso]><div class=\"mso-container\"><![endif]-->\r\n    <table id=\"u_body\" style=\"border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;min-width: 320px;Margin: 0 auto;background-color: #000000;width:100%\" cellpadding=\"0\" cellspacing=\"0\">\r\n        <tbody>\r\n            <tr style=\"vertical-align: top\">\r\n                <td style=\"word-break: break-word;border-collapse: collapse !important;vertical-align: top\">\r\n                    <!--[if (mso)|(IE)]><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td align=\"center\" style=\"background-color: #000000;\"><![endif]-->\r\n\r\n\r\n                    <div class=\"u-row-container\" style=\"padding: 0px;background-color: transparent\">\r\n                        <div class=\"u-row\" style=\"Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;\">\r\n                            <div style=\"border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;\">\r\n                                <!--[if (mso)|(IE)]><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td style=\"padding: 0px;background-color: transparent;\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"width:600px;\"><tr style=\"background-color: transparent;\"><![endif]-->\r\n                                <!--[if (mso)|(IE)]><td align=\"center\" width=\"600\" style=\"width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\" valign=\"top\"><![endif]-->\r\n                                <div class=\"u-col u-col-100\" style=\"max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;\">\r\n                                    <div style=\"height: 100%;width: 100% !important;\">\r\n                                        <!--[if (!mso)&(!IE)]><!--><div style=\"height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\">\r\n                                            <!--<![endif]-->\r\n\r\n                                            <table id=\"u_content_image_1\" style=\"font-family:'Open Sans',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">\r\n                                                <tbody>\r\n                                                    <tr>\r\n                                                        <td class=\"v-container-padding-padding\" style=\"overflow-wrap:break-word;word-break:break-word;font-family:'Open Sans',sans-serif;\" align=\"left\">\r\n\r\n                                                            <table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\">\r\n                                                                <tr>\r\n                                                                    <td class=\"v-text-align\" style=\"padding-right: 0px;padding-left: 0px;\" align=\"center\">\r\n\r\n                                                                        <img align=\"center\" border=\"0\" src=\"https://buildingonsuccess.net/demos/conference/images/logotranss.png\" alt=\"Logo\" title=\"Logo\" style=\"outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: inline-block !important;border: none;height: auto;float: none;max-width: 272.6px;\" width=\"272.6\" class=\"v-src-width v-src-max-width\" />\r\n\r\n                                                                    </td>\r\n                                                                </tr>\r\n                                                            </table>\r\n\r\n                                                        </td>\r\n                                                    </tr>\r\n                                                </tbody>\r\n                                            </table>\r\n\r\n                                            <!--[if (!mso)&(!IE)]><!-->\r\n                                        </div><!--<![endif]-->\r\n                                    </div>\r\n                                </div>\r\n                                <!--[if (mso)|(IE)]></td><![endif]-->\r\n                                <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->\r\n                            </div>\r\n                        </div>\r\n                    </div>\r\n\r\n\r\n\r\n                    <div class=\"u-row-container\" style=\"padding: 0px;background-color: transparent\">\r\n                        <div class=\"u-row\" style=\"Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;\">\r\n                            <div style=\"border-collapse: collapse;display: table;width: 100%;height: 100%;background-image: url('images/image-6.png');background-repeat: no-repeat;background-position: center top;background-color: transparent;\">\r\n                                <!--[if (mso)|(IE)]><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td style=\"padding: 0px;background-color: transparent;\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"width:600px;\"><tr style=\"background-image: url('images/image-6.png');background-repeat: no-repeat;background-position: center top;background-color: transparent;\"><![endif]-->\r\n                                <!--[if (mso)|(IE)]><td align=\"center\" width=\"600\" style=\"background-color: #ffffff;width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;\" valign=\"top\"><![endif]-->\r\n                                <div class=\"u-col u-col-100\" style=\"max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;\">\r\n                                    <div style=\"background-color: #ffffff;height: 100%;width: 100% !important;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;\">\r\n                                        <!--[if (!mso)&(!IE)]><!--><div style=\"height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;\">\r\n                                            <!--<![endif]-->\r\n\r\n                                            <table style=\"font-family:'Open Sans',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">\r\n                                                <tbody>\r\n                                                    <tr>\r\n                                                        <td class=\"v-container-padding-padding\" style=\"overflow-wrap:break-word;word-break:break-word;padding:60px 10px 10px;font-family:'Open Sans',sans-serif;\" align=\"left\">\r\n\r\n                                                            <div class=\"v-text-align\" style=\"line-height: 170%; text-align: center; word-wrap: break-word;\">\r\n                                                                <p style=\"font-size: 14px; line-height: 170%;\"><span style=\"font-size: 20px; line-height: 34px;\"><strong><span style=\"line-height: 34px; font-size: 20px;\">Hello<br>Dr. [[contactname]],</span></strong></span></p>\r\n                                                            </div>\r\n\r\n                                                        </td>\r\n                                                    </tr>\r\n                                                </tbody>\r\n                                            </table>\r\n\r\n                                            <table id=\"u_content_text_3\" style=\"font-family:'Open Sans',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">\r\n                                                <tbody>\r\n                                                    <tr>\r\n                                                        <td class=\"v-container-padding-padding\" style=\"overflow-wrap:break-word;word-break:break-word;padding:10px 100px 20px;font-family:'Open Sans',sans-serif;\" align=\"left\">\r\n\r\n                                                            <div class=\"v-text-align\" style=\"line-height: 170%; text-align: center; word-wrap: break-word;\">\r\n                                                                <p style=\"font-size: 14px; line-height: 170%;\"><span style=\"font-size: 16px; line-height: 27.2px;\">Your Registration code is </span><br><strong style=\"font-size: 16px; line-height: 27.2px;\">[[code]] </strong><br><span style=\"font-size: 16px; line-height: 27.2px;\">You will need this code to verify your registration at the conference.</span><br><span style=\"font-size: 16px; line-height: 27.2px;\">Or you can use this QR Code instead</span></p>\r\n                                                            </div>\r\n\r\n                                                        </td>\r\n                                                    </tr>\r\n                                                    <tr>\r\n                                                        <td class=\"v-container-padding-padding\" style=\"overflow-wrap:break-word;word-break:break-word;padding:10px 100px 20px;font-family:'Open Sans',sans-serif;\" align=\"left\">\r\n\r\n                                                            <div class=\"v-text-align\" style=\"line-height: 170%; text-align: center; word-wrap: break-word;\">\r\n                                                                <img align=\"center\" border=\"0\" src=\"[[qrcode]]\" alt=\"Logo\" title=\"Logo\" style=\"outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: inline-block !important;border: none;height: auto;float: none;max-width: 272.6px;\" width=\"272.6\" class=\"v-src-width v-src-max-width\" />\r\n                                                               <br>\r\n         <br>\r\n                                                         <a role=\"button\" class=\"btn btn-primary\" href=\"[[downloadqr]]\">Download QR Code</a>\r\n                                                            </div>\r\n\r\n\r\n                                                        </td>\r\n                                                    </tr>\r\n                                                </tbody>\r\n                                            </table>\r\n                                        </div>\r\n                                    </div>\r\n                                </div>\r\n\r\n                            </div>\r\n                        </div>\r\n                    </div>\r\n\r\n\r\n\r\n                  \r\n\r\n\r\n\r\n                    <div class=\"u-row-container\" style=\"padding: 0px;background-color: transparent\">\r\n                        <div class=\"u-row\" style=\"Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;\">\r\n                            <div style=\"border-collapse: collapse;display: table;width: 100%;height: 100%;background-image: url('images/image-5.png');background-repeat: no-repeat;background-position: center top;background-color: transparent;\">\r\n                                <!--[if (mso)|(IE)]><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td style=\"padding: 0px;background-color: transparent;\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"width:600px;\"><tr style=\"background-image: url('images/image-5.png');background-repeat: no-repeat;background-position: center top;background-color: transparent;\"><![endif]-->\r\n                                <!--[if (mso)|(IE)]><td align=\"center\" width=\"300\" style=\"background-color: #f1c40f;width: 300px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;\" valign=\"top\"><![endif]-->\r\n                                <div class=\"u-col u-col-50\" style=\"max-width: 320px;min-width: 300px;display: table-cell;vertical-align: top;\">\r\n                                    <div style=\"background-color: #f1c40f;height: 100%;width: 100% !important;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;\">\r\n                                        <!--[if (!mso)&(!IE)]><!--><div style=\"height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;\">\r\n                                            <!--<![endif]-->\r\n\r\n                                            <table id=\"u_content_heading_2\" style=\"font-family:'Open Sans',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">\r\n                                                <tbody>\r\n                                                    <tr>\r\n                                                        <td class=\"v-container-padding-padding\" style=\"overflow-wrap:break-word;word-break:break-word;padding:30px 10px 0px 50px;font-family:'Open Sans',sans-serif;\" align=\"left\">\r\n\r\n                                                            <h1 class=\"v-text-align\" style=\"margin: 0px; line-height: 140%; text-align: left; word-wrap: break-word; font-weight: normal; font-family: 'Rubik',sans-serif; font-size: 22px;\">\r\n                                                                <div>\r\n                                                                    <div><strong>AME</strong> & Teoxane</div>\r\n                                                                </div>\r\n                                                            </h1>\r\n\r\n                                                        </td>\r\n                                                    </tr>\r\n                                                </tbody>\r\n                                            </table>\r\n\r\n                                      \r\n\r\n                                            <!--[if (!mso)&(!IE)]><!-->\r\n                                        </div><!--<![endif]-->\r\n                                    </div>\r\n                                </div>\r\n                                <!--[if (mso)|(IE)]></td><![endif]-->\r\n                                <!--[if (mso)|(IE)]><td align=\"center\" width=\"300\" style=\"background-color: #f1c40f;width: 300px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;\" valign=\"top\"><![endif]-->\r\n                                <div class=\"u-col u-col-50\" style=\"max-width: 320px;min-width: 300px;display: table-cell;vertical-align: top;\">\r\n                                    <div style=\"background-color: #f1c40f;height: 100%;width: 100% !important;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;\">\r\n                                        <!--[if (!mso)&(!IE)]><!--><div style=\"height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;\">\r\n                                            <!--<![endif]-->\r\n\r\n                                            <table id=\"u_content_text_5\" style=\"font-family:'Open Sans',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">\r\n                                                <tbody>\r\n                                                    <tr>\r\n                                                        <td class=\"v-container-padding-padding\" style=\"overflow-wrap:break-word;word-break:break-word;padding:31px 50px 30px 10px;font-family:'Open Sans',sans-serif;\" align=\"left\">\r\n\r\n                                                            <div class=\"v-text-align\" style=\"line-height: 170%; text-align: right; word-wrap: break-word;\">\r\n                                                                <a href=\"https://goo.gl/maps/1qv5jDBhuK6L22dr9\" target=\"_blank\">location</a>\r\n                                                            </div>\r\n\r\n                                                        </td>\r\n                                                    </tr>\r\n                                                </tbody>\r\n                                            </table>\r\n\r\n                                            <!--[if (!mso)&(!IE)]><!-->\r\n                                        </div><!--<![endif]-->\r\n                                    </div>\r\n                                </div>\r\n                                <!--[if (mso)|(IE)]></td><![endif]-->\r\n                                <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->\r\n                            </div>\r\n                        </div>\r\n                    </div>\r\n\r\n\r\n\r\n                \r\n\r\n\r\n                    <!--[if (mso)|(IE)]></td></tr></table><![endif]-->\r\n                </td>\r\n            </tr>\r\n        </tbody>\r\n    </table>\r\n    <!--[if mso]></div><![endif]-->\r\n    <!--[if IE]></div><![endif]-->\r\n</body>\r\n\r\n</html>\r\n";
            string bodyy = body.Replace("[[contactname]]", db.contact.Find(t.ContactId).ContactName).Replace("[[code]]", t.RegistrationCode.ToString()).Replace("[[qrcode]]", "https://{{URL}}.com/image/"+t.Id+".png").Replace("[[downloadqr]]", "https://{{URL}}.com/Dev/DownloadQR/"+t.Id);
            MailMessage m = new MailMessage();

            MailAddress m1 = new MailAddress(t.Email);
            MailAddress m2 = new MailAddress("ameksa@tedsaudia.com");

            m.To.Add(m1);
            m.To.Add(m2);        
            m.Subject = "TED AME Registration Code";
            m.From = new MailAddress("ame@tedsaudia.com");
            m.Sender = new MailAddress("ame@tedsaudia.com");
            m.Bcc.Add("ame@tedsaudia.com");
            m.Body = bodyy;
            m.IsBodyHtml = true;
            m.Priority = MailPriority.High;
            SmtpClient smtp = new SmtpClient("smtp.hostinger.com", 587);
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential("ame@tedsaudia.com", "Adminpass@1");
            smtp.Send(m);
            return true;
        }

        public TedDataModel AnonRegister(SaamAnonRegisterModel obj)
        {
            Contact c = new Contact();
            c.AccountId = 738;
            c.ContactTypeId = 10;
            c.ContactName = obj.FullName;
            c.Email = obj.Email;
            c.MobileNumber = obj.PhoneNumber;
            c.DistrictId = 1;
            c.RelationshipNote = "TED Registration";
            c.Guidd = Guid.NewGuid();
            db.contact.Add(c);
            db.SaveChanges();
            EventTravelRequest etr = new EventTravelRequest();
            etr.TopConfirmed = true;
            etr.TopAction = true;
            etr.EventId = 9;
            etr.ContactId = c.Id;
            etr.Confirmed = true;
            etr.WayInDestinationId = 304;
            etr.WayInDeparture = new DateTime(2022,11,8);
            etr.WayOutCityId = 304;
            etr.WayOutDeparture = new DateTime(2022,11,10);
            etr.ExtendIdentityUserId = "03bbdbda-6e3b-4c17-9aba-6cc966cbf3c9";
            db.EventTravelRequest.Add(etr);
            TedDataModel tdm = Start(c.Guidd.ToString());
            tdm.g = c.Guidd.ToString();
            TED t = db.ted.Find(tdm.Id);
            t.IsComing = true;
            t.IsNotComing = false;
            t.Email = c.Email;
            t.PhoneNumber = c.MobileNumber;
            t.Date = ti.GetCurrentTime();
            db.SaveChanges();
            QRCodeGenerator qr = new QRCodeGenerator();
            Bitmap qh = QRCodeHelper.GetQRCode("https://{{DashboardURL}}/Organizer/LongCode.html?" + t.guidd.ToString(), 16, Color.Black, Color.White, QRCodeGenerator.ECCLevel.H);
            qh.Save(Directory.GetCurrentDirectory() + "/image/" + t.Id + ".png", ImageFormat.Png);
            string body = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional //EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\r\n<html xmlns=\"http://www.w3.org/1999/xhtml\" xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:o=\"urn:schemas-microsoft-com:office:office\">\r\n<head>\r\n    <!--[if gte mso 9]>\r\n    <xml>\r\n      <o:OfficeDocumentSettings>\r\n        <o:AllowPNG/>\r\n        <o:PixelsPerInch>96</o:PixelsPerInch>\r\n      </o:OfficeDocumentSettings>\r\n    </xml>\r\n    <![endif]-->\r\n    <meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\">\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\r\n    <meta name=\"x-apple-disable-message-reformatting\">\r\n    <!--[if !mso]><!-->\r\n    <meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\"><!--<![endif]-->\r\n    <title></title>\r\n\r\n    <style type=\"text/css\">\r\n        @media only screen and (min-width: 620px) {\r\n            .u-row {\r\n                width: 600px !important;\r\n            }\r\n\r\n                .u-row .u-col {\r\n                    vertical-align: top;\r\n                }\r\n\r\n                .u-row .u-col-50 {\r\n                    width: 300px !important;\r\n                }\r\n\r\n                .u-row .u-col-100 {\r\n                    width: 600px !important;\r\n                }\r\n        }\r\n\r\n        @media (max-width: 620px) {\r\n            .u-row-container {\r\n                max-width: 100% !important;\r\n                padding-left: 0px !important;\r\n                padding-right: 0px !important;\r\n            }\r\n\r\n            .u-row .u-col {\r\n                min-width: 320px !important;\r\n                max-width: 100% !important;\r\n                display: block !important;\r\n            }\r\n\r\n            .u-row {\r\n                width: calc(100% - 40px) !important;\r\n            }\r\n\r\n            .u-col {\r\n                width: 100% !important;\r\n            }\r\n\r\n                .u-col > div {\r\n                    margin: 0 auto;\r\n                }\r\n        }\r\n\r\n        body {\r\n            margin: 0;\r\n            padding: 0;\r\n        }\r\n\r\n        table,\r\n        tr,\r\n        td {\r\n            vertical-align: top;\r\n            border-collapse: collapse;\r\n        }\r\n\r\n        p {\r\n            margin: 0;\r\n        }\r\n\r\n        .ie-container table,\r\n        .mso-container table {\r\n            table-layout: fixed;\r\n        }\r\n\r\n        * {\r\n            line-height: inherit;\r\n        }\r\n\r\n        a[x-apple-data-detectors='true'] {\r\n            color: inherit !important;\r\n            text-decoration: none !important;\r\n        }\r\n\r\n        table, td {\r\n            color: #000000;\r\n        }\r\n\r\n        #u_body a {\r\n            color: #0000ee;\r\n            text-decoration: underline;\r\n        }\r\n\r\n        #u_content_text_4 a {\r\n            color: #f1c40f;\r\n        }\r\n\r\n        @media (max-width: 480px) {\r\n            /* #u_content_image_1 .v-src-width {\r\n                width: auto !important;\r\n            }\r\n\r\n            #u_content_image_1 .v-src-max-width {\r\n                max-width: 25% !important;\r\n            } */\r\n\r\n            #u_content_text_3 .v-container-padding-padding {\r\n                padding: 10px 20px 20px !important;\r\n            }\r\n\r\n            #u_content_button_1 .v-size-width {\r\n                width: 65% !important;\r\n            }\r\n\r\n            #u_content_text_2 .v-container-padding-padding {\r\n                padding: 20px 20px 60px !important;\r\n            }\r\n\r\n            #u_content_text_4 .v-container-padding-padding {\r\n                padding: 60px 20px !important;\r\n            }\r\n\r\n            #u_content_heading_2 .v-container-padding-padding {\r\n                padding: 30px 10px 0px !important;\r\n            }\r\n\r\n            #u_content_heading_2 .v-text-align {\r\n                text-align: center !important;\r\n            }\r\n\r\n            #u_content_social_1 .v-container-padding-padding {\r\n                padding: 10px 10px 10px 98px !important;\r\n            }\r\n\r\n            #u_content_text_5 .v-container-padding-padding {\r\n                padding: 10px 20px 30px !important;\r\n            }\r\n\r\n            #u_content_text_5 .v-text-align {\r\n                text-align: center !important;\r\n            }\r\n        }\r\n    </style>\r\n\r\n\r\n\r\n    <!--[if !mso]><!-->\r\n    <link href=\"https://fonts.googleapis.com/css?family=Open+Sans:400,700&display=swap\" rel=\"stylesheet\" type=\"text/css\">\r\n    <link href=\"https://fonts.googleapis.com/css?family=Rubik:400,700&display=swap\" rel=\"stylesheet\" type=\"text/css\"><!--<![endif]-->\r\n\r\n</head>\r\n\r\n<body class=\"clean-body u_body\" style=\"margin: 0;padding: 0;-webkit-text-size-adjust: 100%;background-color: #000000;color: #000000\">\r\n    <!--[if IE]><div class=\"ie-container\"><![endif]-->\r\n    <!--[if mso]><div class=\"mso-container\"><![endif]-->\r\n    <table id=\"u_body\" style=\"border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;min-width: 320px;Margin: 0 auto;background-color: #000000;width:100%\" cellpadding=\"0\" cellspacing=\"0\">\r\n        <tbody>\r\n            <tr style=\"vertical-align: top\">\r\n                <td style=\"word-break: break-word;border-collapse: collapse !important;vertical-align: top\">\r\n                    <!--[if (mso)|(IE)]><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td align=\"center\" style=\"background-color: #000000;\"><![endif]-->\r\n\r\n\r\n                    <div class=\"u-row-container\" style=\"padding: 0px;background-color: transparent\">\r\n                        <div class=\"u-row\" style=\"Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;\">\r\n                            <div style=\"border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;\">\r\n                                <!--[if (mso)|(IE)]><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td style=\"padding: 0px;background-color: transparent;\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"width:600px;\"><tr style=\"background-color: transparent;\"><![endif]-->\r\n                                <!--[if (mso)|(IE)]><td align=\"center\" width=\"600\" style=\"width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\" valign=\"top\"><![endif]-->\r\n                                <div class=\"u-col u-col-100\" style=\"max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;\">\r\n                                    <div style=\"height: 100%;width: 100% !important;\">\r\n                                        <!--[if (!mso)&(!IE)]><!--><div style=\"height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\">\r\n                                            <!--<![endif]-->\r\n\r\n                                            <table id=\"u_content_image_1\" style=\"font-family:'Open Sans',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">\r\n                                                <tbody>\r\n                                                    <tr>\r\n                                                        <td class=\"v-container-padding-padding\" style=\"overflow-wrap:break-word;word-break:break-word;font-family:'Open Sans',sans-serif;\" align=\"left\">\r\n\r\n                                                            <table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\">\r\n                                                                <tr>\r\n                                                                    <td class=\"v-text-align\" style=\"padding-right: 0px;padding-left: 0px;\" align=\"center\">\r\n\r\n                                                                        <img align=\"center\" border=\"0\" src=\"https://buildingonsuccess.net/demos/conference/images/logotranss.png\" alt=\"Logo\" title=\"Logo\" style=\"outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: inline-block !important;border: none;height: auto;float: none;max-width: 272.6px;\" width=\"272.6\" class=\"v-src-width v-src-max-width\" />\r\n\r\n                                                                    </td>\r\n                                                                </tr>\r\n                                                            </table>\r\n\r\n                                                        </td>\r\n                                                    </tr>\r\n                                                </tbody>\r\n                                            </table>\r\n\r\n                                            <!--[if (!mso)&(!IE)]><!-->\r\n                                        </div><!--<![endif]-->\r\n                                    </div>\r\n                                </div>\r\n                                <!--[if (mso)|(IE)]></td><![endif]-->\r\n                                <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->\r\n                            </div>\r\n                        </div>\r\n                    </div>\r\n\r\n\r\n\r\n                    <div class=\"u-row-container\" style=\"padding: 0px;background-color: transparent\">\r\n                        <div class=\"u-row\" style=\"Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;\">\r\n                            <div style=\"border-collapse: collapse;display: table;width: 100%;height: 100%;background-image: url('images/image-6.png');background-repeat: no-repeat;background-position: center top;background-color: transparent;\">\r\n                                <!--[if (mso)|(IE)]><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td style=\"padding: 0px;background-color: transparent;\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"width:600px;\"><tr style=\"background-image: url('images/image-6.png');background-repeat: no-repeat;background-position: center top;background-color: transparent;\"><![endif]-->\r\n                                <!--[if (mso)|(IE)]><td align=\"center\" width=\"600\" style=\"background-color: #ffffff;width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;\" valign=\"top\"><![endif]-->\r\n                                <div class=\"u-col u-col-100\" style=\"max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;\">\r\n                                    <div style=\"background-color: #ffffff;height: 100%;width: 100% !important;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;\">\r\n                                        <!--[if (!mso)&(!IE)]><!--><div style=\"height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;\">\r\n                                            <!--<![endif]-->\r\n\r\n                                            <table style=\"font-family:'Open Sans',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">\r\n                                                <tbody>\r\n                                                    <tr>\r\n                                                        <td class=\"v-container-padding-padding\" style=\"overflow-wrap:break-word;word-break:break-word;padding:60px 10px 10px;font-family:'Open Sans',sans-serif;\" align=\"left\">\r\n\r\n                                                            <div class=\"v-text-align\" style=\"line-height: 170%; text-align: center; word-wrap: break-word;\">\r\n                                                                <p style=\"font-size: 14px; line-height: 170%;\"><span style=\"font-size: 20px; line-height: 34px;\"><strong><span style=\"line-height: 34px; font-size: 20px;\">Hello<br>Dr. [[contactname]],</span></strong></span></p>\r\n                                                            </div>\r\n\r\n                                                        </td>\r\n                                                    </tr>\r\n                                                </tbody>\r\n                                            </table>\r\n\r\n                                            <table id=\"u_content_text_3\" style=\"font-family:'Open Sans',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">\r\n                                                <tbody>\r\n                                                    <tr>\r\n                                                        <td class=\"v-container-padding-padding\" style=\"overflow-wrap:break-word;word-break:break-word;padding:10px 100px 20px;font-family:'Open Sans',sans-serif;\" align=\"left\">\r\n\r\n                                                            <div class=\"v-text-align\" style=\"line-height: 170%; text-align: center; word-wrap: break-word;\">\r\n                                                                <p style=\"font-size: 14px; line-height: 170%;\"><span style=\"font-size: 16px; line-height: 27.2px;\">Your Registration code is </span><br><strong style=\"font-size: 16px; line-height: 27.2px;\">[[code]] </strong><br><span style=\"font-size: 16px; line-height: 27.2px;\">You will need this code to verify your registration at the conference.</span><br><span style=\"font-size: 16px; line-height: 27.2px;\">Or you can use this QR Code instead</span></p>\r\n                                                            </div>\r\n\r\n                                                        </td>\r\n                                                    </tr>\r\n                                                    <tr>\r\n                                                        <td class=\"v-container-padding-padding\" style=\"overflow-wrap:break-word;word-break:break-word;padding:10px 100px 20px;font-family:'Open Sans',sans-serif;\" align=\"left\">\r\n\r\n                                                            <div class=\"v-text-align\" style=\"line-height: 170%; text-align: center; word-wrap: break-word;\">\r\n                                                                <img align=\"center\" border=\"0\" src=\"[[qrcode]]\" alt=\"Logo\" title=\"Logo\" style=\"outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: inline-block !important;border: none;height: auto;float: none;max-width: 272.6px;\" width=\"272.6\" class=\"v-src-width v-src-max-width\" />\r\n                                                               <br>\r\n         <br>\r\n                                                         <a role=\"button\" class=\"btn btn-primary\" href=\"[[downloadqr]]\">Download QR Code</a>\r\n                                                            </div>\r\n\r\n\r\n                                                        </td>\r\n                                                    </tr>\r\n                                                </tbody>\r\n                                            </table>\r\n                                        </div>\r\n                                    </div>\r\n                                </div>\r\n\r\n                            </div>\r\n                        </div>\r\n                    </div>\r\n\r\n\r\n\r\n                  \r\n\r\n\r\n\r\n                    <div class=\"u-row-container\" style=\"padding: 0px;background-color: transparent\">\r\n                        <div class=\"u-row\" style=\"Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;\">\r\n                            <div style=\"border-collapse: collapse;display: table;width: 100%;height: 100%;background-image: url('images/image-5.png');background-repeat: no-repeat;background-position: center top;background-color: transparent;\">\r\n                                <!--[if (mso)|(IE)]><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td style=\"padding: 0px;background-color: transparent;\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"width:600px;\"><tr style=\"background-image: url('images/image-5.png');background-repeat: no-repeat;background-position: center top;background-color: transparent;\"><![endif]-->\r\n                                <!--[if (mso)|(IE)]><td align=\"center\" width=\"300\" style=\"background-color: #f1c40f;width: 300px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;\" valign=\"top\"><![endif]-->\r\n                                <div class=\"u-col u-col-50\" style=\"max-width: 320px;min-width: 300px;display: table-cell;vertical-align: top;\">\r\n                                    <div style=\"background-color: #f1c40f;height: 100%;width: 100% !important;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;\">\r\n                                        <!--[if (!mso)&(!IE)]><!--><div style=\"height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;\">\r\n                                            <!--<![endif]-->\r\n\r\n                                            <table id=\"u_content_heading_2\" style=\"font-family:'Open Sans',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">\r\n                                                <tbody>\r\n                                                    <tr>\r\n                                                        <td class=\"v-container-padding-padding\" style=\"overflow-wrap:break-word;word-break:break-word;padding:30px 10px 0px 50px;font-family:'Open Sans',sans-serif;\" align=\"left\">\r\n\r\n                                                            <h1 class=\"v-text-align\" style=\"margin: 0px; line-height: 140%; text-align: left; word-wrap: break-word; font-weight: normal; font-family: 'Rubik',sans-serif; font-size: 22px;\">\r\n                                                                <div>\r\n                                                                    <div><strong>AME</strong> & Teoxane</div>\r\n                                                                </div>\r\n                                                            </h1>\r\n\r\n                                                        </td>\r\n                                                    </tr>\r\n                                                </tbody>\r\n                                            </table>\r\n\r\n                                      \r\n\r\n                                            <!--[if (!mso)&(!IE)]><!-->\r\n                                        </div><!--<![endif]-->\r\n                                    </div>\r\n                                </div>\r\n                                <!--[if (mso)|(IE)]></td><![endif]-->\r\n                                <!--[if (mso)|(IE)]><td align=\"center\" width=\"300\" style=\"background-color: #f1c40f;width: 300px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;\" valign=\"top\"><![endif]-->\r\n                                <div class=\"u-col u-col-50\" style=\"max-width: 320px;min-width: 300px;display: table-cell;vertical-align: top;\">\r\n                                    <div style=\"background-color: #f1c40f;height: 100%;width: 100% !important;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;\">\r\n                                        <!--[if (!mso)&(!IE)]><!--><div style=\"height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;\">\r\n                                            <!--<![endif]-->\r\n\r\n                                            <table id=\"u_content_text_5\" style=\"font-family:'Open Sans',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">\r\n                                                <tbody>\r\n                                                    <tr>\r\n                                                        <td class=\"v-container-padding-padding\" style=\"overflow-wrap:break-word;word-break:break-word;padding:31px 50px 30px 10px;font-family:'Open Sans',sans-serif;\" align=\"left\">\r\n\r\n                                                            <div class=\"v-text-align\" style=\"line-height: 170%; text-align: right; word-wrap: break-word;\">\r\n                                                                <a href=\"https://goo.gl/maps/1qv5jDBhuK6L22dr9\" target=\"_blank\">location</a>\r\n                                                            </div>\r\n\r\n                                                        </td>\r\n                                                    </tr>\r\n                                                </tbody>\r\n                                            </table>\r\n\r\n                                            <!--[if (!mso)&(!IE)]><!-->\r\n                                        </div><!--<![endif]-->\r\n                                    </div>\r\n                                </div>\r\n                                <!--[if (mso)|(IE)]></td><![endif]-->\r\n                                <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->\r\n                            </div>\r\n                        </div>\r\n                    </div>\r\n\r\n\r\n\r\n                \r\n\r\n\r\n                    <!--[if (mso)|(IE)]></td></tr></table><![endif]-->\r\n                </td>\r\n            </tr>\r\n        </tbody>\r\n    </table>\r\n    <!--[if mso]></div><![endif]-->\r\n    <!--[if IE]></div><![endif]-->\r\n</body>\r\n\r\n</html>\r\n";
            string bodyy = body.Replace("[[contactname]]", db.contact.Find(t.ContactId).ContactName).Replace("[[code]]", t.RegistrationCode.ToString()).Replace("[[qrcode]]", "https://{{URL}}.com/image/" + t.Id + ".png").Replace("[[downloadqr]]", "https://{{URL}}.com/Dev/DownloadQR/" + t.Id);
            MailMessage m = new MailMessage();

            MailAddress m1 = new MailAddress(t.Email);
            MailAddress m2 = new MailAddress("ameksa@tedsaudia.com");

            m.To.Add(m1);
            m.To.Add(m2);
            m.Subject = "TED AME Registration Code";
            m.From = new MailAddress("ame@tedsaudia.com");
            m.Sender = new MailAddress("ame@tedsaudia.com");
            m.Bcc.Add("ame@tedsaudia.com");
            m.Body = bodyy;
            m.IsBodyHtml = true;
            m.Priority = MailPriority.High;
            SmtpClient smtp = new SmtpClient("smtp.hostinger.com", 587);
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential("ame@tedsaudia.com", "Adminpass@1");
            smtp.Send(m);
            return tdm;
        }

        public bool Attendance(int id)
        {
            TED t = db.ted.Find(id);
            t.Attend = true;
            t.AttendTime = ti.GetCurrentTime();
            db.SaveChanges();
            return true;
        }

        public TedRegisterModel ChangeInfo(int id)
        {
            TED t = db.ted.Find(id);
            TedRegisterModel res = new TedRegisterModel();
            res.Id = id;
            res.Email = t.Email;
            res.PhoneNumber = t.PhoneNumber;
            db.SaveChanges();

            return res;
            
        }

        public bool ChangeMind(int id)
        {
            TED t = db.ted.Find(id);
            t.IsComing = false;
            t.IsNotComing = false;
            db.SaveChanges();
            return true;
        }

        public List<TedAttendanceTable> GetAttendanceReport()
        {
            List<TedAttendanceTable> res = new List<TedAttendanceTable>();
            List<TED> t = db.ted.Where(a => a.IsComing == true).ToList();
            foreach (var item in t)
            {
                TedAttendanceTable r = new TedAttendanceTable();
                r.Name = db.contact.Find(item.ContactId).ContactName;
                r.Code = item.RegistrationCode;
                if (item.Attend == true)
                {
                    r.att = true;
                    r.time = (DateTime)item.AttendTime;
                }
                else
                {
                    r.att = false;
                    r.time = new DateTime(1993, 03, 03);
                }
                res.Add(r);
            }
            return res.OrderBy(a => a.Name).ToList();
        }

        public TedRegistrationData GetDataByCode(int c)
        {

            TedRegistrationData res = new TedRegistrationData();
                TED t = db.ted.Where(a => a.RegistrationCode == c).FirstOrDefault();
                if (t == null)
                {
                    res.Id = 0;
                    res.Name = "Wrong Code";
                    res.Date = "Wrong Code";
                    res.Email = "Wrong Code";
                    res.Phone = "Wrong Code";
                    res.Code = 0;

                }
                else
                {
                    if (t.IsComing == false)
                    {
                        res.Id = 0;
                        res.Name = db.contact.Find(t.ContactId).ContactName;
                        res.Date = "Not Registered";
                        res.Email = "Not Registered";
                        res.Phone = "Not Registered";
                        res.Code = 0;
                    }
                    else
                    {
                        if (t.Attend == true)
                        {
                            res.Id = 0;
                            res.Name = db.contact.Find(t.ContactId).ContactName;
                            res.Date = "This doctor attendance has already confirmed";
                            res.Email = "This doctor attendance has already confirmed";
                            res.Phone = "This doctor attendance has already confirmed";
                            res.Code = 0;
                        }
                        else
                        {
                            string e = t.Email.Split('@')[0];
                            string a = t.Email.Split('@')[1];
                            res.Id = t.Id;
                            res.Name = db.contact.Find(t.ContactId).ContactName;
                            res.Date = t.Date.ToString("dd/MM/yyyy - hh:mm tt");
                            res.Phone = "**********" + t.PhoneNumber.Substring(t.PhoneNumber.Length - 4);
                            res.Email = "**********" + e.Substring(e.Length - 4) + "@" + a;
                            res.Code = t.RegistrationCode;
                        }
                    }
                }

            return res;
        }

        public TedRegistrationData GetDataByGuid(string g)
        {
            TedRegistrationData res = new TedRegistrationData();
            if (g.Length == 36 || g.Length == 32)
            {
                Guid gg = new Guid(g);

                TED t = db.ted.Where(a => a.guidd == gg).FirstOrDefault();
                if (t == null)
                {
                    res.Id = 0;
                    res.Name = "Wrong Code";
                    res.Date = "Wrong Code";
                    res.Email = "Wrong Code";
                    res.Phone = "Wrong Code";
                    res.Code = 0;
                   
                }
                else
                {
                    if (t.IsComing == false)
                    {
                        res.Id = 0;
                        res.Name = db.contact.Find(t.ContactId).ContactName;
                        res.Date = "Not Registered";
                        res.Email = "Not Registered";
                        res.Phone = "Not Registered";
                        res.Code = 0;
                    }
                    else
                    {
                        if (t.Attend == true)
                        {
                            res.Id = 0;
                            res.Name = db.contact.Find(t.ContactId).ContactName;
                            res.Date = "This doctor attendance has already confirmed";
                            res.Email = "This doctor attendance has already confirmed";
                            res.Phone = "This doctor attendance has already confirmed";
                            res.Code = 0;
                        }
                        else
                        {
                            string e = t.Email.Split('@')[0];
                            string a = t.Email.Split('@')[1];
                            res.Id = t.Id;
                            res.Name = db.contact.Find(t.ContactId).ContactName;
                            res.Date = t.Date.ToString("dd/MM/yyyy - hh:mm tt");
                            res.Phone = "**********" + t.PhoneNumber.Substring(t.PhoneNumber.Length - 4);
                            res.Email = "**********" + e.Substring(e.Length - 4) + "@" + a;
                            res.Code = t.RegistrationCode;
                        }
                    }
                }
                
            }
            else
            {
                res.Id = 0;
                res.Name = "Wrong Code";
                res.Date = "Wrong Code";
                res.Email = "Wrong Code";
                res.Phone = "Wrong Code";
                res.Code = 0;
            }


                return res;
        }

        public string GetDoctorName(string g)
        {
            Guid gg = new Guid(g);
            Contact c = db.contact.Where(a => a.Guidd == gg).FirstOrDefault();
            return c.ContactName;
        }

        public List<TedReport> GetTedReport()
        {
            List<TED> t = db.ted.ToList();

            List<TedReport> res = new List<TedReport>();

            foreach (var item in t)
            {
                TedReport r = new TedReport();
                r.Id = item.Id;
                r.Name = db.contact.Find(item.ContactId).ContactName;
                if (item.IsComing == true)
                {
                    r.Registered = true;
                    r.RegistrationDate = item.Date.ToString("dd MMMM yyyy - hh:mm tt");
                }
                if (item.Attend == true)
                {
                    r.Attended = true;
                    DateTime tt = (DateTime)item.AttendTime;
                    r.AttendanceTime = tt.ToString("hh:mm tt");
                }
                res.Add(r);
            }

            return res.OrderBy(a => a.Name).ToList();

        }

        public bool Reject(int id)
        {
            TED t = db.ted.Find(id);
            t.IsComing = false;
            t.IsNotComing = true;
            t.Date = ti.GetCurrentTime();
            db.SaveChanges();
            return true;
        }

        public TedDataModel Start(string gu)
        {
            Guid g = new Guid(gu);
            Contact c = db.contact.Where(a => a.Guidd == g).FirstOrDefault();
            TED t = db.ted.Where(a => a.ContactId == c.Id).FirstOrDefault();
            if (t == null)
            {
                List<TED> tedlist = db.ted.ToList();

                int code;
                if (tedlist == null || tedlist.Count == 0)
                {
                    code = 1001;
                }
                else
                {
                    int maxcode = db.ted.Select(a => a.RegistrationCode).Max();
                    code = (int)maxcode + 1;
                }

                TED obj = new TED();
                obj.ContactId = c.Id;
                obj.Date = ti.GetCurrentTime();
                obj.RegistrationCode = code;
                obj.guidd = g;
                db.ted.Add(obj);
                db.SaveChanges();
                TedDataModel res = new TedDataModel();
                res.Id = obj.Id;
                res.ContactId = obj.ContactId;
                res.ContactName = c.ContactName;
                res.Email = c.Email;
                res.PhoneNumber = c.MobileNumber;
                res.IsNotComing = obj.IsNotComing;
                res.IsComing = obj.IsComing;
                return res;
            }
            else
            {
                TedDataModel res = new TedDataModel();
                res.Id = t.Id;
                res.ContactId = t.ContactId;
                res.ContactName = db.contact.Find(t.ContactId).ContactName;
                res.Email = c.Email;
                res.PhoneNumber = c.MobileNumber;
                res.IsNotComing = t.IsNotComing;
                res.IsComing = t.IsComing;
                return res;
            }
        }

        public bool SubmitChangeInfo(TedRegisterModel m)
        {
            TED t = db.ted.Find(m.Id);
            t.IsComing = true;
            t.IsNotComing = false;
            t.Email = m.Email;
            t.PhoneNumber = m.PhoneNumber;
            db.SaveChanges();
            return true;
        }
    }
}

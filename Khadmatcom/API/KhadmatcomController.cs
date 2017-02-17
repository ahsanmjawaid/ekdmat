﻿using MailChimp;
using MailChimp.Helper;
using MailChimp.Lists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading;
using System.Web.Configuration;
using System.Web.Http;
using Khadmatcom.Services;
using HyperPayClient;

namespace Khadmatcom.API
{


    public class KhadmatcomController : ApiController
    {
        /*
         name: $('#txtContactEmail').val(),
                        email: $('#txtContactName').val(),
                        phone: $('#txtPhone').text(),
                        subject: $('select[id$=ddlSubject] :selected').val(),
                        message: $('#txtMessage').val(),
             */

        [HttpGet]
        [ActionName("ContactUs")]
        public bool ContactUs(string name, string email, string phone, string subject, string message)
        {
            Dictionary<string, string> keysValues = new Dictionary<string, string>
            {
                {"name", name},
                {"email", email},
                {"phone", phone},
                {"subject", subject},
                {"message", message}
            };
            string toAddress = WebConfigurationManager.AppSettings["ContactUsEmail"];
            string adminEmail = WebConfigurationManager.AppSettings["AdminEmail"];
            string siteMasterEmail = WebConfigurationManager.AppSettings["SiteMasterEmail"];
            try
            {
                Servston.MailManager.SendMail("contact-us.html", keysValues, "New Contact Request: " + subject,
                    toAddress, adminEmail, email, new List<string>() { siteMasterEmail });

                return true;
            }
            catch (Exception ex)
            {
                // todo:log the exception
                return false;

            }
        }


        [HttpGet]
        [ActionName("Checkout")]
        public string Checkout(decimal amount, int transactionId, int attempt, string userIp)
        {

            try
            {
                PaymentManager paymentManager = new PaymentManager();
                var _return = paymentManager.Checkout(amount, transactionId, attempt, userIp);

                return _return["id"];//.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries)[0];
            }
            catch (Exception ex)
            {
                // todo:log the exception
                return "-";

            }
        }
        [HttpGet]
        [ActionName("JoinRequest")]
        public string JoinRequest(string companyName, string providerEmail, string contactName, string providerPhoneNumber, int cityId, int mainCategoryId, string servicesJsonData)
        {
            string result = "";
            try
            {
                JoinRequestService joinRequestService = new JoinRequestService();
                int addResult = joinRequestService.AddRequest(cityId, companyName, contactName, providerEmail, mainCategoryId,
                    providerPhoneNumber, servicesJsonData);

                if (addResult == -1)
                    result = ("هذا الإيميل قد تم إنشاء طلب له من قبل ...فضلا راجع إدارة البوابة");
                else if (addResult == 0)
                    result = ("حدث خطأ أثناء إرسال طلبك ...فضلا حاول مرة أخري لاحقا");
                else
                {
                    //send the notification email
                    Dictionary<string, string> keysValues = new Dictionary<string, string>
                    {
                        { "companyName", companyName},
                        { "email", providerEmail},
                        { "contactName", contactName},
                        { "phone", providerPhoneNumber},
                        { "id", addResult.ToString()}
                    };
                    string toAddress = WebConfigurationManager.AppSettings["ContactUsEmail"];
                    string adminEmail = WebConfigurationManager.AppSettings["AdminEmail"];
                    string siteMasterEmail = WebConfigurationManager.AppSettings["SiteMasterEmail"];
                    Servston.MailManager.SendMail("join-request.html", keysValues, "New Join Request: " + companyName,
                             toAddress, adminEmail, providerEmail, new List<string>() { siteMasterEmail });


                }

            }
            catch (Exception ex)
            {
                // todo:log the exception
                result = "حدث خطأ أثناء إرسال طلبك ...فضلا حاول مرة أخري لاحقا";

            }
            return result;
        }

        [HttpGet]
        [ActionName("UpdateProviderRequest")]
        public bool UpdateProviderRequest(int userId, int id, int status, string reason, decimal price, int duration = 0)
        {
            try
            {
                ServiceRequests _serviceRequests = new ServiceRequests();
                _serviceRequests.UpdateProviderRequest(id, userId, status, reason, price, duration);
                return true;
            }
            catch (Exception ex)
            {
                // todo:log the exception
                return false;

            }

        }

        [HttpGet]
        [ActionName("IncreaseProviderRequest")]
        public bool IncreaseProviderRequest(int id, int duration)
        {
            try
            {
                ServiceRequests _serviceRequests = new ServiceRequests();
                _serviceRequests.IncreaceRequestDuration(id, duration);
                // send notification to the first provider

                var request = _serviceRequests.GetRequest(id);
                var client = _serviceRequests.GetRequest(id).Client;
                Dictionary<string, string> keysValues = new Dictionary<string, string>
                {
                    {"name", client.FullName},
                    {"no", id.ToString()},
                    {"ServiceName", request.Service.Name}
                };

                string replyToAddress = WebConfigurationManager.AppSettings["ContactUsEmail"];
                string adminEmail = WebConfigurationManager.AppSettings["AdminEmail"];
                string siteMasterEmail = WebConfigurationManager.AppSettings["SiteMasterEmail"];
                try
                {
                    Servston.MailManager.SendMail("client/request-time.html", keysValues,
                        "تم الاستجابة على طلبكم ببوابة خدماتكم",
                        client.Email, adminEmail, replyToAddress, new List<string>() { siteMasterEmail });

                }
                catch (Exception ex)
                {
                }

                return true;
            }
            catch (Exception ex)
            {
                // todo:log the exception
                return false;

            }
        }

        [HttpGet]
        [ActionName("CloseProviderRequest")]
        public bool CloseProviderRequest(int id)
        {
            try
            {
                ServiceRequests _serviceRequests = new ServiceRequests();
                _serviceRequests.CloseProviderRequest(id);
                // send notification to the first provider

                var request = _serviceRequests.GetRequest(id);
                var client = _serviceRequests.GetRequest(id).Client;
                Dictionary<string, string> keysValues = new Dictionary<string, string>
                {
                    {"name", client.FullName},
                    {"no", id.ToString()},
                    {"duration", request.CurrentDuration.ToString()},
                    {"ServiceName", request.Service.Name}
                };

                string replyToAddress = WebConfigurationManager.AppSettings["ContactUsEmail"];
                string adminEmail = WebConfigurationManager.AppSettings["AdminEmail"];
                string siteMasterEmail = WebConfigurationManager.AppSettings["SiteMasterEmail"];
                try
                {
                    Servston.MailManager.SendMail("client/request-finished.html", keysValues,
                        "تم الاستجابة على طلبكم ببوابة خدماتكم",
                        client.Email, adminEmail, replyToAddress, new List<string>() { siteMasterEmail });

                }
                catch (Exception ex)
                {
                }

                return true;
            }
            catch (Exception ex)
            {
                // todo:log the exception
                return false;

            }
        }

        [HttpGet]
        [ActionName("ConfirmRequest")]
        public bool ConfirmRequest(int id, bool dummy, int x)
        {
            try
            {
                AdminServices adminServices = new AdminServices();
                adminServices.ConfirmRequest(id);
                return true;
            }
            catch (Exception ex)
            {
                // todo:log the exception
                return false;

            }
        }

    }
}

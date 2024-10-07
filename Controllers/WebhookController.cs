using DataCloudDemo.Common;
using DataCloudDemo.Models;
using DataCloudDemo.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Globalization;

namespace DataCloudDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly DCContext _context;
        public WebhookController(DCContext context)
        {
            _context = context;
        }

        [Route("Logdata/{id?}")]
        [HttpPost]
        public async Task<JsonResult> Logdata(string? id)
        {
            Console.WriteLine("{0}", id);
            if (Request.Headers.ContainsKey("x-signature"))
            {
                Console.WriteLine("{0}", Request.Headers["x-signature"]);
                try
                {
                    string rawRequestBody = string.Empty;
                    using (var reader = new StreamReader(Request.Body,
                                  encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false))
                    {
                        rawRequestBody = await reader.ReadToEndAsync();
                    }
                    if (Utilities.IsVerifySignature(Request.Headers["x-signature"], rawRequestBody))
                    {
                        Console.WriteLine("raw body:{0}", rawRequestBody);
                        WebhookHerokuClickEventViewModel webhookHerokuClickEvent = Newtonsoft.Json.JsonConvert.DeserializeObject<WebhookHerokuClickEventViewModel>(rawRequestBody);
                        if (webhookHerokuClickEvent != null)
                        {
                            Console.WriteLine("events:{0}", Newtonsoft.Json.JsonConvert.SerializeObject(webhookHerokuClickEvent.events));
                            if (webhookHerokuClickEvent.events != null)
                            {
                                foreach (var item in webhookHerokuClickEvent.events)
                                {
                                    if (!string.IsNullOrEmpty(item.PayloadCurrentValue))
                                    {
                                        var payloadCurrentValue = item.PayloadCurrentValue.Replace(Utilities.DC_DMO_NAME + '_', "").Replace("__c", "");
                                        Console.WriteLine("payload:{0}", payloadCurrentValue);
                                        var jsonObj = JsonSerializer.Deserialize<IDictionary<string, object>>(payloadCurrentValue);
										if (jsonObj != null && jsonObj.Keys.Count > 0)
										{
											WebhookHistory webhook = new WebhookHistory();
											webhook.Contactid = jsonObj[$"{Utilities.DC_CONNECTOR_NAME}_contactId"].ToString();
											webhook.Eventtype = jsonObj[$"{Utilities.DC_CONNECTOR_NAME}_interactionName"].ToString();
											webhook.EventId = jsonObj["eventId"].ToString();
											webhook.Platformeventinvokedon = DateTime.ParseExact(jsonObj["dateTime"].ToString(), "yyyy-MM-dd HH:mm:ss", new CultureInfo("en-US"), DateTimeStyles.None);
											webhook.Dataactioninitiatedon = webhookHerokuClickEvent.creationDateTime;

											await _context.WebhookHistories.AddAsync(webhook);
											int result = await _context.SaveChangesAsync();
											Console.WriteLine("Webhook insert: {0}", result);
										}
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: {0}", ex.ToString());
                }
            }
            return new JsonResult(new { Status = (int)HttpStatusCode.OK });
        }
    }
}

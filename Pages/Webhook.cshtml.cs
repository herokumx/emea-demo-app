using DataCloudDemo.Models;
using DataCloudDemo.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace DataCloudDemo.Pages
{
    public class WebhookModel : PageModel
    {
        private readonly DCContext _context;
        public WebhookModel(DCContext context)
        {
            _context = context;
        }

        public IList<WebhookHistory> lRows { get; set; }
        public void OnGet()
        {
            ViewData["CurrentPage"] = "monitor";
            lRows = _context.WebhookHistories.Select(i => new WebhookHistory()
            {
                Id = i.Id,
                Contactid = i.Contactid,
                Firstname = i.Firstname,
                Lastname = i.Lastname,
                Eventtype = i.Eventtype,
                Createdate = i.Createdate,
                Dataactioninitiatedon = i.Dataactioninitiatedon,
                Platformeventinvokedon = i.Platformeventinvokedon,
                Functioninvokedon = i.Functioninvokedon,
                Contactupdatedon = i.Contactupdatedon,
                EventId = i.EventId
            }).ToList().OrderByDescending(p => p.Createdate).ToList();
        }

        public async override Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context,
                                          PageHandlerExecutionDelegate next)
        {
            try
            {
                var appConfig = await _context.AppConfigs.FirstOrDefaultAsync();
                if (appConfig == null || (appConfig != null && string.IsNullOrEmpty(appConfig.ContactId)))
                {
                    context.Result = RedirectToPage("/appsettings");
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex);
            }

            await next.Invoke();
        }
    }
}

using DataCloudDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace DataCloudDemo.Pages
{
    public class EventMonitorModel : PageModel
    {
        private readonly DCContext _context;
        public EventMonitorModel(DCContext context)
        {
            _context = context;
        }

        public IList<EventHistory> lRows { get; set; }

        public void OnGet()
        {
            ViewData["CurrentPage"] = "monitor";
            lRows = _context.EventHistories.Select(i => new EventHistory()
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

        public ActionResult OnPostGetAjax(EventHistory levent)
        {
            try
            {
                if (levent != null && !string.IsNullOrEmpty(levent.Contactid))
                {
                    _context.EventHistories.Add(levent);
                    int result = _context.SaveChanges();
                    if (result > 0)
                    {
                        Response.StatusCode = (int)HttpStatusCode.OK;
                    }
                    else
                    {
                        Response.StatusCode = (int)HttpStatusCode.NotFound;
                    }
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: {0}", ex.Message);
                return new JsonResult(new { Message = ex.Message, Status = Response.StatusCode });
            }

            return new JsonResult(new { Status = Response.StatusCode });
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

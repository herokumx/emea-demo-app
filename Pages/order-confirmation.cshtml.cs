using DataCloudDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DataCloudDemo.Pages
{
    public class order_confirmationModel : PageModel
    {
        private readonly DCContext _context;
        public order_confirmationModel(DCContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            ViewData["CurrentPage"] = "order";
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
                else
                {
                    ViewData["ContactId"] = appConfig.ContactId;
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

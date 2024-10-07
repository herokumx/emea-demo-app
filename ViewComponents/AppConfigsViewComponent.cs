using DataCloudDemo.Models;
using DataCloudDemo.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DataCloudDemo.ViewComponents
{
    public class AppConfigsViewComponent : ViewComponent
    {
        private readonly DCContext _context;
        public AppConfigsViewComponent(DCContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            AppSettingsViewModel settings = new AppSettingsViewModel();
            try
            {
                if (HttpContext.Request.Path.HasValue)
                {
                    if (HttpContext.Request.Path.ToString().ToLower() == "/appsettings")
                    {
                        return View(settings);
                    }
                }

                var appsettings = await _context.AppConfigs.FirstOrDefaultAsync();
                if (appsettings != null)
                {
                    settings.Locale = appsettings.UserLocale;
                    settings.Timezone = appsettings.UserTimezone;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error :{0}", ex);
            }
            return View(settings);
        }
    }
}

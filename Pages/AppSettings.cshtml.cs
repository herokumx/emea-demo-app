using DataCloudDemo.Models;
using DataCloudDemo.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DataCloudDemo.Pages
{
    public class AppSettingsModel : PageModel
    {
        private readonly DCContext _context;
        public AppSettingsModel(DCContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGet()
        {
            ViewData["CurrentPage"] = "appsettings";
            ErrorMessage = "";

            try
            {
                var appConfig = await _context.AppConfigs.FirstOrDefaultAsync();
                if (appConfig != null)
                {
                    settings = new AppSettingsViewModel()
                    {
                        ContactId = appConfig.ContactId,
                        Locale = appConfig.UserLocale,
                        Timezone = appConfig.UserTimezone
                    };
                }
                else
                {
                    settings = new AppSettingsViewModel();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            return Page();
        }


        [TempData]
        public string ErrorMessage { get; set; }

        [BindProperty]
        public AppSettingsViewModel? settings { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var appConfig = _context.AppConfigs.FirstOrDefault();
                if (appConfig == null)
                {
                    appConfig = new AppConfig()
                    {
                        ContactId = settings!.ContactId,
                        UserLocale = settings.Locale,
                        UserTimezone = settings.Timezone
                    };
                    _context.AppConfigs.Add(appConfig);
                    _context.SaveChanges();
                    ErrorMessage = "The app settings has been saved successfully.";
                }
                else
                {
                    appConfig.ContactId = settings!.ContactId;
                    appConfig.UserLocale = settings.Locale;
                    appConfig.UserTimezone = settings.Timezone;
                    _context.Entry(appConfig).State = EntityState.Modified;
                    _context.SaveChanges();
                    ErrorMessage = "The app settings has been updated successfully.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            return await Task.FromResult(Page());
        }
    }
}

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BlazorAppEvergreenOIDC.Data;

public class LogoutModel : PageModel
{
    private readonly IConfiguration configuration;
    private readonly FileLogger fileLogger;

    public LogoutModel(IConfiguration configuration, FileLogger fileLogger)
    {
        this.configuration = configuration;
        this.fileLogger = fileLogger;
    }
    public async Task<IActionResult> OnGet()
    {
        try
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
            return Redirect(this.configuration.GetValue<string>("OpenIDConnect:LogoutEndpoint")!);
        }
        catch (Exception e)
        {
            e.GetType();
            fileLogger.SaveLogToFile(e.ToString());
            throw;
        }
    }
}

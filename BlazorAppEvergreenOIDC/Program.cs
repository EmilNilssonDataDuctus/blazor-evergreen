using BlazorAppEvergreenOIDC;
using BlazorAppEvergreenOIDC.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Options;
using MudBlazor.Services;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<ScimUserService>();
builder.Services.AddSingleton<FileLogger>();
builder.Services.AddSingleton<TokenClient>();
builder.Services.AddScoped<TokenProvider>();

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddAuthentication(options =>
{

    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
    //options.Cookie.SameSite = SameSiteMode.Strict;
})
.AddOpenIdConnect(options =>
{

    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

    options.Authority = Configuration.GetValue<string>("OpenIdConnect:Issuer");
    options.ClientId = Configuration.GetValue<string>("OpenIdConnect:ClientId");
    options.ClientSecret = Configuration.GetValue<string>("OpenIdConnect:ClientSecret");
    options.ResponseType = OpenIdConnectResponseType.Code;
    options.ResponseMode = OpenIdConnectResponseMode.Query;
    options.GetClaimsFromUserInfoEndpoint = true;

    // Program wont let me access Authority Curity on http unless I add this
    options.RequireHttpsMetadata = false;

    string scopeString = Configuration.GetValue<string>("OpenIDConnect:Scope");
    scopeString.Split(" ", StringSplitOptions.TrimEntries).ToList().ForEach(scope =>
    {
        options.Scope.Add(scope);
    });

    options.Authority = Configuration["OpenIDConnect:Issuer"];

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = options.Authority,
        ValidAudience = options.ClientId,
        ValidateIssuer = Convert.ToBoolean(Configuration["OpenIDConnect:ValidateIssuer"])
    };

    options.Events.OnRedirectToIdentityProviderForSignOut = (context) =>
    {
        context.ProtocolMessage.PostLogoutRedirectUri = Configuration.GetValue<string>("OpenIdConnect:PostLogoutRedirectUri");
        return Task.CompletedTask;
    };

    options.SaveTokens = true;
});

builder.Services.AddAuthorization();
builder.Services.AddRazorPages();

builder.Services.AddLocalization();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();
var supportedCultures = new[] { "en-US", "sv-SE" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);
app.UseRequestLocalization(localizationOptions);

// https://stackoverflow.com/questions/53980129/oidc-login-fails-with-correlation-failed-cookie-not-found-while-cookie-is
app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.None,
    Secure = CookieSecurePolicy.Always
});

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
// Alt from Curity docs https://curity.io/resources/learn/dotnet-openid-connect-website/#integrating-net-security
//app.UseEndpoints(endpoints => {
//    endpoints.MapRazorPages();
//});

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

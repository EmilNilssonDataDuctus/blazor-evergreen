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
builder.Services.AddScoped<JwtHandler>();

// The following code is from Curitys demo app
// DONT use it as it removes all microsoft schema issued claims
//JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.Cookie.SameSite = SameSiteMode.Strict;
})
.AddOpenIdConnect(options =>
{
    options.NonceCookie.SameSite = SameSiteMode.Strict;
    options.CorrelationCookie.SameSite = SameSiteMode.Strict;

    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

    options.Authority = Configuration.GetValue<string>("OpenIdConnect:Issuer");
    options.ClientId = Configuration.GetValue<string>("OpenIdConnect:ClientId");
    options.ClientSecret = Configuration.GetValue<string>("OpenIdConnect:ClientSecret");
    options.ResponseType = OpenIdConnectResponseType.Code;
    options.ResponseMode = OpenIdConnectResponseMode.Query;
    options.GetClaimsFromUserInfoEndpoint = false;

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
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
    options.Secure = CookieSecurePolicy.Always;
});

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

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

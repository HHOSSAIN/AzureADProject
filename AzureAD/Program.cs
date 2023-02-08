using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using static System.Net.WebRequestMethods;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient(); //we need to add httpclient factory in dependency injection in program.cs along with adding it in home controller

// Add services to the container..application client id: 06091f65-ffc0-4dc1-b442-04be47b6ffe9
//auth endpoint: https://login.microsoftonline.com/21af2c97-fc02-46fd-84fb-323961b73470/oauth2/v2.0/authorize
builder.Services.AddControllersWithViews();

//take care of client side part of open id connect flow with middlewares
//builder.AddAuth
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme; //cookies
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme; //OpenIdConnect
}).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme) //cookie will be used for authentication for subsequent request. 
 .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options => //this handler will be responsible for authorization request and manipulating handler. when authentication
 {                                                                        //is required in our app, it will use openidconnect as default
     options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;                                                                         //assign sign in scheme to cookies
     options.Authority  = "https://login.microsoftonline.com/21af2c97-fc02-46fd-84fb-323961b73470/v2.0"; //to find the value, go to endponnts page in azure, copy the metadata doc link, go to link, copy and paster in json parser to find "issuer"
     options.ClientId = "06091f65-ffc0-4dc1-b442-04be47b6ffe9"; //taken from app registration in azure where it shows 
     //options.ResponseType = "id_token";
     options.ResponseType = "code";
     options.SaveTokens = true; //tokens from aad will be saved in auth cookie header

     options.Scope.Add("api://f6b5f57a-2924-435e-93d3-9ef705e7d308/AdminAccess"); // taken from api permissions from "azure ad" app registration. before adding this api
                                                                                //permission in this main app, we had to expose this api in "azure ad api" app

     //line below added after removing token id and adding secret key instead in AAD..we can find it certificate and secrets in azure
     options.ClientSecret = "d5i8Q~U~HmCWLGX~5aylc2dhbtdixzAqqlYQYbR9"; //needed for apps to authenticate itself.
 }); 
                                                               


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
//the middlewares
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//add authentiation in pipeline befoe authorisation
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

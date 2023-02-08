using AzureAD.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace AzureAD.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory; //we gett httpclient factory with dependency injection


        /*once user is authenticted in home controller, we will have access token. we can retrieve that access token in method.*/
        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory) //we gett httpclient factory with dependency injection
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult SignIn() 
        {
            var scheme = OpenIdConnectDefaults.AuthenticationScheme;
            var redirectUrl = Url.ActionContext.HttpContext.Request.Scheme + "://" + Url.ActionContext.HttpContext.Request.Host;
            return Challenge(new AuthenticationProperties()
            {
                RedirectUri = redirectUrl
            }, scheme);
        }

        public IActionResult SignOut()
        {
            var scheme = OpenIdConnectDefaults.AuthenticationScheme;
            return SignOut(new AuthenticationProperties(), CookieAuthenticationDefaults.AuthenticationScheme, scheme); 
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> APICall()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var client = _httpClientFactory.CreateClient(); //creating a client for httpclient factory
            var request = new HttpRequestMessage( //actual request for our api. we will need to input the exact uri. we can get it by running the api project 
                                                 //collecting the url from curl get
                    HttpMethod.Get,
                    "https://localhost:44366/WeatherForecast"

            );

            //before we send out the request, we will have to add authorization header
            request.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessToken); //1st arg=authentication type->bearer

            //after formation of request, the response has to be forwarded
            var response = await client.SendAsync(request);

            if(response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //issue
            }

            return Content(response.ToString());
        }
    }
}
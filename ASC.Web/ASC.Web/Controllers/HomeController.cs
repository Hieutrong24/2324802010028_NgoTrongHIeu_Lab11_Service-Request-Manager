using ASC.Utilities;
using ASC.Web.Configuration;
using ASC.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace ASC.Web.Controllers
{
    public class HomeController : AnonymousController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IOptions<ApplicationSettings> _settings;

        public HomeController(
            ILogger<HomeController> logger,
            IOptions<ApplicationSettings> settings)
        {
            _logger = logger;
            _settings = settings;
        }

        public IActionResult Index()
        {
            HttpContext.Session.SetSession("ApplicationTitle", _settings.Value.ApplicationTitle);

            ViewBag.Title = _settings.Value.ApplicationTitle;

            return View();
        }

        [HttpGet]
        public IActionResult GetStarted()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return LocalRedirect("/ServiceRequests/Dashboard/Dashboard");
            }

            return LocalRedirect("/Identity/Account/Login");
        }

        public IActionResult Dashboard()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return LocalRedirect("/ServiceRequests/Dashboard/Dashboard");
            }

            return LocalRedirect("/Identity/Account/Login");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? id = null)
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

            _logger.LogError("Error page displayed. StatusCode: {StatusCode}, RequestId: {RequestId}", id, requestId);

            Response.StatusCode = id ?? 500;

            return View(new ErrorViewModel
            {
                RequestId = requestId
            });
        }
    }
}
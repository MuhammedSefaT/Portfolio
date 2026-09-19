using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Portfolyo.Web.Models;

namespace Portfolyo.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger) => _logger = logger;

    public IActionResult Index() => View();

    // Adres çubuğunda Türkçe ve kısa görünsün diye ayrı yollar tanımlı.
    [HttpGet]
    [Route("hakkimda")]
    public IActionResult About() => View();

    [HttpGet]
    [Route("projelerim")]
    public IActionResult Projects() => View();

    [HttpGet]
    [Route("iletisim")]
    public IActionResult Contact() => View();

    [HttpGet]
    [Route("gizlilik")]
    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
        => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}

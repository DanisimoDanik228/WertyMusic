using Microsoft.AspNetCore.Mvc;

namespace WertyMusic.Controllers;

public class HomeController : Controller
{
    [HttpGet("")]
    [HttpGet("Index")]
    public IActionResult Index()
    {
        return View();
    }
}
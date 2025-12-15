using Microsoft.AspNetCore.Mvc;

namespace WertyMusic.Controllers;

public class HomeController : Controller
{
    private const string _indexView = "BeautifulMusic/BeautyIndex"; //"Index"
    [HttpGet("")]
    [HttpGet("Index")]
    public IActionResult Index()
    {
        return View(_indexView);
    }
}
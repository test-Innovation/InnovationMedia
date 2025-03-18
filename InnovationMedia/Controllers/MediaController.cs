using Microsoft.AspNetCore.Mvc;

namespace InnovationMedia.Controllers
{
    public class MediaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace FrontToBack_Pronia.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult ErrorPage(string error)
        {
            return View(model:error);
        }
    }
}

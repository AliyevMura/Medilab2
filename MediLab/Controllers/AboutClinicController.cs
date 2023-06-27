using Microsoft.AspNetCore.Mvc;

namespace MediLab.Controllers
{
    public class AboutClinicController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

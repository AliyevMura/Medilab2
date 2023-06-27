using Microsoft.AspNetCore.Mvc;

namespace MediLab.Controllers
{
	public class AboutDoctorController : Controller
	{
		[HttpGet("{Id}")]
		public IActionResult Index([FromRoute]int Id)
		{
			return View();
		}
	}
}

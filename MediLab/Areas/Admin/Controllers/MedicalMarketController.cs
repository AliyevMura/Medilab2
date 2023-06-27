using MediLab.Areas.Admin.ViewModels.MedicalMarkets;
using MediLab.DAL;
using MediLab.Models;
using MediLab.Utilities.Contains;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Data;

namespace MediLab.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "Admin,Moderator")]
    public class MedicalMarketController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        public MedicalMarketController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.MedicalMarkets.Where(p => !p.IsDeleted).OrderByDescending(p => p.Id).ToListAsync());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]


        public async Task<IActionResult> Create(CreateMarketVM postVM)
        {
            if (!ModelState.IsValid) return View(postVM);



            if (postVM.Photo.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("Photo", ErrorMessages.FileMustBeImageType);
            }



            if (postVM.Photo.Length / 1024 < 200)
            {
                ModelState.AddModelError("Photo", ErrorMessages.FileSizeMustLessThan200KB);
            }



            string rootPath = Path.Combine(_environment.WebRootPath, "cdn.doctortap.az", "uploads","product","small");
            string filename = Guid.NewGuid().ToString() + postVM.Photo.FileName;


            using (FileStream fileStream = new FileStream(Path.Combine(rootPath, filename), FileMode.Create))
            {
                await postVM.Photo.CopyToAsync(fileStream);
            }




            MedicalMarket post = new MedicalMarket
            {
                Name=postVM.Name,
                Price=postVM.Price,
                ImagePath = filename
            };



            await _context.MedicalMarkets.AddAsync(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }




        public async Task<IActionResult> Update(int id)
        {
            MedicalMarket team = await _context.MedicalMarkets.FindAsync(id);
            if (team == null)
            {
                return NotFound();
            }
            UpdateMarketVM updateTeamVM = new UpdateMarketVM()
            {
                Name = team.Name,
                Price = team.Price,
                Id = id
            };
            return View(updateTeamVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateMarketVM teamVM)
        {
            if (!ModelState.IsValid)
            {
                return View(teamVM);
            }
            MedicalMarket team1 = await _context.MedicalMarkets.FirstOrDefaultAsync(s => s.Id == teamVM.Id);

            if (!teamVM.Photo.ContentType.Contains("image/"))
            {


                ModelState.AddModelError("Photo", ErrorMessages.FileMustBeImageType);
            }


            if (teamVM.Photo.Length / 1024 < 200)
            {
                ModelState.AddModelError("Photo", ErrorMessages.FileSizeThanLess200KB);
            }

            string rootPath = Path.Combine(_environment.WebRootPath, "cdn.doctortap.az", "uploads", "product", "small");
            string filename = (await _context.MedicalMarkets.FindAsync(teamVM.Id))?.ImagePath;




            string filePath = Path.Combine(rootPath, filename);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }


            string newFileName = Guid.NewGuid().ToString() + teamVM.Photo.FileName;
            string resultPath = Path.Combine(rootPath, newFileName);

            using (FileStream file = new FileStream(resultPath, FileMode.Create))
            {
                await teamVM.Photo.CopyToAsync(file);
            }

            team1.Name = teamVM.Name;

            team1.Price = teamVM.Price;
            team1.ImagePath = newFileName;


            string fileName = Guid.NewGuid().ToString() + teamVM.Photo.FileName;


            using (FileStream fileStream = new FileStream(Path.Combine(rootPath, fileName), FileMode.Create))
            {
                await teamVM.Photo.CopyToAsync(fileStream);
            }
            MedicalMarket team = new MedicalMarket
            {
                Name = teamVM.Name,
                Price = teamVM.Price,
                ImagePath = fileName
            };

            await _context.MedicalMarkets.AddAsync(team);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }


        public async Task<IActionResult> Delete(int id)
        {

            MedicalMarket post = await _context.MedicalMarkets.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            string filepath = Path.Combine(_environment.WebRootPath, "assets", "images");

            if (System.IO.File.Exists(filepath))
            {
                System.IO.File.Delete(filepath);
            }


            _context.MedicalMarkets.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

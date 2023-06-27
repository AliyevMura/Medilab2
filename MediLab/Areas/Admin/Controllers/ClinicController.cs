using MediLab.Areas.Admin.ViewModels.Clinics;
using MediLab.Areas.Admin.ViewModels.MedicalMarkets;
using MediLab.DAL;
using MediLab.Models;
using MediLab.Utilities.Contains;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MediLab.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles ="Admin")]
    public class ClinicController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        public ClinicController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Clinics.Where(p => !p.IsDeleted).OrderByDescending(p => p.Id).ToListAsync());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]


        public async Task<IActionResult> Create(CreateClinicVM postVM)
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



            string rootPath = Path.Combine(_environment.WebRootPath, "cdn.doctortap.az", "uploads", "clinic");
            string filename = Guid.NewGuid().ToString() + postVM.Photo.FileName;


            using (FileStream fileStream = new FileStream(Path.Combine(rootPath, filename), FileMode.Create))
            {
                await postVM.Photo.CopyToAsync(fileStream);
            }




            Clinic clinic = new Clinic
            {
                Name = postVM.Name,
                Description = postVM.Description,
                Adress= postVM.Adress,
                ImagePath = filename
            };



            await _context.Clinics.AddAsync(clinic);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int id)
        {
            Clinic clinic = await _context.Clinics.FindAsync(id);
            if (clinic == null)
            {
                return NotFound();
            }
            UpdateClinicVM updateClinicVM = new UpdateClinicVM()
            {
                Name = clinic.Name,
                Description = clinic.Description,
                Adress = clinic.Adress,
                Id = id
            };
            return View(updateClinicVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateClinicVM teamVM)
        {
            if (!ModelState.IsValid)
            {
                return View(teamVM);
            }
            Clinic team1 = await _context.Clinics.FirstOrDefaultAsync(s => s.Id == teamVM.Id);

            if (!teamVM.Photo.ContentType.Contains("image/"))
            {


                ModelState.AddModelError("Photo", ErrorMessages.FileMustBeImageType);
            }


            if (teamVM.Photo.Length / 1024 < 200)
            {
                ModelState.AddModelError("Photo", ErrorMessages.FileSizeThanLess200KB);
            }

            string rootPath = Path.Combine(_environment.WebRootPath, "cdn.doctortap.az", "uploads", "clinic");
            string filename = (await _context.Clinics.FindAsync(teamVM.Id))?.ImagePath;




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

            team1.Description = teamVM.Description;
            team1.Adress = teamVM.Adress;
            team1.ImagePath = newFileName;


            string fileName = Guid.NewGuid().ToString() + teamVM.Photo.FileName;


            using (FileStream fileStream = new FileStream(Path.Combine(rootPath, fileName), FileMode.Create))
            {
                await teamVM.Photo.CopyToAsync(fileStream);
            }
            Clinic clinic = new Clinic
            {
                Name = teamVM.Name,
                Description = teamVM.Description,
                Adress = teamVM.Adress,
                
                ImagePath = fileName
            };

            await _context.Clinics.AddAsync(clinic);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Delete(int id)
        {

            Clinic clinic = await _context.Clinics.FindAsync(id);
            if (clinic == null)
            {
                return NotFound();
            }

            string filepath = Path.Combine(_environment.WebRootPath, "cdn.doctortap.az", "uploads","clinic");

            if (System.IO.File.Exists(filepath))
            {
                System.IO.File.Delete(filepath);
            }


            _context.Clinics.Remove(clinic);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }


}


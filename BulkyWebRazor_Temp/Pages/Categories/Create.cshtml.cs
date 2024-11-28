using BulkyWebRazor_Temp.Data;
using BulkyWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor_Temp.Pages.Categories
{
    //This is use to bind all properties in this method
    [BindProperties]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        //In Razor pages we need to add below syntax without that we will get all data as null
        //[BindProperty]
        public Category Categories { get; set; }
        public CreateModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public void OnGet()
        {
        }
        public IActionResult OnPost() 
        {
            _db.Categories.Add(Categories);
            _db.SaveChanges();
            TempData["Success"] = "Category added successfully";
            return RedirectToPage("Index");

        }

    }
}

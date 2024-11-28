using BulkyWebRazor_Temp.Data;
using BulkyWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor_Temp.Pages.Categories
{
    [BindProperties]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public Category Categories { get; set; }
        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public void OnGet(int? id)
        {
            if (id !=null && id !=0) 
            {
                Categories = _db.Categories.Find(id);
            }
            
        }
        public IActionResult OnPost()
        {
           if (ModelState.IsValid)
            {
                _db.Categories.Update(Categories);
                _db.SaveChanges();
                TempData["SuccessUp"] = "Category updated successfully";
                return RedirectToPage("Index");
            }
            return Page(); 

        }
    }
}

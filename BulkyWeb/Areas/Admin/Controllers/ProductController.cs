using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        //For to show Product
        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
            return View(objProductList);
        }
        //For to show create view
        //public IActionResult Create()
        //{
        //    //To get categories by using Projection in EF core
        //    //IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll().
        //    //    Select(u => new SelectListItem
        //    //    {
        //    //        Text = u.Name,
        //    //        Value = u.Id.ToString()

        //    //    });
        //    //ViewBag
        //    //ViewBag.CategoryList = CategoryList;

        //    //ViewData
        //    //ViewData["CategoryList"] = CategoryList;

        //    ProductVM productVM = new()
        //    {
        //        CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
        //        {
        //            Text = u.Name,
        //            Value = u.Id.ToString()
        //        })
        //    };

        //    return View(productVM);
        //}

        //For Upsert
        public IActionResult Upsert(int? Id)
        {
            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };
            if (Id == null || Id == 0)
            {
                //create 
                return View(productVM);
            }
            else 
            {
                //Update
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == Id);
                return View(productVM);
            }        
        }

        ////For to create Product
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Create(ProductVM obj,IFormFile? file)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _unitOfWork.Product.Add(obj.Product);
        //        _unitOfWork.Save();
        //        TempData["Success"] = "Product added successfully";
        //        return RedirectToAction("Index");
        //    }
        //    else 
        //    {
        //        obj.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
        //        {
        //            Text = u.Name,
        //            Value = u.Id.ToString()
        //        });

        //        return View(obj); // return to the form with validation errors
        //    }

        //}

        //For to show Edit view
        //public IActionResult Edit(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }

        //    //Three way to do it.
        //    Product? ProductfromDb = _unitOfWork.Product.Get(u => u.Id == id);
        //    //Product? ProductfromDb1 = _db.Categories.Find(id);
        //    //Product? ProductfromDb2 = _db.Categories.Where(u => u.Id == id).FirstOrDefault();

        //    if (ProductfromDb == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(ProductfromDb);
        //}

        //For to Edit Product
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Edit(Product obj)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _unitOfWork.Product.Update(obj);
        //        _unitOfWork.Save();
        //        TempData["SuccessUp"] = "Product updated successfully";
        //        return RedirectToAction("Index");
        //    }
        //    return View(obj); // return to the form with validation errors
        //}
        //For to show Delete view

        //For to Upsert Product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM obj, IFormFile? file)
        { 
            if (ModelState.IsValid)
            {
                //To add image
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file !=null)
                { 
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"Images\Product");

                    if (!string.IsNullOrEmpty(obj.Product.ImageUrl))
                    {
                        //Delete the old image
                        var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.TrimStart('\\'));
                        if(System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    { 
                        file.CopyTo(fileStream);
                    }

                    obj.Product.ImageUrl = @"\Images\Product\" + fileName;
                         
                }

                if (obj.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(obj.Product);
                    TempData["Success"] = "Product added successfully";
                }
                else
                {
                    _unitOfWork.Product.Update(obj.Product);
                    TempData["Success"] = "Product updated successfully";
                }
                _unitOfWork.Save();
                
                return RedirectToAction("Index");
            }
            else
            {
                obj.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });

                return View(obj); // return to the form with validation errors
            }

        }
        //public IActionResult Delete(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    Product? ProductfromDb = _unitOfWork.Product.Get(u => u.Id == id);
        //    if (ProductfromDb == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(ProductfromDb);
        //}

        //For to Delete Product
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public IActionResult DeletePost(int? id)
        //{
        //    Product? obj = _unitOfWork.Product.Get(u => u.Id == id);
        //    if (obj == null)
        //    {
        //        return NotFound();
        //    }
        //    _unitOfWork.Product.Remove(obj);
        //    _unitOfWork.Save();
        //    TempData["SuccessDel"] = "Product deleted successfully";
        //    return RedirectToAction("Index");

        //}

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new {data = objProductList });
        }
        [HttpDelete]
        public IActionResult Delete(int? Id)
        {
            var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == Id);
            if (productToBeDeleted == null) 
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath,
                productToBeDeleted.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldImagePath))
            { 
                System.IO.File.Delete(oldImagePath);
            }

            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();
            return Json(new { success = true, message ="Product delete Successful" });

        }

        #endregion
    }
}

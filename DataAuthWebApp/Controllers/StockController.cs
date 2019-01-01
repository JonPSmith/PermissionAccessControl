using System.Linq;
using DataLayer.EfCode;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DataAuthWebApp.Controllers
{
    public class StockController : Controller
    {
        private readonly MultiTenantDbContext _context;

        public StockController(MultiTenantDbContext context)
        {_context = context;
        }


        // GET: Stock
        public ActionResult Index()
        {
            var stock = _context.CurrentStock.Include(x => x.AtShop).ToList();
            return View(stock);
        }

        //// GET: Stock/Details/5
        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        //// GET: Stock/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Stock/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(IFormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add insert logic here

        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: Stock/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //// POST: Stock/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add update logic here

        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: Stock/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: Stock/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add delete logic here

        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}
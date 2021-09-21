using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalRCore.Models;

namespace SignalRCore.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly IHubContext<SignalrServer> _signalrHub;
        public ProductController(ApplicationDBContext context, IHubContext<SignalrServer> signalrHub)
        {
            _context = context;
            _signalrHub = signalrHub;
        }

        // GET: Product
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }
        [HttpGet]
        public IActionResult GetProducts()
        {
            var res = _context.Products.ToList();
            return Ok(res);
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ProdId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Product/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProdId,ProdName,Category,UnitPrice,StockQty")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                await _signalrHub.Clients.All.SendAsync("LoadProduct");
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int ProdId, [Bind("ProdId,ProdName,Category,UnitPrice,StockQty")] Product product)
        {
            if (ProdId != product.ProdId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    await _signalrHub.Clients.All.SendAsync("LoadProduct");

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProdId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ProdId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int ProdId)
        {
            var product = await _context.Products.FindAsync(ProdId);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            await _signalrHub.Clients.All.SendAsync("LoadProduct");

            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProdId == id);
        }
    }
}

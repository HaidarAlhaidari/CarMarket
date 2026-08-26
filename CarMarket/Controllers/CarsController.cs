using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarMarket.Models;
using CarMarket.Data;

public class CarsController : Controller
{
    private readonly ApplicationDbContext _context;

    public CarsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: CARS
    public async Task<IActionResult> Index(
        string searchString,
        string fuelType)
    {
        var cars = from car in _context.Cars
                   select car;

        // Search by Brand or Model
        if (!string.IsNullOrEmpty(searchString))
        {
            cars = cars.Where(car =>
                car.Brand.Contains(searchString) ||
                car.Model.Contains(searchString));
        }

        // Filter by Fuel Type
        if (!string.IsNullOrEmpty(fuelType))
        {
            cars = cars.Where(car =>
                car.FuelType == fuelType);
        }

        return View(await cars.ToListAsync());
    }

    // GET: CARS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var car = await _context.Cars
            .FirstOrDefaultAsync(m => m.Id == id);
        if (car == null)
        {
            return NotFound();
        }

        return View(car);
    }

    // GET: CARS/Create
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    // POST: CARS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(
    [Bind("Id,Brand,Model,Year,Price,Mileage,FuelType,Description,ImageUrl,ImageUrl2,ImageUrl3,ImageUrl4")] Car car)
    {
        if (ModelState.IsValid)
        {
            _context.Add(car);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        return View(car);
    }

    // GET: CARS/Edit/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var car = await _context.Cars.FindAsync(id);
        if (car == null)
        {
            return NotFound();
        }
        return View(car);
    }

    // POST: CARS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(
     int id,
     [Bind("Id,Brand,Model,Year,Price,Mileage,FuelType,Description,ImageUrl,ImageUrl2,ImageUrl3,ImageUrl4")] Car car)
    {
        if (id != car.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(car);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CarExists(car.Id))
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

        return View(car);
    }

    // GET: CARS/Delete/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var car = await _context.Cars
            .FirstOrDefaultAsync(m => m.Id == id);
        if (car == null)
        {
            return NotFound();
        }

        return View(car);
    }

    // POST: CARS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]

    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var car = await _context.Cars.FindAsync(id);
        if (car != null)
        {
            _context.Cars.Remove(car);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool CarExists(int? id)
    {
        return _context.Cars.Any(e => e.Id == id);
    }
}

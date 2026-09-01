using CarMarket.Data;
using CarMarket.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class CarsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public CarsController(
        ApplicationDbContext context,
        UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // Alla får visa och söka bland bilar
    [AllowAnonymous]
    public async Task<IActionResult> Index(
        string? searchString,
        string? fuelType)
    {
        var cars = _context.Cars.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            cars = cars.Where(car =>
                car.Brand.Contains(searchString) ||
                car.Model.Contains(searchString));
        }

        if (!string.IsNullOrWhiteSpace(fuelType))
        {
            cars = cars.Where(car =>
                car.FuelType == fuelType);
        }

        return View(await cars.ToListAsync());
    }

    // Alla får visa detaljer
    [AllowAnonymous]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var car = await _context.Cars
            .FirstOrDefaultAsync(car => car.Id == id);

        if (car == null)
        {
            return NotFound();
        }

        return View(car);
    }

    // Bara inloggade användare får öppna Create
    [Authorize]
    public IActionResult Create()
    {
        return View();
    }

    // Bara inloggade användare får skapa en annons
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind(
            "Brand,Model,Year,Price,Mileage,FuelType,Description," +
            "ImageUrl,ImageUrl2,ImageUrl3,ImageUrl4")]
        Car car)
    {
        if (ModelState.IsValid)
        {
            // Användaren som är inloggad blir ägare
            car.SellerId = _userManager.GetUserId(User);

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        return View(car);
    }

    // Säljaren eller Admin får öppna Edit
    [Authorize]
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

        if (!CanManageCar(car))
        {
            return Forbid();
        }

        return View(car);
    }

    // Säljaren eller Admin får spara ändringar
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        [Bind(
            "Id,Brand,Model,Year,Price,Mileage,FuelType,Description," +
            "ImageUrl,ImageUrl2,ImageUrl3,ImageUrl4")]
        Car input)
    {
        if (id != input.Id)
        {
            return NotFound();
        }

        // Hämta originalet från databasen
        var existingCar = await _context.Cars.FindAsync(id);

        if (existingCar == null)
        {
            return NotFound();
        }

        // Kontrollera ägare innan något ändras
        if (!CanManageCar(existingCar))
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            return View(input);
        }

        // Uppdatera endast tillåtna egenskaper.
        // SellerId ändras aldrig här.
        existingCar.Brand = input.Brand;
        existingCar.Model = input.Model;
        existingCar.Year = input.Year;
        existingCar.Price = input.Price;
        existingCar.Mileage = input.Mileage;
        existingCar.FuelType = input.FuelType;
        existingCar.Description = input.Description;
        existingCar.ImageUrl = input.ImageUrl;
        existingCar.ImageUrl2 = input.ImageUrl2;
        existingCar.ImageUrl3 = input.ImageUrl3;
        existingCar.ImageUrl4 = input.ImageUrl4;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CarExists(id))
            {
                return NotFound();
            }

            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    // Säljaren eller Admin får öppna Delete
    [Authorize]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var car = await _context.Cars
            .FirstOrDefaultAsync(car => car.Id == id);

        if (car == null)
        {
            return NotFound();
        }

        if (!CanManageCar(car))
        {
            return Forbid();
        }

        return View(car);
    }

    // Säljaren eller Admin får bekräfta Delete
    [HttpPost]
    [ActionName("Delete")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var car = await _context.Cars.FindAsync(id);

        if (car == null)
        {
            return NotFound();
        }

        if (!CanManageCar(car))
        {
            return Forbid();
        }

        _context.Cars.Remove(car);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // Kontrollerar om användaren är ägare eller Admin
    private bool CanManageCar(Car car)
    {
        string? currentUserId = _userManager.GetUserId(User);

        bool isOwner =
            car.SellerId != null &&
            car.SellerId == currentUserId;

        bool isAdmin = User.IsInRole("Admin");

        return isOwner || isAdmin;
    }

    private bool CarExists(int id)
    {
        return _context.Cars.Any(car => car.Id == id);
    }
}
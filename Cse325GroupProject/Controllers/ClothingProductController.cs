using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Cse325GroupProject.Models;
using Cse325GroupProject.Services;
using MongoDB.Driver;

namespace Cse325GroupProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClothingProductsController : ControllerBase
{
    private readonly ClothingProductService _service;

    public ClothingProductsController(ClothingProductService service)
    {
        _service = service;
    }

    // -------------------------
    // PUBLIC ENDPOINTS
    // -------------------------

    // Anyone can view all products
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<Product>>> GetAll() =>
        await _service.GetAllAsync();

    // Anyone can view a single product
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<Product>> GetById(string id)
    {
        Console.WriteLine(BCrypt.Net.BCrypt.HashPassword("1234"));
        var product = await _service.GetByIdAsync(id);
        if (product is null) return NotFound();
        return product;
    }

    // -------------------------
    // ADMIN / EMPLOYEE ENDPOINTS
    // -------------------------

    // Only Admins can create products
    [HttpPost]
    [Authorize(Roles = "admin,employee")]
    public async Task<IActionResult> Create(Product product)
    {
        await _service.CreateAsync(product);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    // Admins and Employees can update products
    [HttpPut("{id}")]
    [Authorize(Roles = "admin,employee")]
    public async Task<IActionResult> Update(string id, Product product)
    {
        var existing = await _service.GetByIdAsync(id);
        if (existing is null) return NotFound();

        product.Id = id;
        await _service.UpdateAsync(id, product);
        return NoContent();
    }

    // Only Admins can delete products
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(string id)
    {
        var existing = await _service.GetByIdAsync(id);
        if (existing is null) return NotFound();

        await _service.DeleteAsync(id);
        return NoContent();
    }

    // -------------------------
    // DEBUG ENDPOINT (optional)
    // -------------------------

    [HttpGet("debug")]
    [AllowAnonymous]
    public IActionResult Debug([FromServices] IConfiguration config, [FromServices] IMongoClient client)
    {
        var dbName = config["MongoDB:DatabaseName"];
        var database = client.GetDatabase(dbName);

        var collections = database.ListCollectionNames().ToList();

        return Ok(new
        {
            DatabaseName = dbName,
            Collections = collections
        });
    }
}
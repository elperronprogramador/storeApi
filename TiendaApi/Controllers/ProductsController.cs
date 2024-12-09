using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaApi.Data;
using TiendaApi.Entities;

namespace TiendaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly StoreContext _storeContext;

        public ProductsController(StoreContext storeContext)
        {
            _storeContext = storeContext;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var products = await _storeContext.Products.ToListAsync(); 
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    
        [HttpGet("Search")]
        public async Task<IActionResult> SearchByName(string name)
        {
            try
            {
           
                if (string.IsNullOrWhiteSpace(name))
                {
                    return BadRequest("Product name cannot be empty.");
                }
 
                var products = await _storeContext.Products
                    .Where(p => EF.Functions.ILike(p.Name, $"%{name}%"))  
                    .ToListAsync();

                if (!products.Any())
                {
                    return NotFound("No products found matching the given name.");
                }

                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Product product)
        {

            try
            {
                await _storeContext.Products.AddAsync(product);
                await _storeContext.SaveChangesAsync();
      
                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                var product = await _storeContext.Products.FindAsync(id);

                if (product == null)
                {
                    return NotFound($"Product with Id {id} not found.");
                }
 
                _storeContext.Products.Remove(product);
                await _storeContext.SaveChangesAsync();

                return NoContent();  
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody]Product product)
        {
            try
            {
                var existingProduct = await _storeContext.Products.FindAsync(id);

                if (existingProduct == null)
                {
                    return NotFound($"Product with Id {id} not found.");
                }

                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.Amount  = product.Amount;
                existingProduct.Img = product.Img;

                await _storeContext.SaveChangesAsync();

                return Ok(existingProduct);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

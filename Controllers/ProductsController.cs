using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;
using backend.DTOs;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<Product>>> GetProducts([FromQuery] PaginationRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var query = _context.Products.AsQueryable();

                
                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    query = query.Where(p => EF.Functions.Like(p.Name.ToLower(), $"%{request.Search.ToLower()}%"));
                }

                // Ordenar por nombre
                query = query.OrderBy(p => p.Name);

                // Obtener el total de elementos
                var totalItems = await query.CountAsync();

                // Calcular paginación
                var totalPages = (int)Math.Ceiling((double)totalItems / request.PageSize);

                // Aplicar paginación
                var products = await query
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                var result = new PagedResult<Product>
                {
                    Data = products,
                    CurrentPage = request.Page,
                    PageSize = request.PageSize,
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    HasPreviousPage = request.Page > 1,
                    HasNextPage = request.Page < totalPages,
                    Search = request.Search
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(CreateProductDto productDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validaciones adicionales
                if (string.IsNullOrWhiteSpace(productDto.Name))
                {
                    return BadRequest(new { message = "El nombre del producto no puede estar vacío" });
                }

                if (productDto.Name.Trim().Length < 2)
                {
                    return BadRequest(new { message = "El nombre debe tener al menos 2 caracteres" });
                }

                if (productDto.Price <= 0)
                {
                    return BadRequest(new { message = "El precio debe ser mayor a 0" });
                }

                if (productDto.Stock < 0)
                {
                    return BadRequest(new { message = "El stock no puede ser negativo" });
                }

                // Verificar si ya existe un producto con el mismo nombre
                var existingProduct = await _context.Products
                    .FirstOrDefaultAsync(p => p.Name.ToLower() == productDto.Name.Trim().ToLower());
                
                if (existingProduct != null)
                {
                    return BadRequest(new { message = "Ya existe un producto con ese nombre" });
                }

                var product = new Product
                {
                    Name = productDto.Name.Trim(),
                    Price = productDto.Price,
                    Stock = productDto.Stock
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetProducts), null, product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al crear el producto", details = ex.Message });
            }
        }




    }
}
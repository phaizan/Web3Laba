using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using back.Models;
using back.DataContext;

namespace back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly MyDataContext _dbContext;

        public CartController(MyDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/cart/{userId}
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCartItems(int userId)
        {
            var cart = await _dbContext.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Tour)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
                return NotFound("Cart is empty.");

            return Ok(cart.CartItems.Select(ci => new
            {
                ci.Id,
                ci.TourId,
                TourName = ci.Tour.TourName,
                ci.Quantity,
                ci.TotalPrice
            }));
        }

        // POST: api/cart/{userId}
        [HttpPost("{userId}")]
        public async Task<IActionResult> AddToCart(int userId, [FromBody] CartItemDto newCartItem)
        {
            if (newCartItem == null || newCartItem.Quantity <= 0)
                return BadRequest("Invalid item details.");

            var cart = await _dbContext.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CartItems = new List<CartItem>()
                };
                _dbContext.Carts.Add(cart);
            }

            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.TourId == newCartItem.TourId);

            if (existingItem != null)
            {
                existingItem.Quantity += newCartItem.Quantity;
                existingItem.TotalPrice = existingItem.Quantity * existingItem.Tour.Price; // Обновление цены
            }
            else
            {
                var tour = await _dbContext.Tours.FindAsync(newCartItem.TourId);
                if (tour == null)
                    return NotFound("Tour not found.");

                var cartItem = new CartItem
                {
                    Tour = tour,
                    TourId = newCartItem.TourId,
                    Quantity = newCartItem.Quantity,
                    TotalPrice = newCartItem.Quantity * tour.Price, // Расчет итоговой цены
                    Cart = cart
                };
                cart.CartItems.Add(cartItem);
                _dbContext.CartItems.Add(cartItem);
            }


            await _dbContext.SaveChangesAsync();
            return Ok("Item added to cart.");
        }


        // PUT: api/cart/{userId}/{cartItemId}
        [HttpPut("{userId}/{cartItemId}")]
        public async Task<IActionResult> UpdateCartItem(int userId, int cartItemId, [FromBody] CartItemDto updatedCartItem)
        {
            var cartItem = await _dbContext.CartItems
                .Include(ci => ci.Tour) // Добавьте Include для Tour
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.Cart.UserId == userId);

            if (cartItem == null)
                return NotFound("Item not found in the cart.");

            cartItem.Quantity += updatedCartItem.Quantity;

            if (cartItem.Quantity > 0)
            {
                 cartItem.TotalPrice = cartItem.Quantity * cartItem.Tour.Price; // Пересчет цены
                _dbContext.CartItems.Update(cartItem);
            }
            else
            {
                _dbContext.CartItems.Remove(cartItem);
            }

            await _dbContext.SaveChangesAsync();

            return Ok("Cart item updated.");
        }




        // DELETE: api/cart/{userId}/{cartItemId}
        [HttpDelete("{userId}/{cartItemId}")]
        public async Task<IActionResult> RemoveFromCart(int userId, int cartItemId)
        {
            var cartItem = await _dbContext.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.Cart.UserId == userId);

            if (cartItem == null)
                return NotFound("Item not found in the cart.");

            _dbContext.CartItems.Remove(cartItem);
            await _dbContext.SaveChangesAsync();

            return Ok("Item removed from cart.");
        }

        // DELETE: api/cart/{userId}
        [HttpDelete("{userId}")]
        public async Task<IActionResult> ClearCart(int userId)
        {
            var cart = await _dbContext.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
                return NotFound("Cart is already empty.");

            _dbContext.CartItems.RemoveRange(cart.CartItems);
            await _dbContext.SaveChangesAsync();

            return Ok("Cart cleared.");
        }
    }

    // DTO for incoming requests
    public class CartItemDto
    {
        public int TourId { get; set; }
        public int Quantity { get; set; }
    }
}

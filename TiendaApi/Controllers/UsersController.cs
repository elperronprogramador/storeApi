using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaApi.Data;
using TiendaApi.Dtos.User;
using TiendaApi.Entities;

namespace TiendaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly StoreContext _storeContext; 

        public UsersController(StoreContext storeContext)
        {
            _storeContext = storeContext;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var users = await _storeContext.Users.ToListAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserCreateDto data)
        {


            try
            {
                var usere = await _storeContext.Users.FirstOrDefaultAsync(u =>
                   u.Email == data.Email
               );
                if (usere != null) return BadRequest("repit email");


                var password = HashPassword(data.Password);
                var user = new User { 
                Name= data.Name,
                Email= data.Email,
                Password= password,
                };

                await _storeContext.Users.AddAsync(user);
                await _storeContext.SaveChangesAsync();
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto data)
        {
            try
            {
                var user = await _storeContext.Users.FirstOrDefaultAsync(u =>
                    u.Email == data.Email
                );
                if (user == null) return BadRequest("error");

                var password = HashPassword(data.Password);

                if (password != user.Password) return BadRequest("Retry");

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                
                byte[] bytes = Encoding.UTF8.GetBytes(password);
 
                byte[] hash = sha256.ComputeHash(bytes);

               
                StringBuilder builder = new StringBuilder();
                foreach (var b in hash)
                {
                    builder.Append(b.ToString("x2")); 
                }

                return builder.ToString();
            }
        }

    }
}

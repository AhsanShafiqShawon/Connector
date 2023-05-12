using API.Data;
using API.DTOs;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly ApplicationDbContext _context;
        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpPost("register")] // POST: api/account/register
        public async Task<ActionResult<AppUser>> Register(RegisterDTO registerDTO)
        {
            if(await UserExists(registerDTO.Username))  return BadRequest("Username is taken");
            using var hmac = new HMACSHA512();
            var user = new AppUser
            {
                UserName = registerDTO.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
                PasswordSalt = hmac.Key
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }
        [HttpPost("login")] // POST: api/account/login
        public async Task<ActionResult<AppUser>> Login(LoginDTO loginDTO)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == loginDTO.Username);
            if (user == null) return Unauthorized("invalid user");

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));
            for(int i = 0; i < computedHash.Length; i++)
            {
                if (user.PasswordHash[i] != computedHash[i])
                    return Unauthorized("invalid password");
            }
            return user;
        }
        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Backend.Entities;
using Backend.Entities.Models;
using Backend.Payloads;
using Microsoft.AspNetCore.Authorization;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.Collections.Generic;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

            private IConfiguration _config { get; }
            private readonly BackendContext _db;

            public AccountController(BackendContext db, IConfiguration configuration)
            {
                _config = configuration;
                _db = db;
            }

            [AllowAnonymous]
            [HttpPost("register")]
            public ActionResult<User> Register([FromBody]RegisterPayload registerPayload)
            {
            
                try
                {
                    MailAddress m = new MailAddress(registerPayload.Email);
                }   
                catch (Exception ex)
                {
                    
                    return new JsonResult(new { status = "false", message = "email format "+ registerPayload.Email });
                }
            
            
                try
                {
                    var existingUserWithMail = _db.Users
                .Any(u => u.Email == registerPayload.Email);

                    if (existingUserWithMail)
                    {
                      return Conflict(new { status = false, message = "Email Exists" });
                    }

                var userToCreate = new User
                {
                    Email = registerPayload.Email,
                    UserName = registerPayload.UserName,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerPayload.Password),
                    Gender = registerPayload.Gender,
                    Role = "SimpleUser",
                    };
               
                _db.Users.Add(userToCreate);
                    
                    

                    _db.SaveChanges();

                    return Ok(new { status = true, user = userToCreate, firstname = registerPayload.UserName });
                }
                catch (Exception ex)
                {
                return new JsonResult(new { status = "false", message = ""+ ex.Message });
            }
            
        }

            [AllowAnonymous]
            [Route("login")]
            [HttpPost]
            public async Task<IActionResult> Login([FromBody]LoginPayload loginPayload)
            {
                try
                {
                   MailAddress m = new MailAddress(loginPayload.Email);
                }
                catch (FormatException)
                {
                    return new JsonResult(new { status = "false", message = "email format" });
                }
                 var foundUser = _db.Users
                    .SingleOrDefault(u => u.Email == loginPayload.Email);

                if (foundUser != null)
                {
                if (BCrypt.Net.BCrypt.Verify(loginPayload.Password, foundUser.PasswordHash))
                {
                        var tokenString = GenerateJSONWebToken(foundUser);

                         return new JsonResult(new
                        {
                        status = "true",
                        firstName = foundUser.UserName,
                        Id = foundUser.Id,
                        token= tokenString

                        });
                    }
                    return BadRequest(new { status = false, message = "Wrong password or email " });
                }
                else
                {
                    return BadRequest(new { status = false, message = "No user with this email found" });
                }

            }

        [HttpPatch("changeUsername")]
            public async Task<IActionResult> changeUsername(long id, string newUsername)
            {
                var foundUser = _db.Users
                   .SingleOrDefault(u => u.Id == id);

                foundUser.UserName = newUsername;

                _db.SaveChanges();

                return new JsonResult(new { status = "Success" });

            }

        [HttpPatch("changePassword")]
        public async Task<IActionResult> changePassword(long id, string newPassword)
        {
            var foundUser = _db.Users
               .SingleOrDefault(u => u.Id == id);

            foundUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

            _db.SaveChanges();

            return new JsonResult(new { status = "Success" });

        }

        private string GenerateJSONWebToken(User user)
            {

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("Role", user.Role),
                new Claim("Id", user.Id.ToString()),
                };

                var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                  _config["Jwt:Issuer"],
                  claims,
                  expires: DateTime.Now.AddDays(30),
                  signingCredentials: credentials);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
        
    }
}

using Backend.Entities;
using Backend.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Backend.Enum;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IConfiguration _config { get; }
        private readonly BackendContext _db;
        public UserController(BackendContext db, IConfiguration configuration)
        {
            _config = configuration;
            _db = db;
        }

        [HttpGet("getuser/{id}")]
        public async Task<ActionResult<User>> GetUserById([FromRoute] long id)
        {
            try
            {
                return _db.Users
                    .Include(user => user.Followers)
                    .Include(user => user.Following)
                    .Where(user => id == user.Id)
                    .Single();

            }
            catch (Exception)
            {
                return new JsonResult(new { status = "false", message = "user id not found" });
            }
        }
        [HttpGet("allUsers")]
        public async Task<ActionResult<List<User>>> AllUsers(int pageSize, int pageNumber, UsersSortType sortType)
        {
            var currentUser = HttpContext.User;
            if (currentUser.HasClaim(claims => claims.Type == "Role"))
            {
                var role = currentUser.Claims.FirstOrDefault(c => c.Type == "Role").Value;
                if (role == "Admin")
                {
                    var usersQuery = _db.Users.AsNoTracking().AsQueryable();

                    if (sortType == UsersSortType.FirstNameAscendent)
                        usersQuery = usersQuery.OrderBy(u => u.UserName);
                    else if (sortType == UsersSortType.FirstNameDescendent)
                        usersQuery = usersQuery.OrderBy(u => u.UserName);
                    else if (sortType == UsersSortType.LastNameAscendent)
                        usersQuery = usersQuery.OrderBy(u => u.UserName);
                    else if (sortType == UsersSortType.LastNameDescendent)
                        usersQuery = usersQuery.OrderBy(u => u.UserName);
                    else usersQuery = usersQuery.OrderBy(u => u.UserName);

                    usersQuery = usersQuery
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize);

                    var users = usersQuery.ToList();

                    return users;
                }
            }
            return new JsonResult(new { status = "false" });
        }

        [HttpPost("Follow")]
        public async Task<IActionResult> Follow(String myUser, String followUser)
        {

            var followQuery = _db.Follows.Where(follow => myUser == follow.User && followUser == follow.Follows);

            if (myUser == followUser) return new JsonResult(new { status = "sameUser" });

            if (!followQuery.Any())
            {

                var follower = new Follow
                {
                    User = myUser,
                    Follows = followUser
                };

                _db.Follows.Add(follower);
                _db.SaveChanges();

                return new JsonResult(new { status = "follow" });

            }
            else
            {
                _db.Follows.Remove(followQuery.Single());
                _db.SaveChanges();
                return new JsonResult(new { status = "unfollow" });
            }

        }

        [HttpGet("getUserList")]
        public async Task<HashSet<String>> getUserList(string user)
        {
            var followQuery = _db.Follows.Where(follow => user == follow.User);
            var userList = new HashSet<string>();

            foreach (Follow f in followQuery.ToList())
            {
                userList.Add(f.Follows);
            }

            return userList;

        }
    }
}

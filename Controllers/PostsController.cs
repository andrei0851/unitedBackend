using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Backend.Entities;
using Backend.Entities.Models;
using Microsoft.Extensions.Configuration;
using Backend.Payloads;
using System.Diagnostics;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : Controller
    {
        private IConfiguration _config { get; }
        private readonly BackendContext _db;

        public PostsController(BackendContext db, IConfiguration configuration)
        {
            _config = configuration;
            _db = db;
        }

        [HttpPost("addpost")]
        public async Task<IActionResult> addpost([FromBody]PostPayload postPayload)
        {
            var post = new Post();
            post.UserName = postPayload.UserName;
            post.imageLink = postPayload.imgLink;
            post.text = postPayload.text;

            _db.Posts.Add(post);
            _db.SaveChanges();

            return Ok(new { status = true });

        }

        [HttpDelete("deletePost")]
        public async Task<IActionResult> deletePost(String postID)
        {
            int post = Int32.Parse(postID);
            var foundPost = _db.Posts
               .Where(p => p.Id == post).Single();

            _db.Posts.Remove(foundPost);
            _db.SaveChanges();


            return new JsonResult(new { status = "Success" });

        }

        [HttpPost("like")]
        public async Task<IActionResult> like(String postID)
        {
            int post = Int32.Parse(postID);
            var foundPost = _db.Posts
               .Where(p => p.Id == post).Single();

            foundPost.Likes++;
            _db.SaveChanges();


            return new JsonResult(new { status = "Success" });

        }

        [HttpGet("getAllPosts")]
        public async Task<List<Post>> getAllPosts()
        {

            var postQuery = _db.Posts.AsNoTracking();
            return postQuery.ToList();

        }

        [HttpGet("getFollowingPosts")]
        public async Task<List<Post>> getFollowingPosts(string user)
        {
            var followQuery = _db.Follows.Where(follow => user == follow.User);
            var userList = new HashSet<string>();

            foreach (Follow f in followQuery.ToList())
            {
                userList.Add(f.Follows);
            }

            var postList = new List<Post>();

            foreach (string s in userList)
            {
                var postQuery = _db.Posts.Where(post => s == post.UserName);
                postList.AddRange(postQuery.ToList());
            }

            return postList;

        }

        [HttpGet("getUserPosts")]
        public async Task<List<Post>> getUserPosts(String UserName)
        {
            var postQuery = _db.Posts.AsNoTracking();
            postQuery = postQuery.Where(post => post.UserName == UserName);
            return postQuery.ToList();
        }


    }
}

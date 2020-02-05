using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Middleware.Models;

namespace Middleware.Controllers
{
    [Route("/todo")]
    [ApiController]
    public class TodoControllers : ControllerBase
    {
        private AppDBContext _appDbContext { get; set; }

        public TodoControllers(AppDBContext appDBContext)
        {
            _appDbContext = appDBContext;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            var token = System.IO.File.ReadAllLines("Token.txt").Last();
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtSecurityTokenHandler.ReadToken(token) as JwtSecurityToken;
            var sub = securityToken.Claims.First(u => u.Type == "sub").Value;
            var x = from l in _appDbContext.Post where l.UserId == Convert.ToInt32(sub) select l;
            return Ok(x);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Post([FromBody] Posts posts)
        {
            var token = System.IO.File.ReadAllLines("Token.txt").Last();
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtSecurityTokenHandler.ReadToken(token) as JwtSecurityToken;
            var sub = securityToken?.Claims.First(u => u.Type == "sub").Value;
            posts.UserId = Convert.ToInt32(sub);
            _appDbContext.Post.Add(posts);
            _appDbContext.SaveChanges();
            return Ok(_appDbContext.Post.Include(u => u.User).ToList());
        }

        [Authorize]
        [HttpPatch("{id}")]
        public IActionResult Update(int id, [FromBody] Posts post)
        {
            var token = System.IO.File.ReadAllLines("Token.txt").Last();

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtSecurityTokenHandler.ReadToken(token) as JwtSecurityToken;
            var find = _appDbContext.Post.Find(id);
            find.list = post.list;
            _appDbContext.SaveChanges();
            return Ok(_appDbContext.Post.Include(u => u.User).ToList());
        }

        [Authorize]
        [HttpPatch("done/{id}")]
        public IActionResult Done(int id)
        {
            var token = System.IO.File.ReadAllLines("Token.txt").Last();

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtSecurityTokenHandler.ReadToken(token) as JwtSecurityToken;
            var todo = _appDbContext.Post.Find(id);
            todo.status = true;
            _appDbContext.SaveChanges();
            return Ok(_appDbContext.Post.Include(u => u.User).ToList());
        }

        [Authorize]
        [HttpDelete("delete/{id}")]
        public ActionResult<string> Delete(int id)
        {
            var token = System.IO.File.ReadAllLines("Token.txt").Last();

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtSecurityTokenHandler.ReadToken(token) as JwtSecurityToken;
            var find = _appDbContext.Post.Find(id);
            _appDbContext.Remove(find);
            _appDbContext.SaveChanges();
            return Ok("Berhasil menghapus");
        }

        [Authorize]
        [HttpDelete("clear")]
        public ActionResult<string> Clear()
        {
            var token = System.IO.File.ReadAllLines("Token.txt").Last();

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtSecurityTokenHandler.ReadToken(token) as JwtSecurityToken;
            var sub = securityToken.Claims.First(u => u.Type == "sub").Value;
            var x = from l in _appDbContext.Post where l.UserId == Convert.ToInt32(sub) select l;
            _appDbContext.Post.RemoveRange(x);
            _appDbContext.SaveChanges();
            return Ok("Data berhasil dihapus semua");
        }
    }
}
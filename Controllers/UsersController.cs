//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Mmt.Api.Models;

//namespace Mmt.Api.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class UsersController : ControllerBase
//    {
//        private readonly MmtContext _context;

//        public UsersController(MmtContext context)
//        {
//            _context = context;
//        }

//        [HttpGet("search")]
//        public async Task<ActionResult<IEnumerable<User>>> Search(string? userName)
//        {
//            try
//            {
//                IQueryable<User> query = _context.Users;
//                if (!string.IsNullOrEmpty(userName))
//                {
//                    query = query.Where(u => u.UserName.Contains(userName));
//                }

//                var result = await query.ToListAsync();

//                if (result.Any())
//                {
//                    return Ok(result);
//                }

//                return NotFound();
//            }
//            catch (Exception)
//            {
//                return StatusCode(StatusCodes.Status500InternalServerError,
//                    "Error retrieving data from the database");
//            }
//        }

//        //voor een later gebruik
//        //[HttpGet("search")]
//        //public async Task<ActionResult<IEnumerable<User>>> Search(string? firstName, string? lastName)
//        //{
//        //    try
//        //    {
//        //        IQueryable<User> query = _context.Users;
//        //        if (!string.IsNullOrEmpty(firstName))
//        //        {
//        //            query = query.Where(u => u.FirstName.Contains(firstName));
//        //        }

//        //        if (lastName != null)
//        //        {
//        //            query = query.Where(e => e.LastName == lastName);
//        //        }

//        //        var result = await query.ToListAsync();

//        //        if (result.Any())
//        //        {
//        //            return Ok(result);
//        //        }

//        //        return NotFound();
//        //    }
//        //    catch (Exception)
//        //    {
//        //        return StatusCode(StatusCodes.Status500InternalServerError,
//        //            "Error retrieving data from the database");
//        //    }
//        //}



//        // GET: api/Users
//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<User>>> Getusers()
//        {
//            return await _context.Users.ToListAsync();
//        }

//        // GET: api/Users/5
//        [HttpGet("{id}")]
//        public async Task<ActionResult<User>> GetUser(int id)
//        {
//            var user = await _context.Users.FindAsync(id);

//            if (user == null)
//            {
//                return NotFound();
//            }

//            return user;
//        }

//        // PUT: api/Users/5
//        // To protect from overposting attacks, enable the specific properties you want to bind to, for
//        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
//        [HttpPut("{id}")]
//        public async Task<IActionResult> PutUser(int id, User user)
//        {
//            if (id != user.Id)
//            {
//                return BadRequest();
//            }

//            _context.Entry(user).State = EntityState.Modified;

//            try
//            {
//                await _context.SaveChangesAsync();
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                if (!UserExists(id))
//                {
//                    return NotFound();
//                }
//                else
//                {
//                    throw;
//                }
//            }

//            return NoContent();
//        }

//        // POST: api/Users
//        // To protect from overposting attacks, enable the specific properties you want to bind to, for
//        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
//        [HttpPost]
//        public async Task<ActionResult<User>> PostUser(User user)
//        {
//            _context.Users.Add(user);
//            await _context.SaveChangesAsync();

//            return CreatedAtAction("GetUser", new { id = user.Id }, user);
//        }

//        // DELETE: api/Users/5
//        [HttpDelete("{id}")]
//        public async Task<ActionResult<User>> DeleteUser(int id)
//        {
//            var user = await _context.Users.FindAsync(id);
//            if (user == null)
//            {
//                return NotFound();
//            }

//            _context.Users.Remove(user);
//            await _context.SaveChangesAsync();

//            return user;
//        }

//        private bool UserExists(int id)
//        {
//            return _context.Users.Any(e => e.Id == id);
//        }
//    }
//}

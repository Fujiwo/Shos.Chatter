using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shos.Chatter.Server.Controllers
{
    using Models;

    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        readonly ChatterContext context;

        public UsersController(ChatterContext context)
            => this.context = context;

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
            => context.Users is null
               ? new User[] {}
               : await context.Users.Where(user => !user.HasDeleted).OrderBy(user => user.Name).ToListAsync();

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            if (context.Users is null)
                return NotFound();

            var user = await context.Users.FindAsync(id);

            if (user is null || user.HasDeleted)
                return NotFound();

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
                return BadRequest();

            context.Entry(user).State = EntityState.Modified;

            try {
                await context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (UserExists(id))
                    throw;
                else
                    return NotFound();
            }
            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            if (context.Users is null)
                return BadRequest();

            user.InsertDateTime = DateTime.Now;
            context.Users.Add(user);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            if (context.Users is null)
                return NotFound();

            var user = await context.Users.FindAsync(id);
            if (user is null)
                return NotFound();

            user.HasDeleted = true;
            context.Entry(user).State = EntityState.Modified;
            //context.Users.Remove(user);
            await context.SaveChangesAsync();
            return user;
        }

        bool UserExists(int id) => context.Users is not null && context.Users.Any(user => user.Id == id && !user.HasDeleted);
    }
}
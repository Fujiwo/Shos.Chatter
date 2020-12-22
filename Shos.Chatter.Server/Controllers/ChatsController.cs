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
    public class ChatsController : ControllerBase
    {
        readonly ChatterContext context;

        public ChatsController(ChatterContext context)
            => this.context = context;

        // GET: api/Chats
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Chat>>> GetChats()
            => context.Chats is null ? new Chat[] {}
                                     : await context.Chats.OrderByDescending(chat => chat.UpdateDateTime).ToListAsync();

        // GET: api/Chats/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Chat>> GetChat(int id)
        {
            if (context.Chats is null)
                return NotFound();

            var chat = await context.Chats.FindAsync(id);

            if (chat is null)
                return NotFound();

            return chat;
        }

        // PUT: api/Chats/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutChat(int id, Chat chat)
        {
            if (id != chat.Id)
                return BadRequest();

            chat.UpdateDateTime = DateTime.Now;
            context.Entry(chat).State = EntityState.Modified;

            try {
                await context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (ChatExists(id))
                    throw;
                else
                    return NotFound();
            }
            return NoContent();
        }

        // POST: api/Chats
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Chat>> PostChat(Chat chat)
        {
            if (context.Chats is null || !UserExists(chat.UserId))
                return BadRequest();

            chat.InsertDateTime = chat.UpdateDateTime = DateTime.Now;
            context.Chats.Add(chat);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetChat", new { id = chat.Id }, chat);
        }

        // DELETE: api/Chats/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Chat>> DeleteChat(int id)
        {
            if (context.Chats is null)
                return NotFound();

            var chat = await context.Chats.FindAsync(id);
            if (chat is null)
                return NotFound();

            context.Chats.Remove(chat);
            await context.SaveChangesAsync();
            return chat;
        }

        bool ChatExists(int id) => context.Chats is not null && context.Chats.Any(chat => chat.Id == id);
        bool UserExists(int id) => context.Users is not null && context.Users.Any(user => user.Id == id && !user.HasDeleted);
    }
}
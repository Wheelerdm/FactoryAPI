using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FactoryAPI;
using FactoryAPI.Models;
using Microsoft.AspNetCore.SignalR;
using FactoryAPI.SignalR;

namespace FactoryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FactoriesController : ControllerBase
    {
        private readonly DB1Context _context;
        private IHubContext<NotifyHub, ITypedHubClient> _hubContext;

        public FactoriesController(DB1Context context, IHubContext<NotifyHub, ITypedHubClient> hubContext)
        {
            this._context = context;
            this._hubContext = hubContext;
        }

        // GET: api/Factories
        [HttpGet]
        public async Task<ActionResult<FactoryNode>> GetFactory()
        {
            Factory factory = await _context.Factory.FirstOrDefaultAsync(o => o.parentID == -1);

            FactoryNodeBuilder fnb = new FactoryNodeBuilder(_context);
            FactoryNode fn = fnb.CreateFactoryNode(factory);

            return fn;
        }

        // GET: api/Factories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Factory>> GetFactory(int id)
        {
            var factory = await _context.Factory.FindAsync(id);

            if (factory == null)
            {
                return NotFound();
            }

            return factory;
        }

        // PUT: api/Factories/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFactory(int id, Factory factory)
        {
            if (id != factory.ID)
            {
                return BadRequest();
            }

            _context.Entry(factory).State = EntityState.Modified;

            try
            {
                 _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FactoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            FactoryNodeBuilder fnb = new FactoryNodeBuilder(_context);
            FactoryNode fn = fnb.CreateFactoryNode(factory);

            try
            {
                _hubContext.Clients.All.BroadcastMessage("update", fn);
            }
            catch (Exception e)
            {
                //If the hubContext fails to notify others, The caller doesn't care
                //  but we would log the exception
            }

            return NoContent();
        }

        // POST: api/Factories
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<FactoryNode>> PostFactory(FactoryNode factoryNode)
        {

            //_context.Factory.Add(factory);
            //await _context.SaveChangesAsync();
            if (factoryNode.children != null)
            {
                System.Console.WriteLine(factoryNode.children);
            }

            //return CreatedAtAction("GetFactory", new { id = factory.ID }, factory);
            FactoryNodeBuilder fnb = new FactoryNodeBuilder(_context);
            Factory f = fnb.CreateFactory(factoryNode);

            FactoryNode fn = fnb.CreateFactoryNode(f);
            
            try
            {
                _hubContext.Clients.All.BroadcastMessage("create",fn);
            }
            catch (Exception e)
            {
                //If the hubContext fails to notify others, The caller doesn't care
                //  but we would log the exception
            }

            return fn;
        }

        // DELETE: api/Factories/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Factory>> DeleteFactory(int id)
        {
            var factory = await _context.Factory.FindAsync(id);
            if (factory == null)
            {
                return NotFound();
            }

            //Deleting the root node is not allowed
            if (factory.parentID == -1)
            {
                return NotFound();
            }

            //Find Children & remove them before deleting parent node
            var children = _context.Factory
                     .Where(f => f.parentID == factory.ID).OrderBy(o => o.ID).ToList<Factory>();

            foreach (var childFactory in children)
            {
                CreatedAtAction("DeleteFactory", new { id=childFactory.ID});
            }

            //TODO - Consider removing
            //Not a fan of additional db calls just to delete an item
            //However, we want to be consistent in what our API returns.

            FactoryNodeBuilder fnb = new FactoryNodeBuilder(_context);
            FactoryNode fn = fnb.CreateFactoryNode(factory);
            _context.Factory.Remove(factory);
            _context.SaveChanges();

            try
            {
                _hubContext.Clients.All.BroadcastMessage("delete", fn);
            }
            catch (Exception e)
            {
                //If the hubContext fails to notify others, The caller doesn't care
                //  but we would log the exception
            }

            return factory;
        }

        private bool FactoryExists(int id)
        {
            return _context.Factory.Any(e => e.ID == id);
        }
    }
}

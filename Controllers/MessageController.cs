using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using FactoryAPI.Models;
using FactoryAPI.SignalR;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private IHubContext<NotifyHub, ITypedHubClient> _hubContext;

        public MessageController(IHubContext<NotifyHub, ITypedHubClient> hubContext)
        {
            this._hubContext = hubContext;
        }

        [HttpPost]
        public string Post([FromBody]Message msg)
        {
            string retMsg;
            try
            {
              //  _hubContext.Clients.All.BroadcastMessage(msg.Type, msg.Payload);
                retMsg = "success";
            }
            catch (Exception e)
            {
                retMsg = e.ToString();
            }

            return retMsg;
        }

    }
}

using Domain;
using Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Services;
using System.Text.Json.Nodes;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EchoAPI.Controllers
{

    [Route("api/invitations/")]
    [ApiController]
    public class InvitationsController : ControllerBase
    {
        private ContactService _sevice;
        private ContextData _context;
        private readonly IHubContext<ChatHub> _myHubContext;

        public InvitationsController(ContactService s, ContextData contextData, IHubContext<ChatHub> ch)
        {
            _sevice = s;
            _context = contextData;
            _myHubContext = ch;
        }

        // POST api/<Inventations>
        [HttpPost]
        public IActionResult Post([FromBody] Invitation invt)
        {
            if (!_context.isUserExit(invt.to))
                return NotFound();
            JsonObject json = new JsonObject();
            json.Add("id", invt.from);
            json.Add("name", invt.from);
            json.Add("server", invt.server);
            int code = _sevice.AddContact(json, invt.to);
            if (code == 404)
                return NotFound();
            if (code == 400)
                return BadRequest();
            signal(invt.to);
            return Created("~api/invintations/", invt);
        }

        private async void signal(string groupName)
        {
            await _myHubContext.Clients.Groups(groupName).SendAsync("ReceiveMessage");

        }
    }
}
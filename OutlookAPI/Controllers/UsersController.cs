using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetOutlook.Database;

namespace OutlookAPI.Controllers
{
    [ApiController]
    [Route("NetOutlook/Users")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> logger;
        private readonly OutlookContext context;

        public UsersController(ILogger<UsersController> logger, OutlookContext context)
        {
            this.logger = logger;
            this.context = context;
        }


        [HttpGet]
        public IEnumerable<UserRaw> GetAllUsers()
        {
            List<UserRaw> users= new List<UserRaw>();
            var tab = context.Users.ToArray();
            foreach(User user in tab)
            {
                users.Add(new UserRaw(user));
            }
            return users;
        }

        [HttpGet("id={id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<UserRaw> GetUserById(string id)
        {
            int rId = -1;
            if(!int.TryParse(id,out rId))
            {
                return BadRequest();
            }
            User user = context.Users.Find(rId);
            if(user==null)
            {
                return NotFound();
            }
            return Ok(new UserRaw(user));
        }
    }
}

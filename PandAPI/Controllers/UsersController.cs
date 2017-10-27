using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PandAPI.PandApi;

namespace PandAPI.Controllers
{
    [Route("api/[controller]")]
    public class UsersController :Controller
    {
        private PandaStore store;

        public UsersController(PandaStore store)
        {
            this.store = store;
            
        }

        [HttpPost]
        public IActionResult Post([FromBody]UserModel userModel)
        {
            try
            {
                if (this.store.AddUser(userModel))
                    return Ok(new {message = "ok"});
                
                return BadRequest(new {message = "User already exists"});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new {exception = ex.ToString()});
            }
            

        }
        
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] {"value1", "value2"};
        }
    }
}
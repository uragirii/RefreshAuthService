using RefreshAuthService.Context;
using RefreshAuthService.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace RefreshAuthService.Controllers
{
    public class ClientController : ApiController
    {
        [AllowAnonymous]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public Client Post([FromBody]Client client)
        { 
            using (AuthRepository _repo = new AuthRepository())
            {
                _repo.AddClient(client);
                return client;
            }
        }
    }
}

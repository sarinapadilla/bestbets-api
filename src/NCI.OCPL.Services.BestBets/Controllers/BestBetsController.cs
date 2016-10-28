using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NCI.OCPL.Services.BestBets.Controllers
{
    [Route("[controller]")]
    public class BestBetsController : Controller
    {

        // GET api/values/5
        [HttpGet("{term}")]
        public string Get(string term)
        {
            return term;
        }

    }
}

using AspNet.Security.OpenIdConnect.Server;
using AureliaAspNetCoreAuth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AureliaAspNetCoreAuth.Controllers
{
    [Authorize]
    public class MoviesController : Controller
    {



        [Authorize]
        [HttpGet("/movies")]
        public IActionResult Get()
        {
            return Json(new List<Movie>() {
                    new Movie(1, "Star Wars", 1983),
                    new Movie(2, "Star Trek", 1981),
                    new Movie(3, "Shrek", 2004),
            });
        }
    }
}

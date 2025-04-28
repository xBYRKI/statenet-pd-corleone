using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace statenet_lspd.Controllers
{
    [AllowAnonymous]  // diese ganze Controller‑Klasse ist öffentlich
    public class PublicController : Controller
    {
        // GET /Public
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}

using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace statenet_lspd.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly ApplicationDbContext _db;

        protected BaseController(ApplicationDbContext db)
        {
            _db = db;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                bool hasPending = _db.ServiceInstructions
                    .Any(si => si.IsActive &&
                               !_db.UserInstructionAcceptances
                                  .Any(a => a.UserId == userId && a.ServiceInstructionId == si.Id));
                ViewBag.HasPending = hasPending;
            }
            else
            {
                ViewBag.HasPending = false;
            }

            base.OnActionExecuting(context);
        }
    }
}

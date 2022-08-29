using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System;

namespace Unik.Comments.API.Controllers;

/// <summary>
/// Api Controller Base
/// </summary>
[ApiController]
[Authorize]
[Produces("application/json")]
[Route("/api/v{v:apiVersion}/[controller]")]
public class ApiControllerBase : ControllerBase
{
}
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Roomify.Contracts.RequestModels.Authentication;
using Roomify.Services;

namespace Accelist.Career.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IMediator _mediator;

        private readonly IOptions<AppSettings> _options;

        public AuthenticationController(IMediator mediator, IOptions<AppSettings> options)
        {
            _mediator = mediator;
            _options = options;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] EmailLoginRequestModel request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

    }
}

using MediatR;
using Roomify.Contracts.ResponseModels.Authentication;

namespace Roomify.Contracts.RequestModels.Authentication;

public class EmailLoginRequestModel : IRequest<EmailLoginResponseModel>
{
    public string Email { get; set; }
    public string Password { get; set; }
    public bool RememberMe { get; set;}
}

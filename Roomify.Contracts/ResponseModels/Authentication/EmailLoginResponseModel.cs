namespace Roomify.Contracts.ResponseModels.Authentication;

public class EmailLoginResponseModel
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
}

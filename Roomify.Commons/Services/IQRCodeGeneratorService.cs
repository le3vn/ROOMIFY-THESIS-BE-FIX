namespace Roomify.Commons.Services;
public interface IQRCodeGeneratorService
{
    Task <byte[]> GenerateQRCode(string text);
}
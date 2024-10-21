namespace Roomify.Services
{
    public class AppSettings
    {
        public string FrontEndHost { set; get; } = "";
        public string PrivateKey { get; set; } = "decrypted_rootCA.key";
        public string BackEndHost { get; set; } = "http://localhost:5065/";
    }
}

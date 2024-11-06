namespace Roomify.Contracts.ResponseModels.ManageUsers
{
    public class ListUserResponse
    {
        public List<UserModel> UserList { get; set; } = new List<UserModel>();
        public int TotalData { get; set; }
    }
    public class UserModel
    {
        public string Id { set; get; } = "";

        public string GivenName { set; get; } = "";

        public string FamilyName { set; get; } = "";

        public string Email { set; get; } = "";

        public string MinioUrl { get; set; }= "";
    }
}


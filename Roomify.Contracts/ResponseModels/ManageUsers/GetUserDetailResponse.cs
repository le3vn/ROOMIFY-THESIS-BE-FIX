﻿namespace Roomify.Contracts.ResponseModels.ManageUsers
{
    public class GetUserDetailResponse
    {
        public string Id { set; get; } = "";

        public string GivenName { set; get; } = "";

        public string FamilyName { set; get; } = "";

        public string Email { set; get; } = "";

        public bool IsEnabled { set; get; }
        public string MinioUrl { get; set; } = "";
    }
}

namespace Roomify.Contracts.ResponseModels.ManageRoom
{
    public class GetRoomGroupsResponseModel
    {
        public List<RoomGroupModel> RoomGroups { get; set; } = new List<RoomGroupModel>();
        public int TotalData { get; set; }
    }

    public class RoomGroupModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        // Add other properties of RoomGroup as needed.
    }
}

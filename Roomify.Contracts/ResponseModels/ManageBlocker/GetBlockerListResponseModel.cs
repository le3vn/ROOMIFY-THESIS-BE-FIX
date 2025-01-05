using System;

namespace Roomify.Contracts.ResponseModels.ManageBlocker;

public class GetBlockerListResponseModel
{
    public List<BlockerModel> BlockerLists { get; set; } = new List<BlockerModel>();
    public int TotalData { get; set; }
}

public class BlockerModel{
    public int BlockerId { get; set; }
    public string BlockerName { get; set; } ="";
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public bool IsActive { get; set; }
}


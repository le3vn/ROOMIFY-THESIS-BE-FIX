using System;

namespace Roomify.Contracts.ResponseModels.ManageBlocker;

public class GetBlockerDetailResponseModel
{
    public int BlockerId { get; set; }
    public string BlockerName { get; set; } ="";
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public bool IsActive { get; set; }
}

using System;

namespace Roomify.Contracts.RequestModels.ManageBlob;

public class BlobModel
{
    public Guid FileId { get; set; }
    public string FileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
}

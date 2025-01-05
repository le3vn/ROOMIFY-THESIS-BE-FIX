using System;
using Microsoft.AspNetCore.Http;

namespace Roomify.Contracts.RequestModels.ManageBlob;

public class UploadFileRequest
{
    public IFormFile File { get; set; }
}

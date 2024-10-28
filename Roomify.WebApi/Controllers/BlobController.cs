using Roomify.Commons.Constants;
using Roomify.Commons.Extensions;
using Roomify.Commons.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Roomify.Entities;
using Roomify.Contracts.RequestModels.ManageBlob;

namespace Accelist.Amanat.WebAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BlobController : ControllerBase
    {
        private readonly IStorageService _storageService;
        private readonly IMediator _mediator;
        public BlobController(IStorageService storageService, ApplicationDbContext dbContext,
            IOptions<MinIoOptions> minioOptions, IMediator mediator)
        {
            _storageService = storageService;
            _mediator = mediator;
        }

        [HttpGet("presigned-put-object")]
        public async Task<ActionResult<PutModel>> GetPresignedUrlWrite([FromQuery] string fileName)
        {
            var urlFile = await _storageService.GetPresignedUrlWriteAsync($"{BlobPath.File}{fileName}");

            var putModel = new PutModel
            {
                data = urlFile
            };
            return Ok(putModel);
        }

        [HttpGet("presigned-get-object")]
        public async Task<ActionResult<PutModel>> GetPresignedUrlRead([FromQuery] string fileName)
        {
            var urlFile = await _storageService.GetPresignedUrlReadAsync($"{BlobPath.File}{fileName}");

            var putModel = new PutModel
            {
                data = urlFile
            };
            return Ok(urlFile);
        }

        [HttpGet("redirect")]
        public async Task<ActionResult<PutModel>> GetPresignedUrlRediret([FromQuery] string fileName)
        {
            var urlFile = await _storageService.GetPresignedUrlReadAsync($"{BlobPath.File}{fileName}");

            return Redirect(urlFile);
        }
    }
}

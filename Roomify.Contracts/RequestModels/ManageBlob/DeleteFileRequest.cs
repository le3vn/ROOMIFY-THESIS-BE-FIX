using System;
using MediatR;
using Roomify.Contracts.ResponseModels.ManageBlob;

namespace Roomify.Contracts.RequestModels.ManageBlob;

public class DeleteFileRequest : IRequest<DeleteFileResponse>
	{
		public List<string> Id { get; set; } = new List<string>();
	}

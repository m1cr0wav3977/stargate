using MediatR;
using StargateAPI.Business.Data;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Commands
{
    public class DeleteAstronautDetail : IRequest<DeleteAstronautDetailResult>
    {
        public int Id { get; set; }
    }

    public class DeleteAstronautDetailHandler : IRequestHandler<DeleteAstronautDetail, DeleteAstronautDetailResult>
    {
        private readonly StargateContext _context;

        public DeleteAstronautDetailHandler(StargateContext context)
        {
            _context = context;
        }

        public async Task<DeleteAstronautDetailResult> Handle(DeleteAstronautDetail request, CancellationToken cancellationToken)
        {
            // TODO: Implement
            throw new NotImplementedException();
        }
    }

    public class DeleteAstronautDetailResult : BaseResponse
    {
        // TODO: Add result properties if needed
    }
}


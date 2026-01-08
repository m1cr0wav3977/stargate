using MediatR;
using StargateAPI.Business.Data;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Commands
{
    public class UpdateAstronautDetail : IRequest<UpdateAstronautDetailResult>
    {
        public int Id { get; set; }
        // TODO: Add properties to update
    }

    public class UpdateAstronautDetailHandler : IRequestHandler<UpdateAstronautDetail, UpdateAstronautDetailResult>
    {
        private readonly StargateContext _context;

        public UpdateAstronautDetailHandler(StargateContext context)
        {
            _context = context;
        }

        public async Task<UpdateAstronautDetailResult> Handle(UpdateAstronautDetail request, CancellationToken cancellationToken)
        {
            // TODO: Implement
            throw new NotImplementedException();
        }
    }

    public class UpdateAstronautDetailResult : BaseResponse
    {
        public int? Id { get; set; }
    }
}


using MediatR;
using StargateAPI.Business.Data;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Commands
{
    public class CreateAstronautDetail : IRequest<CreateAstronautDetailResult>
    {
        // TODO: Add properties
    }

    public class CreateAstronautDetailHandler : IRequestHandler<CreateAstronautDetail, CreateAstronautDetailResult>
    {
        private readonly StargateContext _context;

        public CreateAstronautDetailHandler(StargateContext context)
        {
            _context = context;
        }

        public async Task<CreateAstronautDetailResult> Handle(CreateAstronautDetail request, CancellationToken cancellationToken)
        {
            // TODO: Implement
            throw new NotImplementedException();
        }
    }

    public class CreateAstronautDetailResult : BaseResponse
    {
        public int? Id { get; set; }
    }
}


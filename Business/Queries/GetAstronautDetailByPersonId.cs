using MediatR;
using StargateAPI.Business.Data;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Queries
{
    public class GetAstronautDetailByPersonId : IRequest<GetAstronautDetailByPersonIdResult>
    {
        public int PersonId { get; set; }
    }

    public class GetAstronautDetailByPersonIdHandler : IRequestHandler<GetAstronautDetailByPersonId, GetAstronautDetailByPersonIdResult>
    {
        private readonly StargateContext _context;

        public GetAstronautDetailByPersonIdHandler(StargateContext context)
        {
            _context = context;
        }

        public async Task<GetAstronautDetailByPersonIdResult> Handle(GetAstronautDetailByPersonId request, CancellationToken cancellationToken)
        {
            // TODO: Implement
            throw new NotImplementedException();
        }
    }

    public class GetAstronautDetailByPersonIdResult : BaseResponse
    {
        public AstronautDetail? AstronautDetail { get; set; }
    }
}




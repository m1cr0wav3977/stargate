using MediatR;
using StargateAPI.Business.Data;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Queries
{
    public class GetAstronautDetailById : IRequest<GetAstronautDetailByIdResult>
    {
        public int Id { get; set; }
    }

    public class GetAstronautDetailByIdHandler : IRequestHandler<GetAstronautDetailById, GetAstronautDetailByIdResult>
    {
        private readonly StargateContext _context;

        public GetAstronautDetailByIdHandler(StargateContext context)
        {
            _context = context;
        }

        public async Task<GetAstronautDetailByIdResult> Handle(GetAstronautDetailById request, CancellationToken cancellationToken)
        {
            // TODO: Implement
            throw new NotImplementedException();
        }
    }

    public class GetAstronautDetailByIdResult : BaseResponse
    {
        public AstronautDetail? AstronautDetail { get; set; }
    }
}




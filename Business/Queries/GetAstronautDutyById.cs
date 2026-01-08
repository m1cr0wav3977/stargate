using MediatR;
using StargateAPI.Business.Data;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Queries
{
    public class GetAstronautDutyById : IRequest<GetAstronautDutyByIdResult>
    {
        public int Id { get; set; }
    }

    public class GetAstronautDutyByIdHandler : IRequestHandler<GetAstronautDutyById, GetAstronautDutyByIdResult>
    {
        private readonly StargateContext _context;

        public GetAstronautDutyByIdHandler(StargateContext context)
        {
            _context = context;
        }

        public async Task<GetAstronautDutyByIdResult> Handle(GetAstronautDutyById request, CancellationToken cancellationToken)
        {
            // TODO: Implement
            throw new NotImplementedException();
        }
    }

    public class GetAstronautDutyByIdResult : BaseResponse
    {
        public AstronautDuty? AstronautDuty { get; set; }
    }
}


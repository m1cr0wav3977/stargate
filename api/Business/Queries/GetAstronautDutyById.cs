using Dapper;
using MediatR;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
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
            var result = new GetAstronautDutyByIdResult();

            var query = "SELECT * FROM [AstronautDuty] WHERE @Id = Id";

            var astronautDuty = await _context.Connection.QueryAsync<AstronautDuty>(
                query,
                new { Id = request.Id }
            );

            result.AstronautDuty = astronautDuty.FirstOrDefault();

            return result;
        }
    }

    public class GetAstronautDutyByIdResult : BaseResponse
    {
        public AstronautDuty? AstronautDuty { get; set; }
    }
}


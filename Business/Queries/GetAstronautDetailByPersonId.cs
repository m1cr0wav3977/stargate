using Dapper;
using MediatR;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
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
            var result = new GetAstronautDetailByPersonIdResult();

            var query = $"SELECT * FROM [AstronautDetail] WHERE '{request.PersonId}' = PersonId";

            var astronautDetail = await _context.Connection.QueryAsync<AstronautDetail>(query);

            result.AstronautDetail = astronautDetail.FirstOrDefault();

            return result;
        }
    }

    public class GetAstronautDetailByPersonIdResult : BaseResponse
    {
        public AstronautDetail? AstronautDetail { get; set; }
    }
}




using Dapper;
using MediatR;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
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
            var result = new GetAstronautDetailByIdResult();

            var query = $"SELECT * FROM [AstronautDetail] WHERE '{request.Id}' = Id";

            var astronautDetail = await _context.Connection.QueryAsync<AstronautDetail>(query);

            result.AstronautDetail = astronautDetail.FirstOrDefault();

            return result;
        }
    }

    public class GetAstronautDetailByIdResult : BaseResponse
    {
        public AstronautDetail? AstronautDetail { get; set; }
    }
}




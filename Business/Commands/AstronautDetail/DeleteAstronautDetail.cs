using Dapper;
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
            var query = $"SELECT * FROM [AstronautDetail] WHERE \'{request.Id}\' = Id";

            var astronautDetail = await _context.Connection.QueryFirstOrDefaultAsync<AstronautDetail>(query);

            if (astronautDetail is null) throw new BadHttpRequestException("Bad Request");

            await _context.Connection.ExecuteAsync($"DELETE FROM [AstronautDetail] WHERE \'{request.Id}\' = Id");

            await _context.SaveChangesAsync();

            return new DeleteAstronautDetailResult() { Id = request.Id };


        }
    }

    public class DeleteAstronautDetailResult : BaseResponse
    {
        public int Id { get; set; }
    }
}
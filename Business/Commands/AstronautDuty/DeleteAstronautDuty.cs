using Dapper;
using MediatR;
using StargateAPI.Business.Data;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Commands
{
    public class DeleteAstronautDuty : IRequest<DeleteAstronautDutyResult>
    {
        public int Id { get; set; }
    }

    public class DeleteAstronautDutyHandler : IRequestHandler<DeleteAstronautDuty, DeleteAstronautDutyResult>
    {
        private readonly StargateContext _context;

        public DeleteAstronautDutyHandler(StargateContext context)
        {
            _context = context;
        }

        public async Task<DeleteAstronautDutyResult> Handle(DeleteAstronautDuty request, CancellationToken cancellationToken)
        {
            var query = $"SELECT * FROM [AstronautDuty] WHERE \'{request.Id}\' = Id";

            var astronautDuty = await _context.Connection.QueryFirstOrDefaultAsync<AstronautDuty>(query);

            if (astronautDuty is null) throw new BadHttpRequestException("Bad Request");

            await _context.Connection.ExecuteAsync($"DELETE FROM [AstronautDuty] WHERE \'{request.Id}\' = Id");

            await _context.SaveChangesAsync();

            return new DeleteAstronautDutyResult() { Id = request.Id };

        }

        
    }

    public class DeleteAstronautDutyResult : BaseResponse
    {
        public int Id { get; set; }
    }
}
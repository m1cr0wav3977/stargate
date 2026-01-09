using Dapper;
using MediatR;
using StargateAPI.Business.Data;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Commands
{
    public class DeletePerson : IRequest<DeletePersonResult>
    {
        public int Id { get; set; }
    }

    public class DeletePersonHandler : IRequestHandler<DeletePerson, DeletePersonResult>
    {
        private readonly StargateContext _context;

        public DeletePersonHandler(StargateContext context)
        {
            _context = context;
        }

        public async Task<DeletePersonResult> Handle(DeletePerson request, CancellationToken cancellationToken)
        {
            var query = $"SELECT * FROM [Person] WHERE \'{request.Id}\' = Id";

            var person = await _context.Connection.QueryFirstOrDefaultAsync<Person>(query);

            if (person is null) throw new BadHttpRequestException("Bad Request");

            await _context.Connection.ExecuteAsync($"DELETE FROM [Person] WHERE \'{request.Id}\' = Id");

            await _context.Connection.ExecuteAsync($"DELETE FROM [AstronautDetail] WHERE \'{request.Id}\' = PersonId");

            await _context.Connection.ExecuteAsync($"DELETE FROM [AstronautDuty] WHERE \'{request.Id}\' = PersonId");

            await _context.SaveChangesAsync();

            return new DeletePersonResult() { Id = request.Id };
        }

        
    }

    public class DeletePersonResult : BaseResponse
    {
        public int Id { get; set; }
    }

}
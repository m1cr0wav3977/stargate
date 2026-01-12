using Dapper;
using MediatR;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Queries
{
    public class GetPersonById : IRequest<GetPersonByIdResult>
    {
        public int Id { get; set; }
    }

    public class GetPersonByIdHandler : IRequestHandler<GetPersonById, GetPersonByIdResult>
    {
        private readonly StargateContext _context;

        public GetPersonByIdHandler(StargateContext context)
        {
            _context = context;
        }

        public async Task<GetPersonByIdResult> Handle(GetPersonById request, CancellationToken cancellationToken)
        {
            var result = new GetPersonByIdResult();

            var query = "SELECT Id as PersonId, Name FROM [Person] WHERE @Id = Id";

            var person = await _context.Connection.QueryAsync<PersonAstronaut>(
                query,
                new { Id = request.Id }
            );

            result.Person = person.FirstOrDefault();

            return result;
        }
    }

    public class GetPersonByIdResult : BaseResponse
    {
        public PersonAstronaut? Person { get; set; }
    }
}


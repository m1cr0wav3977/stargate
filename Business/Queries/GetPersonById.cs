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
            // TODO: Implement
            throw new NotImplementedException();
        }
    }

    public class GetPersonByIdResult : BaseResponse
    {
        public PersonAstronaut? Person { get; set; }
    }
}


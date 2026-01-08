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
            // TODO: Implement
            throw new NotImplementedException();
        }
    }

    public class DeletePersonResult : BaseResponse
    {
        public int Id { get; set; }
    }
}


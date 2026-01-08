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
            // TODO: Implement
            throw new NotImplementedException();
        }
    }

    public class DeleteAstronautDutyResult : BaseResponse
    {
        // TODO: Add result properties if needed
    }
}


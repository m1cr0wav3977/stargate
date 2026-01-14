using MediatR;
using Microsoft.EntityFrameworkCore;
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
            var person = await _context.People
                .Include(p => p.AstronautDetail)
                .Include(p => p.AstronautDuties)
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (person is null) throw new BadHttpRequestException("Bad Request");

            if (person.AstronautDetail != null)
            {
                _context.AstronautDetails.Remove(person.AstronautDetail);
            }

            if (person.AstronautDuties.Any())
            {
                _context.AstronautDuties.RemoveRange(person.AstronautDuties);
            }

            _context.People.Remove(person);

            await _context.SaveChangesAsync(cancellationToken);

            return new DeletePersonResult() { Id = request.Id };
        }

    }

    public class DeletePersonResult : BaseResponse
    {
        public int Id { get; set; }
    }

}
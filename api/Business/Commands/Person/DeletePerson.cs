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
            // Load person with related entities for cascade delete
            var person = await _context.People
                .Include(p => p.AstronautDetail)
                .Include(p => p.AstronautDuties)
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (person is null) throw new BadHttpRequestException("Bad Request");

            // EF Core will handle cascade delete if configured, otherwise remove related entities explicitly
            if (person.AstronautDetail != null)
            {
                _context.AstronautDetails.Remove(person.AstronautDetail);
            }

            if (person.AstronautDuties.Any())
            {
                _context.AstronautDuties.RemoveRange(person.AstronautDuties);
            }

            // Remove the person (cascade delete should handle related entities if configured)
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
using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Controllers;
using System.Net;

namespace StargateAPI.Business.Commands
{
    public class UpdateAstronautDuty : IRequest<UpdateAstronautDutyResult>
    {
        public int Id { get; set; }
        
        public string Rank { get; set; } = string.Empty;
        
        public string DutyTitle { get; set; } = string.Empty;
        
        public DateTime DutyStartDate { get; set; }
        
        public DateTime? DutyEndDate { get; set; }
    }

    public class UpdateAstronautDutyHandler : IRequestHandler<UpdateAstronautDuty, UpdateAstronautDutyResult>
    {
        private readonly StargateContext _context;

        public UpdateAstronautDutyHandler(StargateContext context)
        {
            _context = context;
        }

        public async Task<UpdateAstronautDutyResult> Handle(UpdateAstronautDuty request, CancellationToken cancellationToken)
        {
            var astronautDuty = await _context.AstronautDuties.FirstOrDefaultAsync(z => z.Id == request.Id);

            if (astronautDuty is null) throw new BadHttpRequestException("Bad Request");

            if (astronautDuty.Rank != request.Rank)
            {
                astronautDuty.Rank = request.Rank;
            }
            if (astronautDuty.DutyTitle != request.DutyTitle)
            {
                astronautDuty.DutyTitle = request.DutyTitle;
            }
            if (astronautDuty.DutyStartDate != request.DutyStartDate)
            {
                astronautDuty.DutyStartDate = request.DutyStartDate;
            }
            if (astronautDuty.DutyEndDate != request.DutyEndDate)
            {
                astronautDuty.DutyEndDate = request.DutyEndDate;
            }

            _context.AstronautDuties.Update(astronautDuty);

            await _context.SaveChangesAsync();

            return new UpdateAstronautDutyResult() { Id = astronautDuty.Id };
        }
    }

    public class UpdateAstronautDutyResult : BaseResponse
    {
        public int? Id { get; set; }
    }
}




using Dapper;
using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Controllers;
using System.Net;

namespace StargateAPI.Business.Commands
{
    public class AddAstronautDuty : IRequest<AddAstronautDutyResult>
    {
        public required string Name { get; set; }
        public required int PersonId { get; set; }
        public required string Rank { get; set; }
        public required string DutyTitle { get; set; }

        public DateTime DutyStartDate { get; set; }
    }

    public class AddAstronautDutyPreProcessor : IRequestPreProcessor<AddAstronautDuty>
    {
        private readonly StargateContext _context;

        public AddAstronautDutyPreProcessor(StargateContext context)
        {
            _context = context;
        }

        public Task Process(AddAstronautDuty request, CancellationToken cancellationToken)
        {
            var personID = _context.People.AsNoTracking().FirstOrDefault(z => z.Name == request.Name)?.Id;

            if (personID is null) throw new BadHttpRequestException("Person not found");

            var hasExistingDuties = _context.AstronautDuties.Any(z => z.PersonId == personID);

            // The first duty must have the title "Spaceman"
            if (!hasExistingDuties && !request.DutyTitle.Equals("Spaceman", StringComparison.OrdinalIgnoreCase))
            {
                throw new BadHttpRequestException("To enroll in the astronaut program, the first duty title must be 'Spaceman'");
            }

            var verifyNoPreviousDuty = _context.AstronautDuties.FirstOrDefault(z => z.PersonId == personID && z.DutyTitle == request.DutyTitle && z.DutyStartDate == request.DutyStartDate);

            if (verifyNoPreviousDuty is not null) throw new BadHttpRequestException("Duty already exists for this person and date");
            
            return Task.CompletedTask;
        }
    }

    public class AddAstronautDutyHandler : IRequestHandler<AddAstronautDuty, AddAstronautDutyResult>
    {
        private readonly StargateContext _context;

        public AddAstronautDutyHandler(StargateContext context)
        {
            _context = context;
        }
        public async Task<AddAstronautDutyResult> Handle(AddAstronautDuty request, CancellationToken cancellationToken)
        {
            var hasExistingDuties = await _context.AstronautDuties
                .AnyAsync(z => z.PersonId == request.PersonId, cancellationToken);

            if (!hasExistingDuties && !request.DutyTitle.Equals("Spaceman", StringComparison.OrdinalIgnoreCase))
            {
                throw new BadHttpRequestException("To enroll in the astronaut program, the first duty title must be 'Spaceman'");
            }

            var currentDuty = await _context.AstronautDuties
                .FirstOrDefaultAsync(z => z.PersonId == request.PersonId && z.DutyEndDate == null, cancellationToken);

            if (currentDuty != null)
            {
                currentDuty.DutyEndDate = request.DutyStartDate.AddDays(-1).Date;
                _context.AstronautDuties.Update(currentDuty);
            }

            var newAstronautDuty = new AstronautDuty()
            {
                PersonId = request.PersonId,
                Rank = request.Rank,
                DutyTitle = request.DutyTitle,
                DutyStartDate = request.DutyStartDate.Date,
                DutyEndDate = null
            };

            await _context.AstronautDuties.AddAsync(newAstronautDuty);

            var astronautDetail = await _context.AstronautDetails
                .FirstOrDefaultAsync(z => z.PersonId == request.PersonId, cancellationToken);

            if (astronautDetail == null)
            {
                var careerEndDate = (DateTime?)null;
                
                if (request.DutyTitle.Equals("RETIRED", StringComparison.OrdinalIgnoreCase))
                {
                    careerEndDate = request.DutyStartDate.AddDays(-1).Date;
                }

                astronautDetail = new AstronautDetail()
                {
                    PersonId = request.PersonId,
                    CurrentRank = request.Rank,
                    CurrentDutyTitle = request.DutyTitle,
                    CareerStartDate = request.DutyStartDate.Date,
                    CareerEndDate = careerEndDate
                };
                await _context.AstronautDetails.AddAsync(astronautDetail);
            }
            else
            {
                astronautDetail.CurrentRank = request.Rank;
                astronautDetail.CurrentDutyTitle = request.DutyTitle;

                if (request.DutyTitle.Equals("RETIRED", StringComparison.OrdinalIgnoreCase))
                {
                    astronautDetail.CareerEndDate = request.DutyStartDate.AddDays(-1).Date;
                }

                _context.AstronautDetails.Update(astronautDetail);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new AddAstronautDutyResult()
            {
                Id = newAstronautDuty.Id
            };
        }
    }

    public class AddAstronautDutyResult : BaseResponse
    {
        public int? Id { get; set; }
    }
}


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
            // Check if this is the first duty for this person
            var hasExistingDuties = await _context.AstronautDuties
                .AnyAsync(z => z.PersonId == request.PersonId, cancellationToken);

            // Validate that the first duty must be "Spaceman"
            if (!hasExistingDuties && !request.DutyTitle.Equals("Spaceman", StringComparison.OrdinalIgnoreCase))
            {
                throw new BadHttpRequestException("To enroll in the astronaut program, the first duty title must be 'Spaceman'");
            }

            // Rule 5: A Person's Previous Duty End Date is set to the day before the New Astronaut Duty Start Date
            // Find the current duty (where DutyEndDate is null) for this person
            var currentDuty = await _context.AstronautDuties
                .FirstOrDefaultAsync(z => z.PersonId == request.PersonId && z.DutyEndDate == null, cancellationToken);

            if (currentDuty != null)
            {
                // Set the previous duty's end date to one day before the new duty's start date
                currentDuty.DutyEndDate = request.DutyStartDate.AddDays(-1).Date;
                _context.AstronautDuties.Update(currentDuty);
            }

            // Rule 3 & 4: A Person will only ever hold one current Astronaut Duty at a time
            // A Person's Current Duty will not have a Duty End Date
            var newAstronautDuty = new AstronautDuty()
            {
                PersonId = request.PersonId,
                Rank = request.Rank,
                DutyTitle = request.DutyTitle,
                DutyStartDate = request.DutyStartDate.Date,
                DutyEndDate = null  // Current duty has no end date
            };

            await _context.AstronautDuties.AddAsync(newAstronautDuty);

            // Get or create AstronautDetail for this person
            var astronautDetail = await _context.AstronautDetails
                .FirstOrDefaultAsync(z => z.PersonId == request.PersonId, cancellationToken);

            if (astronautDetail == null)
            {
                // Rule 2: A Person who has not had an astronaut assignment will not have Astronaut records
                // After creating the first duty (Spaceman), we need to create the AstronautDetail
                var careerEndDate = (DateTime?)null;
                
                // Rule 6 & 7: A Person is classified as 'Retired' when a Duty Title is 'RETIRED'
                // A Person's Career End Date is one day before the Retired Duty Start Date
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
                // Update the current rank and duty title
                astronautDetail.CurrentRank = request.Rank;
                astronautDetail.CurrentDutyTitle = request.DutyTitle;

                // Rule 6 & 7: A Person is classified as 'Retired' when a Duty Title is 'RETIRED'
                // A Person's Career End Date is one day before the Retired Duty Start Date
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


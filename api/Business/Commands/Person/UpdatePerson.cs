using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Controllers;
using System.Net;

namespace StargateAPI.Business.Commands
{
    public class UpdatePerson : IRequest<UpdatePersonResult>
    {
        public int Id { get; set; }
        
        public string Name { get; set; } = string.Empty;

        // Optional AstronautDetail properties
        public string? CurrentRank { get; set; }
        public string? CurrentDutyTitle { get; set; }
        public DateTime? CareerStartDate { get; set; }
        public DateTime? CareerEndDate { get; set; }
        public string? Rank { get; set; }
        public string? DutyTitle { get; set; }
        public DateTime? DutyStartDate { get; set; }
        public DateTime? DutyEndDate { get; set; }
    }

    public class UpdatePersonPreProcessor : IRequestPreProcessor<UpdatePerson>
    {
        private readonly StargateContext _context;

        public UpdatePersonPreProcessor(StargateContext context)
        {
            _context = context;
        }

        public Task Process(UpdatePerson request, CancellationToken cancellationToken)
        {
            var person = _context.People.AsNoTracking().FirstOrDefault(z => z.Id == request.Id);

            if (person is null) throw new BadHttpRequestException("Bad Request");

            return Task.CompletedTask;
        }
    }

    public class UpdatePersonHandler : IRequestHandler<UpdatePerson, UpdatePersonResult>
    {
        private readonly StargateContext _context;

        public UpdatePersonHandler(StargateContext context)
        {
            _context = context;
        }

        public async Task<UpdatePersonResult> Handle(UpdatePerson request, CancellationToken cancellationToken)
        {
            var person = await _context.People.FirstOrDefaultAsync(z => z.Id == request.Id);

            if (person is null) throw new BadHttpRequestException("Bad Request");

            // Update Person Name
            if (!string.IsNullOrEmpty(request.Name) && person.Name != request.Name)
            {
                person.Name = request.Name;
            }

            // Update existing AstronautDetail if any properties provided
            if (request.CurrentRank != null || request.CurrentDutyTitle != null || request.CareerStartDate.HasValue || request.CareerEndDate.HasValue)
            {
                var astronautDetail = await _context.AstronautDetails.FirstOrDefaultAsync(z => z.PersonId == request.Id);

                if (astronautDetail == null) throw new BadHttpRequestException("Bad Request - AstronautDetail does not exist");

                if (request.CurrentRank != null && astronautDetail.CurrentRank != request.CurrentRank)
                {
                    astronautDetail.CurrentRank = request.CurrentRank;
                }
                if (request.CurrentDutyTitle != null && astronautDetail.CurrentDutyTitle != request.CurrentDutyTitle)
                {
                    astronautDetail.CurrentDutyTitle = request.CurrentDutyTitle;
                }
                if (request.CareerStartDate.HasValue && astronautDetail.CareerStartDate != request.CareerStartDate.Value)
                {
                    astronautDetail.CareerStartDate = request.CareerStartDate.Value;
                }
                if (request.CareerEndDate.HasValue && astronautDetail.CareerEndDate != request.CareerEndDate)
                {
                    astronautDetail.CareerEndDate = request.CareerEndDate;
                }
                _context.AstronautDetails.Update(astronautDetail);
            }

            // Update existing AstronautDuty if any properties provided (updates most recent duty)
            if (request.Rank != null || request.DutyTitle != null || request.DutyStartDate.HasValue || request.DutyEndDate.HasValue)
            {
                var astronautDuty = await _context.AstronautDuties
                    .Where(z => z.PersonId == request.Id)
                    .OrderByDescending(z => z.DutyStartDate)
                    .FirstOrDefaultAsync();

                if (astronautDuty == null) throw new BadHttpRequestException("Bad Request - AstronautDuty does not exist");

                if (request.Rank != null && astronautDuty.Rank != request.Rank)
                {
                    astronautDuty.Rank = request.Rank;
                }
                if (request.DutyTitle != null && astronautDuty.DutyTitle != request.DutyTitle)
                {
                    astronautDuty.DutyTitle = request.DutyTitle;
                }
                if (request.DutyStartDate.HasValue && astronautDuty.DutyStartDate != request.DutyStartDate.Value)
                {
                    astronautDuty.DutyStartDate = request.DutyStartDate.Value;
                }
                if (request.DutyEndDate.HasValue && astronautDuty.DutyEndDate != request.DutyEndDate)
                {
                    astronautDuty.DutyEndDate = request.DutyEndDate;
                }
                _context.AstronautDuties.Update(astronautDuty);
            }

            _context.People.Update(person);
            await _context.SaveChangesAsync();

            return new UpdatePersonResult() { Id = person.Id };
        }
    }

    public class UpdatePersonResult : BaseResponse
    {
        public int Id { get; set; }
    }
}
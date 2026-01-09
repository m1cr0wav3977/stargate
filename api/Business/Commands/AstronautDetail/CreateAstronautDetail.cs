using Dapper;
using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Controllers;
using System.Net;

namespace StargateAPI.Business.Commands
{
    public class CreateAstronautDetail : IRequest<CreateAstronautDetailResult>
    {
        public int Id { get; set; }

        public int PersonId { get; set; }

        public string CurrentRank { get; set; } = string.Empty;

        public string CurrentDutyTitle { get; set; } = string.Empty;

        public DateTime CareerStartDate { get; set; }

        public DateTime? CareerEndDate { get; set; }

        public virtual Person? Person { get; set; }
    }

    public class CreateAstronautDetailPreProcessor : IRequestPreProcessor<CreateAstronautDetail>
    {
        private readonly StargateContext _context;

        public CreateAstronautDetailPreProcessor(StargateContext context)
        {
            _context = context;
        }

        public Task Process(CreateAstronautDetail request, CancellationToken cancellationToken)
        {

            var person = _context.People.AsNoTracking().FirstOrDefault(z => z.Id == request.PersonId);

            if (person is null) throw new BadHttpRequestException("Bad Request");

            return Task.CompletedTask;
        }
    }

    public class CreateAstronautDetailHandler : IRequestHandler<CreateAstronautDetail, CreateAstronautDetailResult>
    {
        private readonly StargateContext _context;

        public CreateAstronautDetailHandler(StargateContext context)
        {
            _context = context;
        }

        public async Task<CreateAstronautDetailResult> Handle(CreateAstronautDetail request, CancellationToken cancellationToken)
        {
            var query = $"SELECT * FROM [Person] WHERE {request.PersonId} = Id";

            var person = await _context.Connection.QueryFirstOrDefaultAsync<Person>(query);

            if (person is null) throw new BadHttpRequestException("Bad Request");

            var astronautDetail = await _context.AstronautDetails.AsNoTracking().FirstOrDefaultAsync(z => z.PersonId == request.PersonId);

            if (astronautDetail == null)
            {
                astronautDetail = new AstronautDetail()
                {
                    PersonId = request.PersonId,
                    CurrentDutyTitle = request.CurrentDutyTitle,
                    CurrentRank = request.CurrentRank,
                    CareerStartDate = request.CareerStartDate.Date,
                    CareerEndDate = request.CareerEndDate?.Date
                };

                await _context.AstronautDetails.AddAsync(astronautDetail);

                await _context.SaveChangesAsync();
                
            }
            
            return new CreateAstronautDetailResult()
            {
                Id = astronautDetail.Id
            };
            
        }
    }

    public class CreateAstronautDetailResult : BaseResponse
    {
        public int? Id { get; set; }
    }
}


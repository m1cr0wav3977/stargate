using Dapper;
using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Controllers;
using System.Net;

namespace StargateAPI.Business.Commands
{
    public class UpdateAstronautDetail : IRequest<UpdateAstronautDetailResult>
    {
        public int Id { get; set; }
        
        public string CurrentRank { get; set; } = string.Empty;
        
        public string CurrentDutyTitle { get; set; } = string.Empty;
        
        public DateTime CareerStartDate { get; set; }
        
        public DateTime? CareerEndDate { get; set; }
    }

    public class UpdateAstronautDetailHandler : IRequestHandler<UpdateAstronautDetail, UpdateAstronautDetailResult>
    {
        private readonly StargateContext _context;

        public UpdateAstronautDetailHandler(StargateContext context)
        {
            _context = context;
        }

        public async Task<UpdateAstronautDetailResult> Handle(UpdateAstronautDetail request, CancellationToken cancellationToken)
        {
            var astronautDetail = await _context.AstronautDetails.FirstOrDefaultAsync(z => z.Id == request.Id);

            if (astronautDetail is null) throw new BadHttpRequestException("Bad Request");

            if (astronautDetail.CurrentRank != request.CurrentRank)
            {
                astronautDetail.CurrentRank = request.CurrentRank;
            }
            if (astronautDetail.CurrentDutyTitle != request.CurrentDutyTitle)
            {
                astronautDetail.CurrentDutyTitle = request.CurrentDutyTitle;
            }
            if (astronautDetail.CareerStartDate != request.CareerStartDate)
            {
                astronautDetail.CareerStartDate = request.CareerStartDate;
            }
            if (astronautDetail.CareerEndDate != request.CareerEndDate)
            {
                astronautDetail.CareerEndDate = request.CareerEndDate;
            }

            _context.AstronautDetails.Update(astronautDetail);

            await _context.SaveChangesAsync();

            return new UpdateAstronautDetailResult() { Id = astronautDetail.Id };
        }
    }

    public class UpdateAstronautDetailResult : BaseResponse
    {
        public int? Id { get; set; }
    }
}


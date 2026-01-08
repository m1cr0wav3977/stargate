using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Commands
{
    public class UpdatePerson : IRequest<UpdatePersonResult>
    {
        public required int PersonId { get; set; }

        public required int AstronautDetailId { get; set; }

        public required string CurrentRank { get; set; } = string.Empty;

    }

     // public class UpdatePersonPreProcessor : IRequestPreProcessor<UpdatePerson>
     // {
     //     private readonly StargateContext _context;
     //     public UpdatePersonPreProcessor(StargateContext context)
     //     {
     //         _context = context;
     //     }
     //     public Task Process(UpdatePerson request, CancellationToken cancellationToken)
     //     {
     //         var person = _context.People.AsNoTracking().FirstOrDefault(z => z.Id == request.PersonId);

     //         if (person is null) throw new BadHttpRequestException("Bad Request");

     //         var astronautDetail = _context.AstronautDetails.AsNoTracking().FirstOrDefault(z => z.PersonId == request.PersonId);

     //         if (astronautDetail is null)
     //         {
     //             astronautDetail = new AstronautDetail();
     //             astronautDetail.PersonId = request.PersonId;
     //             astronautDetail.CurrentDutyTitle = request.CurrentDutyTitle;
     //             astronautDetail.CurrentRank = request.CurrentRank;
     //             astronautDetail.CareerStartDate = request.CareerStartDate;
     //             astronautDetail.CareerEndDate = request.CareerEndDate;
     //         }

     //         return Task.CompletedTask;
     //     }
     // }

     public class UpdatePersonHandler : IRequestHandler<UpdatePerson, UpdatePersonResult>
     {
         private readonly StargateContext _context;

         public UpdatePersonHandler(StargateContext context)
         {
             _context = context;
         }
         public async Task<UpdatePersonResult> Handle(UpdatePerson request, CancellationToken cancellationToken)
         {
             // TODO: Implement
             // var person = await _context.People.FindAsync(request.PersonId);

             // if (person is null) throw new BadHttpRequestException("Bad Request");

             // person.Name = request.Name;

             // var astronautDetail = await _context.AstronautDetail.FindAsync(request.PersonId);

             // if (astronautDetail is null)
             // {
             //     astronautDetail = new AstronautDetail();
             //     astronautDetail.PersonId = request.PersonId;
             //     astronautDetail.CurrentDutyTitle = request.CurrentDutyTitle;
             //     astronautDetail.CurrentRank = request.CurrentRank;
             //     astronautDetail.CareerStartDate = request.CareerStartDate ?? DateTime.MinValue;
             //     astronautDetail.CareerEndDate = request.CareerEndDate ?? DateTime.MinValue;
             //     await _context.AstronautDetail.AddAsync(astronautDetail);
             // }
             // else
             // {
             //     astronautDetail.CurrentDutyTitle = request.CurrentDutyTitle;
             //     astronautDetail.CurrentRank = request.CurrentRank;
             //     astronautDetail.CareerStartDate = request.CareerStartDate ?? DateTime.MinValue;
             //     astronautDetail.CareerEndDate = request.CareerEndDate ?? DateTime.MinValue;
             // }

             throw new NotImplementedException();
         }

      
     }

    public class UpdatePersonResult : BaseResponse
    {
        public int Id { get; set; }
    }

}
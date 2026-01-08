using MediatR;
using Microsoft.AspNetCore.Mvc;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Queries;
using System.Net;

namespace StargateAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AstronautDetailController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AstronautDetailController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Create

        [HttpPost("")]
        public async Task<IActionResult> CreateAstronautDetail([FromBody] CreateAstronautDetail request)
        {
            try
            {
                var result = await _mediator.Send(request);
                return this.GetResponse(result);
            }
            catch (NotImplementedException)
            {
                return this.GetResponse(new BaseResponse()
                {
                    Message = "Not implemented",
                    Success = false,
                    ResponseCode = 202
                });
            }
            catch (Exception ex)
            {
                return this.GetResponse(new BaseResponse()
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }
        }

        // Read
        [HttpGet("person/{personId:int}")]
        public async Task<IActionResult> GetAstronautDetailByPersonId(int personId)
        {
            try
            {
                var result = await _mediator.Send(new GetAstronautDetailByPersonId()
                {
                    PersonId = personId
                });

                return this.GetResponse(result);
            }
            catch (NotImplementedException)
            {
                return this.GetResponse(new BaseResponse()
                {
                    Message = "Not implemented",
                    Success = false,
                    ResponseCode = 202
                });
            }
            catch (Exception ex)
            {
                return this.GetResponse(new BaseResponse()
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }            
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAstronautDetailById(int id)
        {
            try
            {
                var result = await _mediator.Send(new GetAstronautDetailById()
                {
                    Id = id
                });

                return this.GetResponse(result);
            }
            catch (NotImplementedException)
            {
                return this.GetResponse(new BaseResponse()
                {
                    Message = "Not implemented",
                    Success = false,
                    ResponseCode = 202
                });
            }
            catch (Exception ex)
            {
                return this.GetResponse(new BaseResponse()
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }            
        }

        // Update

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAstronautDetail(int id, [FromBody] UpdateAstronautDetail request)
        {
            try
            {
                request.Id = id;
                var result = await _mediator.Send(request);

                return this.GetResponse(result);
            }
            catch (NotImplementedException)
            {
                return this.GetResponse(new BaseResponse()
                {
                    Message = "Not implemented",
                    Success = false,
                    ResponseCode = 202
                });
            }
            catch (Exception ex)
            {
                return this.GetResponse(new BaseResponse()
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }
        }

        // Delete

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAstronautDetail(int id)
        {
            try
            {
                var result = await _mediator.Send(new DeleteAstronautDetail()
                {
                    Id = id
                });

                return this.GetResponse(result);
            }
            catch (NotImplementedException)
            {
                return this.GetResponse(new BaseResponse()
                {
                    Message = "Not implemented",
                    Success = false,
                    ResponseCode = 202
                });
            }
            catch (Exception ex)
            {
                return this.GetResponse(new BaseResponse()
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }
        }
    }
}
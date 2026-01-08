using MediatR;
using Microsoft.AspNetCore.Mvc;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Queries;
using System.Net;

namespace StargateAPI.Controllers
{
   
    [ApiController]
    [Route("[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PersonController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Create

        [HttpPost("")]
        public async Task<IActionResult> CreatePerson([FromBody] string name)
        {
            try
            {
                var result = await _mediator.Send(new CreatePerson()
                {
                    Name = name
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

        // Read

        [HttpGet("")]
        public async Task<IActionResult> GetPeople()
        {
            try
            {
                var result = await _mediator.Send(new GetPeople()
                {

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

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetPersonByName(string name)
        {
            try
            {
                var result = await _mediator.Send(new GetPersonByName()
                {
                    Name = name
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
        public async Task<IActionResult> GetPersonById(int id)
        {
            try
            {
                var result = await _mediator.Send(new GetPersonById()
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
        public async Task<IActionResult> UpdatePerson(int id, [FromBody] UpdatePerson request)
        {
            try
            {
                request.PersonId = id;
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
        public async Task<IActionResult> DeletePerson(int id)
        {
            try
            {
                var result = await _mediator.Send(new DeletePerson()
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
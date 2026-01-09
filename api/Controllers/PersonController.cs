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
        private readonly ILogger<PersonController> _logger;
        
        public PersonController(IMediator mediator, ILogger<PersonController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        // Create

        [HttpPost("")]
        public async Task<IActionResult> CreatePerson([FromBody] CreatePerson request)
        {
            var requestMethod = HttpContext.Request.Method;
            var requestIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var endpoint = HttpContext.Request.Path;
            _logger.LogInformation("Request: {Method} | IP: {Ip} | Endpoint: {Endpoint}", requestMethod, requestIp, endpoint);
            try
            {
                var result = await _mediator.Send(request);

                _logger.LogInformation("CreatePerson result: {Result}", result);
                return this.GetResponse(result);
            }
            catch (NotImplementedException)
            {
                _logger.LogWarning("CreatePerson not implemented for name: {Name}", request?.Name ?? "Unknown");
                return this.GetResponse(new BaseResponse()
                {
                    Message = "Not implemented",
                    Success = false,
                    ResponseCode = 202
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreatePerson failed for name: {Name}", request?.Name ?? "Unknown");
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
            var requestMethod = HttpContext.Request.Method;
            var requestIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var endpoint = HttpContext.Request.Path;
            _logger.LogInformation("Request: {Method} | IP: {Ip} | Endpoint: {Endpoint}", requestMethod, requestIp, endpoint);
            try
            {
                var result = await _mediator.Send(new GetPeople()
                {

                });

                _logger.LogInformation("GetPeople result: {Result}", result);
                return this.GetResponse(result);
            }
            catch (NotImplementedException)
            {
                _logger.LogWarning("GetPeople not implemented");
                return this.GetResponse(new BaseResponse()
                {
                    Message = "Not implemented",
                    Success = false,
                    ResponseCode = 202
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetPeople failed");
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
            var requestMethod = HttpContext.Request.Method;
            var requestIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var endpoint = HttpContext.Request.Path;
            _logger.LogInformation("Request: {Method} | IP: {Ip} | Endpoint: {Endpoint}", requestMethod, requestIp, endpoint);
            try
            {
                var result = await _mediator.Send(new GetPersonByName()
                {
                    Name = name
                });

                _logger.LogInformation("GetPersonByName result: {Result}", result);
                return this.GetResponse(result);
            }
            catch (NotImplementedException)
            {
                _logger.LogWarning("GetPersonByName not implemented for name: {Name}", name);
                return this.GetResponse(new BaseResponse()
                {
                    Message = "Not implemented",
                    Success = false,
                    ResponseCode = 202
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetPersonByName failed for name: {Name}", name);
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
            var requestMethod = HttpContext.Request.Method;
            var requestIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var endpoint = HttpContext.Request.Path;
            _logger.LogInformation("Request: {Method} | IP: {Ip} | Endpoint: {Endpoint}", requestMethod, requestIp, endpoint);
            try
            {
                var result = await _mediator.Send(new GetPersonById()
                {
                    Id = id
                });

                _logger.LogInformation("GetPersonById result: {Result}", result);
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
            var requestMethod = HttpContext.Request.Method;
            var requestIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var endpoint = HttpContext.Request.Path;
            _logger.LogInformation("Request: {Method} | IP: {Ip} | Endpoint: {Endpoint}", requestMethod, requestIp, endpoint);
            try
            {
                request.PersonId = id;
                var result = await _mediator.Send(request);

                _logger.LogInformation("UpdatePerson result: {Result}", result);
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
            var requestMethod = HttpContext.Request.Method;
            var requestIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var endpoint = HttpContext.Request.Path;
            _logger.LogInformation("Request: {Method} | IP: {Ip} | Endpoint: {Endpoint}", requestMethod, requestIp, endpoint);
            try
            {
                var result = await _mediator.Send(new DeletePerson()
                {
                    Id = id
                });

                _logger.LogInformation("DeletePerson result: {Result}", result);
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
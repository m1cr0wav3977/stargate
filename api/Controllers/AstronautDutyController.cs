using MediatR;
using Microsoft.AspNetCore.Mvc;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Queries;
using System.Net;

namespace StargateAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AstronautDutyController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AstronautDutyController> _logger;
        
        public AstronautDutyController(IMediator mediator, ILogger<AstronautDutyController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        // Create

        [HttpPost("")]
        public async Task<IActionResult> AddAstronautDuty([FromBody] AddAstronautDuty request)
        {
            var requestMethod = HttpContext.Request.Method;
            var requestIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var endpoint = HttpContext.Request.Path;
            _logger.LogInformation("Request: {Method} | IP: {Ip} | Endpoint: {Endpoint}", requestMethod, requestIp, endpoint);
            try
            {
                var result = await _mediator.Send(request);
                _logger.LogInformation("AddAstronautDuty result: {Result}", result);
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

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetAstronautDutiesByName(string name)
        {
            var requestMethod = HttpContext.Request.Method;
            var requestIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var endpoint = HttpContext.Request.Path;
            _logger.LogInformation("Request: {Method} | IP: {Ip} | Endpoint: {Endpoint}", requestMethod, requestIp, endpoint);
            try
            {
                var result = await _mediator.Send(new GetAstronautDutiesByName()
                {
                    Name = name
                });

                _logger.LogInformation("GetAstronautDutiesByName result: {Result}", result);
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
        public async Task<IActionResult> GetAstronautDutyById(int id)
        {
            var requestMethod = HttpContext.Request.Method;
            var requestIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var endpoint = HttpContext.Request.Path;
            _logger.LogInformation("Request: {Method} | IP: {Ip} | Endpoint: {Endpoint}", requestMethod, requestIp, endpoint);
            try
            {
                var result = await _mediator.Send(new GetAstronautDutyById()
                {
                    Id = id
                });

                _logger.LogInformation("GetAstronautDutyById result: {Result}", result);
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

        // [HttpPut("{id:int}")]
        // public async Task<IActionResult> UpdateAstronautDuty(int id, [FromBody] UpdateAstronautDuty request)
        // {
        //     var requestMethod = HttpContext.Request.Method;
        //     var requestIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        //     var endpoint = HttpContext.Request.Path;
        //     _logger.LogInformation("Request: {Method} | IP: {Ip} | Endpoint: {Endpoint}", requestMethod, requestIp, endpoint);
        //     try
        //     {
        //         request.Id = id;
        //         var result = await _mediator.Send(request);

        //         _logger.LogInformation("UpdateAstronautDuty result: {Result}", result);
        //         return this.GetResponse(result);
        //     }
        //     catch (NotImplementedException)
        //     {
        //         return this.GetResponse(new BaseResponse()
        //         {
        //             Message = "Not implemented",
        //             Success = false,
        //             ResponseCode = 202
        //         });
        //     }
        //     catch (Exception ex)
        //     {
        //         return this.GetResponse(new BaseResponse()
        //         {
        //             Message = ex.Message,
        //             Success = false,
        //             ResponseCode = (int)HttpStatusCode.InternalServerError
        //         });
        //     }
        // }

        // Delete

        // [HttpDelete("{id:int}")]
        // public async Task<IActionResult> DeleteAstronautDuty(int id)
        // {
        //     var requestMethod = HttpContext.Request.Method;
        //     var requestIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        //     var endpoint = HttpContext.Request.Path;
        //     _logger.LogInformation("Request: {Method} | IP: {Ip} | Endpoint: {Endpoint}", requestMethod, requestIp, endpoint);
        //     try
        //     {
        //         var result = await _mediator.Send(new DeleteAstronautDuty()
        //         {
        //             Id = id
        //         });

        //         _logger.LogInformation("DeleteAstronautDuty result: {Result}", result);
        //         return this.GetResponse(result);
        //     }
        //     catch (NotImplementedException)
        //     {
        //         return this.GetResponse(new BaseResponse()
        //         {
        //             Message = "Not implemented",
        //             Success = false,
        //             ResponseCode = 202
        //         });
        //     }
        //     catch (Exception ex)
        //     {
        //         return this.GetResponse(new BaseResponse()
        //         {
        //             Message = ex.Message,
        //             Success = false,
        //             ResponseCode = (int)HttpStatusCode.InternalServerError
        //         });
        //     }
        // }
    }
}
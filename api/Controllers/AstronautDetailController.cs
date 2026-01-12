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
        private readonly ILogger<AstronautDetailController> _logger;
        
        public AstronautDetailController(IMediator mediator, ILogger<AstronautDetailController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        // Create

        // [HttpPost("")]
        // public async Task<IActionResult> CreateAstronautDetail([FromBody] CreateAstronautDetail request)
        // {
        //     var requestMethod = HttpContext.Request.Method;
        //     var requestIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        //     var endpoint = HttpContext.Request.Path;
        //     _logger.LogInformation("Request: {Method} | IP: {Ip} | Endpoint: {Endpoint}", requestMethod, requestIp, endpoint);
        //     try
        //     {
        //         var result = await _mediator.Send(request);
        //         _logger.LogInformation("CreateAstronautDetail result: {Result}", result);
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

        // Read
        [HttpGet("person/{personId:int}")]
        public async Task<IActionResult> GetAstronautDetailByPersonId(int personId)
        {
            var requestMethod = HttpContext.Request.Method;
            var requestIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var endpoint = HttpContext.Request.Path;
            _logger.LogInformation("Request: {Method} | IP: {Ip} | Endpoint: {Endpoint}", requestMethod, requestIp, endpoint);
            try
            {
                var result = await _mediator.Send(new GetAstronautDetailByPersonId()
                {
                    PersonId = personId
                });

                _logger.LogInformation("GetAstronautDetailByPersonId result: {Result}", result);
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
            var requestMethod = HttpContext.Request.Method;
            var requestIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var endpoint = HttpContext.Request.Path;
            _logger.LogInformation("Request: {Method} | IP: {Ip} | Endpoint: {Endpoint}", requestMethod, requestIp, endpoint);
            try
            {
                var result = await _mediator.Send(new GetAstronautDetailById()
                {
                    Id = id
                });

                _logger.LogInformation("GetAstronautDetailById result: {Result}", result);
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
        // public async Task<IActionResult> UpdateAstronautDetail(int id, [FromBody] UpdateAstronautDetail request)
        // {
        //     var requestMethod = HttpContext.Request.Method;
        //     var requestIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        //     var endpoint = HttpContext.Request.Path;
        //     _logger.LogInformation("Request: {Method} | IP: {Ip} | Endpoint: {Endpoint}", requestMethod, requestIp, endpoint);
        //     try
        //     {
        //         request.Id = id;
        //         var result = await _mediator.Send(request);

        //         _logger.LogInformation("UpdateAstronautDetail result: {Result}", result);
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
        // public async Task<IActionResult> DeleteAstronautDetail(int id)
        // {
        //     var requestMethod = HttpContext.Request.Method;
        //     var requestIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        //     var endpoint = HttpContext.Request.Path;
        //     _logger.LogInformation("Request: {Method} | IP: {Ip} | Endpoint: {Endpoint}", requestMethod, requestIp, endpoint);
        //     try
        //     {
        //         var result = await _mediator.Send(new DeleteAstronautDetail()
        //         {
        //             Id = id
        //         });

        //         _logger.LogInformation("DeleteAstronautDetail result: {Result}", result);
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
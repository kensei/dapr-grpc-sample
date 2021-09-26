using Microsoft.AspNetCore.Mvc;
using DaprSample.Api.Datas.Dto;
using DaprSample.Api.Helpers;
using DaprSample.Api.Models;
using DaprSample.Api.Datas.Enums;

namespace DaprSample.Api.Controllers
{
    [Route("api/dummy")]
    [ApiController]
    public class TestController
    {
        [HttpGet]
        public CommonResponse<Dummy> GetDummy()
        {
            var result = new Dummy { Id = 1, Name = "hoge" };
            return ResponseHelper.GenerateResponse<Dummy>(result, EnumResponseStatus.Success);
        }
    }
}
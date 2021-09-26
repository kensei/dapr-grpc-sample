using Microsoft.AspNetCore.Mvc;
using DaprSample.MicroService.UsersService.Datas.Dto;
using DaprSample.MicroService.UsersService.Datas.Enums;

namespace DaprSample.MicroService.UsersService.Helpers
{
    public class ResponseHelper
    {
        public static JsonResult GenerateResponse<T>(T data, EnumResponseStatus responseStatus)
        {
            var result = new CommonResponse<T>();
            result.ErrorCode = (int)responseStatus;
            result.Data = data;
            return new JsonResult(result);
        }
    }
}

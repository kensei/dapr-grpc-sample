using DaprSample.Api.Datas.Dto;
using DaprSample.Api.Datas.Enums;

namespace DaprSample.Api.Helpers
{
    public class ResponseHelper
    {
        public static CommonResponse<T> GenerateResponse<T>(T data, EnumResponseStatus responseStatus)
        {
            var result = new CommonResponse<T>();
            result.ErrorCode = (int)responseStatus;
            result.Data = data;
            return result;
        }
    }
}
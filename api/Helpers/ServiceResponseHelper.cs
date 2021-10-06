using DaprSample.Api.Datas.Dto;
using Newtonsoft.Json;
using DaprSample.Api.Configs.Exceptions;

namespace DaprSample.Api.Helpers
{
    public class ServiceResponseHelper
    {
        public static CommonResponse<T> DeserializeObject<T>(string json)
        {
            System.Console.WriteLine("result:" + json);
            try {
                var deserializedResponse = JsonConvert.DeserializeObject<CommonResponse<T>>(json);
                return deserializedResponse;
            } catch (JsonException e) {
                try {
                    var errorResponse = JsonConvert.DeserializeObject<CommonResponse<string>>(json);
                    System.Console.WriteLine("errorcode:" + errorResponse.ErrorCode);
                    System.Console.WriteLine("errorcode:" + errorResponse.Data);
                    throw new ServiceCallFailException($"{errorResponse.ErrorCode} : {errorResponse.Data}");
                } catch (JsonException) {
                    throw new ServiceCallFailException(e.Message);
                }
            }
        }
    }
}

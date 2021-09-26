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
                System.Console.WriteLine("errorcode:" + deserializedResponse.ErrorCode);
                return deserializedResponse;
            } catch (JsonException e) {
                try {
                    var errorResponse = JsonConvert.DeserializeObject<CommonResponse<string>>(json);
                    throw new ServiceCallFailException(e.Message);
                } catch (JsonException) {
                    throw new ServiceCallFailException(e.Message);
                }
            }
        }
    }
}

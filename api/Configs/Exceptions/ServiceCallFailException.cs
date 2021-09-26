using System;

namespace DaprSample.Api.Configs.Exceptions
{
    public class ServiceCallFailException : Exception {
        public ServiceCallFailException()
        {
        }
    
        public ServiceCallFailException(string message)
            : base(message)
        {
        }
    
        public ServiceCallFailException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}

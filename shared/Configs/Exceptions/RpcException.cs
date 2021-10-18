using System;

namespace DaprSample.Shared.Configs.Exceptions
{
    public class RpcException : Exception {
        public RpcException()
        {
        }
    
        public RpcException(string message)
            : base(message)
        {
        }
    
        public RpcException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}

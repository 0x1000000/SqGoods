using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;

namespace SqGoods.Services
{
    public interface IServiceResponse
    {
        bool IsSuccessful([NotNullWhen(false)] out ServiceErrorData? errorData);
    }

    public interface IServiceResponse<T>
    {
        bool GetResult([MaybeNullWhen(false)]out T response, [NotNullWhen(false)] out ServiceErrorData? errorData);
    }

    static class ServiceResponse
    {

        public static IServiceResponse<T> Successful<T>(T result)
        {
            return new SuccessfulServiceResponse<T>(result);
        }

        public static IServiceResponse Successful()
        {
            return SuccessfulServiceResponse.Instance;
        }

        public static ServiceErrorData ErrorData(ServiceErrorCode code, string? message)
        {
            return new ServiceErrorData(code, message);
        }

        public static IServiceResponse<T> Error<T>(ServiceErrorCode code, string? message)
        {
            return new ErrorServiceResponse<T>(new ServiceErrorData(code, message));
        }

        public static IServiceResponse<T> Error<T>(ServiceErrorData errorData)
        {
            return new ErrorServiceResponse<T>(errorData);
        }

        public static IServiceResponse Error(ServiceErrorCode code, string? message)
        {
            return new ErrorServiceResponse(new ServiceErrorData(code, message));
        }

        public static IServiceResponse Error(ServiceErrorData errorData)
        {
            return new ErrorServiceResponse(errorData);
        }

        class SuccessfulServiceResponse<T> : IServiceResponse<T>
        {
            private readonly T _value;

            public SuccessfulServiceResponse(T value)
            {
                this._value = value;
            }

            public bool GetResult([MaybeNullWhen(false)] out T response, [NotNullWhen(false)] out ServiceErrorData? errorData)
            {
                response = this._value;
                errorData = null;
                return true;
            }
        }

        class SuccessfulServiceResponse : IServiceResponse
        {
            public static readonly IServiceResponse Instance = new SuccessfulServiceResponse();

            public bool IsSuccessful([NotNullWhen(false)] out ServiceErrorData? errorData)
            {
                errorData = null;
                return true;
            }
        }

        class ErrorServiceResponse<T> : IServiceResponse<T>
        {
            private readonly ServiceErrorData _serviceErrorData;

            public ErrorServiceResponse(ServiceErrorData serviceErrorData)
            {
                this._serviceErrorData = serviceErrorData;
            }

            public bool GetResult([MaybeNullWhen(false)] out T response, [NotNullWhen(false)] out ServiceErrorData? errorData)
            {
                response = default;
                errorData = this._serviceErrorData;
                return false;
            }
        }

        class ErrorServiceResponse : IServiceResponse
        {
            private readonly ServiceErrorData _serviceErrorData;

            public ErrorServiceResponse(ServiceErrorData serviceErrorData)
            {
                this._serviceErrorData = serviceErrorData;
            }

            public bool IsSuccessful([NotNullWhen(false)] out ServiceErrorData? errorData)
            {
                errorData = this._serviceErrorData;
                return false;
            }
        }
    }

    public class ServiceErrorData
    {
        public readonly ServiceErrorCode ServiceErrorCode;

        public readonly string? Message;

        internal ServiceErrorData(ServiceErrorCode serviceErrorCode, string? message)
        {
            this.ServiceErrorCode = serviceErrorCode;
            this.Message = message;
        }
    }

    public static class ServiceErrorDataExt
    {
        public static IActionResult ToActionResult(this ServiceErrorData errorData, ControllerBase controller)
        {
            switch (errorData.ServiceErrorCode)
            {
                case ServiceErrorCode.NotFound:
                    return controller.NotFound(errorData.Message);
                case ServiceErrorCode.BadRequest:
                    return controller.BadRequest(errorData.Message);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public enum ServiceErrorCode
    {
        NotFound,
        BadRequest
    }

}
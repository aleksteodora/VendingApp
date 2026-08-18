using VendingManagement.Shared.Constants;

namespace VendingManagement.Shared.Common
{
    public class ResponsePackage<T>
    {
        public ResponseStatus Status { get; set; } = ResponseStatus.OK;
        public string Message { get; set; }
        public T Data { get; set; } = default;

        public ResponsePackage()
        {
            Status = ResponseStatus.OK;
            Message = string.Empty;
        }

        public ResponsePackage(T data, ResponseStatus status = ResponseStatus.OK, string message = "")
        {
            Data = data;
            Status = status;
            Message = message;
        }

        public ResponsePackage(ResponseStatus status, string message)
        {
            Status = status;
            Message = message;
        }
    }

    public class ResponsePackageNoData
    {
        public ResponseStatus Status { get; set; } = ResponseStatus.OK;
        public string Message { get; set; }
        public int? ErrorCode { get; set; }

        public ResponsePackageNoData()
        {
            Status = ResponseStatus.OK;
            Message = string.Empty;
        }

        public ResponsePackageNoData(ResponseStatus status, string message)
        {
            Status = status;
            Message = message;
        }
    }
}

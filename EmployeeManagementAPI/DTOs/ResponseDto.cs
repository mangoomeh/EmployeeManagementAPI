namespace EmployeeManagementAPI.DTOs
{
    public class ResponseDto<T>
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public List<T> Result { get; set; }
    }
}

namespace DatingApp.Api.Errors
{
    public class ApiResponse
    {
        public int StatusCodes { get; set; }

        public string Message { get; set; }


        public ApiResponse(int statusCode, string message = null)
        {
            StatusCodes = statusCode;

            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
        }


        private string GetDefaultMessageForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "A Problem was occured, please try again",
                401 => "No Access",
                404 => "Page Not Found",
                _ => "A Problem was occured, Error was not found"
            };
        }
    }
}

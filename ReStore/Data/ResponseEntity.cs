namespace ReStore.Data
{
    public class ResponseEntity<T> where T : class
    {
        public ResponseEntity(bool success, T data, string message)
        {
            Success = success;
            Data = data;
            Message = message;
        }

        public bool Success { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }
    }
}

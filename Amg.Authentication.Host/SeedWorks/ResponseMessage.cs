namespace Amg.Authentication.Host.SeedWorks
{
    public class ResponseMessage
    {
        public ResponseMessage()
        {
        }

        public ResponseMessage(string message)
        {
            Message = message;
            Content = null;
        }

        public string Message { get; set; }
        public object Content { get; set; }

    }

    public class ResponseMessage<T> : ResponseMessage
    {
        public ResponseMessage()
        {
        }

        public ResponseMessage(string message, T content)
        {
            Message = message;
            Content = content;
        }

        public T Content { get; set; }

    }
}

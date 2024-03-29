﻿namespace SampleWebApp.Dtos
{
    public class ResponseMessage
    {
        public ResponseMessage()
        {
        }

        public ResponseMessage(string message
        )
        {
            Message = message;
        }

        public string Message { get; set; }

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

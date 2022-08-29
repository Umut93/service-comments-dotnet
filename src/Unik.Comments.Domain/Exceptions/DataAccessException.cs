using System;
using System.Runtime.Serialization;

namespace Unik.Comments.Domain.Exceptions;

[Serializable]
public class DataAccessException : Exception
{
    public DataAccessException() : base()
    {
    }

    public DataAccessException(string? message) : base(message)
    {
    }

    public DataAccessException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected DataAccessException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
    {
    }
}
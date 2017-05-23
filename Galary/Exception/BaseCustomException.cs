using System;

public abstract class BaseCustomException : Exception
{
    public BaseCustomException(string message)
        : base(message)
    {
    }

    public BaseCustomException(string message, object arg0)
        : base(String.Format(message, arg0))
    {

    }
    public BaseCustomException(string message, params object[] args)
        : base(String.Format(message, args))
    {

    }
    public BaseCustomException(string message, object arg0, object arg1)
        : base(String.Format(message, arg0, arg1))
    {

    }
    public BaseCustomException(string message, object arg0, object arg1, object arg2)
        : base(String.Format(message, arg0, arg1, arg2))
    {

    }
}

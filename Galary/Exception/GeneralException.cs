
public class GeneralException : BaseCustomException
{
    public GeneralException(string message) : base(message) { }
    public GeneralException(string message, object arg0) : base(message, arg0) { }
    public GeneralException(string message, params object[] args) : base(message, args) { }
    public GeneralException(string message, object arg0, object arg1) : base(message, arg0, arg1) { }
    public GeneralException(string message, object arg0, object arg1, object arg2) : base(message, arg0, arg1, arg2) { }
}
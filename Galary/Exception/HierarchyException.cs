using System;

public class HierarchyException : BaseCustomException
{
    public HierarchyException(string message) : base(message) { }
    public HierarchyException(string message, object arg0) : base(message, arg0) { }
    public HierarchyException(string message, params object[] args) : base(message, args) { }
    public HierarchyException(string message, object arg0, object arg1) : base(message, arg0, arg1) { }
    public HierarchyException(string message, object arg0, object arg1, object arg2) : base(message, arg0, arg1, arg2) { }
}
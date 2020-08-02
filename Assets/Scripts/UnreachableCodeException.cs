class UnreachableCodeException : System.Exception
{
    public UnreachableCodeException()
        : base("This code path shouldn't have been reached.") { }
}

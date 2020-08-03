class MultipleInstancesException : System.Exception
{
    public MultipleInstancesException()
        : base("This class should have only one instance on the scene.") { }
}
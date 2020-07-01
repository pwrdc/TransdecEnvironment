class InvalidEnumValueException : System.Exception {
    public InvalidEnumValueException(System.Enum value) : base($"Invalid value \"{value}\" was placed in enum variable of type {value.GetType()}.") { }
}
using System;
using System.Reflection;

[AttributeUsage(AttributeTargets.Field)]
public class ResetParameterAttribute : Attribute {
    // For now it is used to mark fields that are set from resetParameters
    // I have a ready to use script that will update this fields automatically using reflection
    // it just needs some adjustments
}
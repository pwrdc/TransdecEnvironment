using System;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// Attribute that simplyfies working with ML-Agents Academy reset parameters.
/// It replaces error-prone synchronizing functions with one call to a static function
/// that searches through the fields and connects marked ones to the academy reset parameters.
/// </summary>
/// <example>
/// This example illustrates all usage cases and practices.
/// <code>
/// class Example {
///     // assigning default values to variables 
///     // will prevent "variable never assigned" warnings
///     
///     // basic example
///     // this field will be synchronized with 
///     // "CollectData" in the academy reset parameters
///     [ResetParameter] bool collectData = false;
///     
///     // reset parameter name different than the variable name
///     [ResetParameter("CollectData")]
///     bool collectingData = false;
///     
///     // different variable types
///     [ResetParameter] float speed = 0;
///     [ResetParameter] int count = 0;
///     [ResetParameter] bool valid = 0;
///     public enum Side
///     {
///         Left,
///         Right
///     }
///     [ResetParameter] Side side = Side.Left;
///     
///     void Start()
///     {
///         // IMPORTANT: without this call fields will be unassigned
///         // it connects the class instance to the academy
///         ResetParameterAttribute.InitializeAll(this);
///     }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Field)]
public class ResetParameterAttribute : Attribute {
    // if you want to field under name different than field name set this field
    public string name;
    // works only when the field above isn't set
    public bool capitalize;

    public ResetParameterAttribute(string name=null, bool capitalize = true)
    {
        this.name = name;
        this.capitalize = capitalize;
    }

    string Capitalize(string str)
    {
        if (str.Length == 0)
            return "";
        else
            return char.ToUpper(str[0]) + str.Substring(1);
    }

    public void Assign(object obj, FieldInfo field)
    {
        RobotAcademy academy = RobotAcademy.Instance;
        if (academy == null)
        {
            Debug.LogError("There is no RobotAcademy on the scene but there is an object with ResetParameterAttribute that needs it.");
            return;
        }
        try
        {
            if(name==null)
                name = capitalize ? Capitalize(field.Name) : field.Name;
            float resetParameter = academy.GetResetParameter(name);
            Type fieldType = field.FieldType;
            if (fieldType == typeof(float))
            {
                field.SetValue(obj, resetParameter);
            }
            else if (fieldType == typeof(int))
            {
                field.SetValue(obj, (int)resetParameter);
            }
            else if (fieldType.IsEnum)
            {
                field.SetValue(obj, (int)resetParameter);
            }
            else if (fieldType == typeof(bool))
            {
                // bool field is false only if resetParameter is zero
                field.SetValue(obj, resetParameter != 0.0);
            }
            else
            {
                Debug.LogError($"ResetParameterAttribute: type {fieldType} is not supported.");
                return;
            }
        }
        catch (KeyNotFoundException)
        {
            Debug.LogError($"ResetParameterAttribute name {name} doesn't exist in reset parameters dictionary.");
            return;
        }
    }

    struct AttributeAndField
    {
        public ResetParameterAttribute attribute;
        public FieldInfo field;
    }
    static Dictionary<Type, List<AttributeAndField>> cached = new Dictionary<Type, List<AttributeAndField>>();
    
    /// <summary>
    /// Enables all ResetParameter attributes in the object.
    /// Without this call they have no effect.
    /// </summary>
    public static void InitializeAll(object obj)
    {
        RobotAcademy academy = RobotAcademy.Instance;
        if (academy == null)
        {
            Debug.LogError("There is no RobotAcademy on the scene but there is an object with ResetParameterAttribute that needs it.");
            return;
        }
        // update the object whenever reset parametrs change
        academy.onResetParametersChanged.AddListener(() => AssignAll(obj));
    }

    private static void AssignAll(object obj)
    {
        Type type = obj.GetType();
        if (cached.ContainsKey(type))
        {
            List<AttributeAndField> typeCache = cached[type];
            foreach (AttributeAndField attributeAndField in typeCache)
            {
                attributeAndField.attribute.Assign(obj, attributeAndField.field);
            }
        }
        else
        {
            List<AttributeAndField> typeCache = new List<AttributeAndField>();
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            foreach (FieldInfo field in fields)
            {
                foreach (ResetParameterAttribute attribute in field.GetCustomAttributes<ResetParameterAttribute>())
                {
                    attribute.Assign(obj, field);
                    typeCache.Add(new AttributeAndField { attribute = attribute, field = field });
                }
            }
            cached.Add(type, typeCache);
        }
    }
}
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Events;

/*
Usage example:

class MyClass {
    [ResetParameter] bool collectData;
    [ResetParameter("CollectData")] 
    bool collectingData;
    [ResetParameter] float speed;

    void Start(){
        // without this call fields will be unassigned
        ResetParameterAttribute.InitializeAll(this);
    }
}
*/

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

    public void Assign(System.Object obj, FieldInfo field)
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
        catch (KeyNotFoundException e)
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

    public static void InitializeAll(System.Object obj)
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

    static void AssignAll(System.Object obj)
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
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
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
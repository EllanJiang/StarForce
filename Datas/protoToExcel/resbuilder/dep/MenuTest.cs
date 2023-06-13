using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//指定特性可以应用于哪些类型上
[AttributeUsage(AttributeTargets.All, AllowMultiple =false)]
public sealed class ConfigAttribute : Attribute
{
    public ConfigAttribute(string desc)
    {
        Desc = desc;
    }

    public string Desc { get; set; }
}


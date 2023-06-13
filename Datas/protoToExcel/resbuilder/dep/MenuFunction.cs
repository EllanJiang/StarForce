using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using D11;

public class MenuFunction
{
    [MenuItem("Window/GenAllConfigProto")]
    private static void DeleteValidate()
    {
        ConfigClassGen configClassGen = new ConfigClassGen();
        configClassGen.GenerateAllConfigClass(0);
    }
}

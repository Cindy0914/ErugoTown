using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute(menuName = "ScriptableObject/")]
public class ScriptableObjectBase<T> : ScriptableObject where T : ScriptableObjectBase<T>
{

}

#region Guide
/*
[CreateAssetMenuAttribute(menuName = "ScriptableObject/Test")]
public class Test : ScriptableObjectBase<Test>
{
    public string test;
}
*/
#endregion
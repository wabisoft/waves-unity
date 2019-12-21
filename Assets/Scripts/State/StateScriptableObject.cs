using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateScriptableObject<TEnum> : ScriptableObject
    where TEnum : System.Enum
{
    public TEnum id; 
}



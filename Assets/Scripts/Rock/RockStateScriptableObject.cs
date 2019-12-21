using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RockState", menuName = "ScriptableObjects/Rock/State", order = 1)]
public class RockStateScriptableObject : StateScriptableObject<RockStateEnum>
{
    public float linearDrag;
    public float angularDrag;
    public float gravityScale;
    [Range(0.05f, 1)]
    public float throwForce;
}

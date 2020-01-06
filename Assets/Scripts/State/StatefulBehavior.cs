using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStatefulBehavior<TEnum, TScriptableObject>
    where TEnum : System.Enum
    where TScriptableObject : StateScriptableObject<TEnum>
{
    TScriptableObject GetStateSO(TEnum Id);
    IState<TEnum> CurrentState { get; }
    void EnterState(IState<TEnum> state);
    void ExitState();
}


public abstract class StatefulBehavior<TEnum, TScriptableObject> : 
    MonoBehaviour, 
    IStatefulBehavior<TEnum, TScriptableObject>
    where TEnum : System.Enum
    where TScriptableObject : StateScriptableObject<TEnum> 
{
    protected Dictionary<TEnum, TScriptableObject> stateSOLookup_ = new Dictionary<TEnum, TScriptableObject>();
    [Expandable]
    public List<TScriptableObject> StateValues = new List<TScriptableObject>();
    protected Stack<IState<TEnum>> states_ = new Stack<IState<TEnum>>();
    public string CurrentStateName;
    public IState<TEnum> CurrentState { get { return states_.Peek(); } }
    public TScriptableObject GetStateSO(TEnum id)
    {
        return stateSOLookup_.TryGetValue(id, out TScriptableObject value) ? value : null;
    }

    public void EnterState(IState<TEnum> state)
    {
        if(states_.Count > 0)
        {
            CurrentState.Exit();
        }
        state.Enter();
        states_.Push(state);
        CurrentStateName = CurrentState.Id.ToString();
    }

    public void ExitState()
    {
        if(states_.Count == 0) { return; }
        states_.Pop().Exit();
        if(states_.Count == 0) { return; }
        states_.Peek()?.Enter();
        CurrentStateName = CurrentState.Id.ToString();
    }

    public virtual void Awake()
    {
        foreach(var stateValue in StateValues)
        {
            stateSOLookup_[stateValue.id] = stateValue;
        }
    }

    public virtual void Update() { CurrentState.Update(); }
    public virtual void FixedUpdate() { CurrentState.FixedUpdate();  }
    public virtual void OnMouseDown() { CurrentState.OnMouseDown(); }
    public virtual void OnMouseDrag() { CurrentState.OnMouseDrag(); }
    public virtual void OnMouseUp() { CurrentState.OnMouseUp(); }
    public virtual void OnCollisionEnter2D(Collision2D collision) { CurrentState.OnCollisionEnter2D(collision); }
    public virtual void OnCollisionStay2D(Collision2D collision) { CurrentState.OnCollisionStay2D(collision); }
    public virtual void OnCollisionExit2D(Collision2D collision) { CurrentState.OnCollisionExit2D(collision); }
    public virtual void OnTriggerEnter2D(Collider2D collider) { CurrentState.OnTriggerEnter2D(collider); }
    public virtual void OnTriggerStay2D(Collider2D collider) { CurrentState.OnTriggerStay2D(collider); }
    public virtual void OnTriggerExit2D(Collider2D collider) { CurrentState.OnTriggerExit2D(collider); }
}

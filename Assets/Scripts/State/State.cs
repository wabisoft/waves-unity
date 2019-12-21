using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState<TEnum>
{
    TEnum Id { get; }
    void Enter();
    void Exit();
    void Update();
    void FixedUpdate();
    void OnMouseDown();
    void OnMouseDrag();
    void OnMouseUp();
    void OnCollisionEnter2D(Collision2D collision);
    void OnCollisionStay2D(Collision2D collision);
    void OnCollisionExit2D(Collision2D collision);
    void OnTriggerEnter2D(Collider2D collider);
    void OnTriggerStay2D(Collider2D collider);
    void OnTriggerExit2D(Collider2D collider);
}

public abstract class State<TEnum, TIBehavior, TScriptableObject> : IState<TEnum>
    where TEnum : System.Enum
    where TIBehavior : IStatefulBehavior<TEnum, TScriptableObject>
    where TScriptableObject : StateScriptableObject<TEnum>
{
    protected TIBehavior behavior;
    protected TScriptableObject so;

    public abstract TEnum Id { get; }

    public State(TIBehavior b) {
        behavior = b;
        so = b.GetStateSO(Id);
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void OnMouseDown() { }
    public virtual void OnMouseDrag() { }
    public virtual void OnMouseUp() { }
    public virtual void OnCollisionEnter2D(Collision2D collision) { }
    public virtual void OnCollisionStay2D(Collision2D collision) { }
    public virtual void OnCollisionExit2D(Collision2D collision) { }
    public virtual void OnTriggerEnter2D(Collider2D collider) { }
    public virtual void OnTriggerStay2D(Collider2D collider) { }
    public virtual void OnTriggerExit2D(Collider2D collider) { }
}

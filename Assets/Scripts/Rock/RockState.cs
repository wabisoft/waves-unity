using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RockStateEnum
{
    Default,
    Caught,
    Sinking,
    Sunk
}

public abstract class RockState : State<RockStateEnum, RockBehavior, RockStateScriptableObject>
{
    public RockState(RockBehavior rb) : base(rb) { }

    protected float throwStartTime;
    protected float throwEndTime;
    protected Vector2 throwStartPosition;
    protected Vector2 throwEndPosition;


    public override void Enter() {
        base.Enter();
        behavior.rigidbody.drag = so.linearDrag;
        behavior.rigidbody.angularDrag = so.angularDrag;
        behavior.rigidbody.gravityScale = so.gravityScale;
    }

    public override void OnMouseDown() {
        base.OnMouseDown();
        throwStartTime = Time.time;
        throwStartPosition = Input.mousePosition;
    }

    public override void OnMouseUp() {
        base.OnMouseUp();
        throwEndTime = Time.time;
		var throwTimeInterval = throwEndTime - throwStartTime;
		throwEndPosition = Input.mousePosition;
		var direction = throwEndPosition - throwStartPosition;
		behavior.rigidbody.AddForce((direction / throwTimeInterval) * so.throwForce);
    }


    public virtual void CollideSea(SeaBehavior sb) { }
    public virtual void UnCollideSea(SeaBehavior sb) { }
    public virtual void CollideFloor(GameObject floor) { }
}

public class DefaultRockState: RockState
{
    public override RockStateEnum Id { get { return RockStateEnum.Default; } }

    public DefaultRockState(RockBehavior rb) : base(rb) { }

    public override void OnMouseDown()
    {
        base.OnMouseDown();
        // behavior.EnterState(new CaughtRockState(behavior));
    }
    public override void CollideSea(SeaBehavior seaBehavior)
    {
        seaBehavior.CreateWave(seaBehavior.transform.InverseTransformPoint(behavior.transform.position),
            behavior.rigidbody.velocity.magnitude, (int)Mathf.Sign(behavior.rigidbody.velocity.x), (behavior.isNegative)? -1 : 1);
        behavior.EnterState(new SinkingRockState(behavior, seaBehavior));
    }
}

public class CaughtRockState : RockState
{
    public override RockStateEnum Id { get { return RockStateEnum.Caught;  } }

    public CaughtRockState(RockBehavior rb) : base(rb) { }


    public override void Enter()
    {
        base.Enter();
        behavior.rigidbody.velocity = Vector2.zero;
        behavior.rigidbody.angularVelocity = 0.0f;
    }
    public override void Exit()
    {
        base.Exit();
    }

    public override void OnMouseUp()
    {
        behavior.ExitState();
        base.OnMouseUp();
    }
}

public class SinkingRockState : RockState
{
    public override RockStateEnum Id { get { return RockStateEnum.Sinking; } }

    public SeaBehavior seaBehavior;

    public SinkingRockState(RockBehavior rb, SeaBehavior sb) : base(rb) {
        seaBehavior = sb;
    }

    public override void FixedUpdate()
    {
        var waterLevel = seaBehavior.WorldHeightAtWorldPosition(behavior.transform.position);
        if(behavior.Lower().y < waterLevel) { return; }
        UnCollideSea(seaBehavior);
    }

    public override void UnCollideSea(SeaBehavior sb)
    {
        behavior.ExitState();
    }
    public override void CollideFloor(GameObject floor)
    {
        behavior.ExitState();
        behavior.EnterState(new SunkRockState(behavior));
    }

}

public class SunkRockState : RockState
{
    public SunkRockState(RockBehavior rb) : base(rb) { }
    public override RockStateEnum Id { get { return RockStateEnum.Sunk; } }
    public override void FixedUpdate() { }
    public override void UnCollideSea(SeaBehavior sb) { }
}



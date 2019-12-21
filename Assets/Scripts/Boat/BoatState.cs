using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoatStateEnum
{
    Default,
    Sailing,
    Surfing,
}

public abstract class BoatState : State<BoatStateEnum, BoatBehavior, BoatStateScriptableObject>
{
    public BoatState(BoatBehavior bb) : base(bb) { }

	public virtual void CollideSea(SeaBehavior sb)
    {
        var waterLevel = sb.WorldHeightAtWorldPosition(behavior.transform.position);
        var lower = behavior.transform.position - (Vector3)behavior.halfSize;
        if (lower.y > waterLevel) { return; }
        var h = Mathf.Min(waterLevel - lower.y, behavior.collider.size.y);
        var w = behavior.collider.size.x;
        var displacedWater = h * w;
        var rigidbody = behavior.rigidbody;
        if (Vector3.Dot(rigidbody.velocity, Vector3.up) != 0)
        {
            var waterFluidDensity = 50f;
            var bouyancy = displacedWater * waterFluidDensity * -Physics2D.gravity * Time.deltaTime;
            rigidbody.AddForce(bouyancy, ForceMode2D.Impulse);
            rigidbody.velocity *= 0.75f;
        }
    }

    public virtual void CollidePlatform(PlatformBehavior pb) { }
    public virtual void UnCollidePlatform(PlatformBehavior pb) { }

}

public class DefaultBoatState : BoatState
{
    public override BoatStateEnum Id { get { return BoatStateEnum.Default; } }

    public DefaultBoatState(BoatBehavior bb) : base(bb) { } 

    public override void CollideSea(SeaBehavior sb)
    {
        base.CollideSea(sb);
        behavior.EnterState(new SailingBoatState(behavior, sb));
    }
}

public class SailingBoatState : BoatState
{
    public SeaBehavior seaBehavior;

    public override BoatStateEnum Id { get { return BoatStateEnum.Sailing; } }

    public SailingBoatState(BoatBehavior bb, SeaBehavior sb) : base(bb)
    {
        seaBehavior = sb;
    }

    public override void CollideSea(SeaBehavior sb)
    {
        base.CollideSea(sb);
        var posRelativeToSea = sb.transform.InverseTransformPoint(behavior.transform.position);
        var seaVel = sb.VelocityAtX(posRelativeToSea.x);
        behavior.rigidbody.velocity += seaVel;
        var distFromMax = Mathf.Abs(posRelativeToSea.x - sb.posOfMaxHeight.x);
        if (distFromMax <= 0.5f && sb.maxHeightWave != null)
        {
            behavior.ExitState();
            behavior.EnterState(new SurfinBoatState(behavior, sb, sb.maxHeightWave));
        }
    } 
}

public class SurfinBoatState : BoatState
{
	SeaBehavior seaBehavior;
	Wave wave;

    public override BoatStateEnum Id { get { return BoatStateEnum.Surfing;  } }

	public SurfinBoatState(BoatBehavior bb, SeaBehavior sb, Wave w) : base(bb) { seaBehavior = sb; wave = w; }

	public override void FixedUpdate()
	{
		if (wave.killme)
		{
			behavior.ExitState();
			return;
		}
		var posRelativeToSea = seaBehavior.transform.InverseTransformPoint(behavior.transform.position);
        var diff = posRelativeToSea - (Vector3)wave.position;
		if (Mathf.Abs(diff.x) > 0.5)
		{
			behavior.ExitState();
			return;
		}
		var heightOfWave = seaBehavior.LocalHeightAtLocalPosition(wave.position);
		var newPos = new Vector3(wave.position.x, heightOfWave+behavior.halfSize.y, 0);
		var relPos = newPos - posRelativeToSea;
        // behavior.rigidbody.velocity = new Vector2(wave.velocity.x, behavior.rigidbody.velocity.y);
        behavior.rigidbody.MovePosition(behavior.transform.position + relPos);
    }

    public override void CollideSea(SeaBehavior sb)
    {
        base.CollideSea(sb);
        var posRelativeToSea = sb.transform.InverseTransformPoint(behavior.transform.position);
        var seaVel = sb.VelocityAtX(posRelativeToSea.x);
        behavior.rigidbody.velocity += seaVel * 5;
    }
}

public class RestingBoatState : DefaultBoatState
{
    public RestingBoatState(BoatBehavior bb) : base(bb) { }

}

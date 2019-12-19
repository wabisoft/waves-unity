using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBehavior : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collision.gameObject.GetComponent<BoatBehavior>()?.CollidePlatform(this);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        collision.gameObject.GetComponent<BoatBehavior>()?.CollidePlatform(this);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        collision.gameObject.GetComponent<BoatBehavior>()?.UnCollidePlatform(this);
    }

}

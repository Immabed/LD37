using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCollision : MonoBehaviour {

    public PlayerMovement pm;
    public Direction dir;

   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Collidable")
            pm.Collision(dir, true);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Collidable")
            pm.Collision(dir, false);
    }
}

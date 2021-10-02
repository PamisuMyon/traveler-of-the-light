using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathWall : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.tag == "Player" && !other.isTrigger)    
        {
            other.GetComponent<PlayerMovement>().Die();
        }
    }
}

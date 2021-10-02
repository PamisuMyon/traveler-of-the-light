using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerArea2D : MonoBehaviour
{

    public event System.Action<Collider2D> TriggerEnter2D;
    public event System.Action<Collider2D> TriggerStay2D;
    public event System.Action<Collider2D> TriggerExit2D;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        TriggerEnter2D?.Invoke(other);
    }

    private void OnTriggerStay2D(Collider2D other) 
    {
        TriggerStay2D?.Invoke(other);
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        TriggerExit2D?.Invoke(other);
    }
}

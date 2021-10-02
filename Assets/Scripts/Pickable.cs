using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickable : MonoBehaviour
{

    bool picked = false;

    AudioSource audioSource;

    private void Start() 
    {
        audioSource = GetComponent<AudioSource>();    
    }

    public void OnPickUpAnimFinished()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.tag == "Player" && !other.isTrigger)    
        {
            PickUp();
        }
    }

    void PickUp()
    {
        if (picked) return;
        picked = true;
        Door.Instance.KeyPicked();
        GetComponent<Animator>().SetBool("IsPicked", true);
        Utils.PlayRandomPitch(audioSource);
    }
}

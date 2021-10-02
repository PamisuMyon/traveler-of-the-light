using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Door : MonoBehaviour
{
    public static Door Instance;

    public bool isOpen;

    Animator animator;
    Text keyNumber;

    float total;
    float current;

    void Start()
    {
        Instance = this;

        animator = GetComponent<Animator>();
        keyNumber = GetComponentInChildren<Text>();
        total = GameObject.FindGameObjectsWithTag("Key").Length;
        current = 0;

        if (keyNumber != null)
            keyNumber.text = current + "/" + total;
        Toggle(isOpen);
    }

    public void KeyPicked()
    {
        current++;
        keyNumber.text = current + "/" + total;
        if (current == total)
            Toggle(this.isOpen = true);
    }

    void Toggle(bool isOpen)
    {
        animator.SetBool("IsOpen", isOpen);
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (!isOpen) return;
        if (other.tag == "Player" && !other.isTrigger)
        {
            other.GetComponent<PlayerMovement>().Transport();
        }
    }
    
}

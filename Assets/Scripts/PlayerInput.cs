using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public bool jump;
    public bool jumpHeld;
    public float horizontal;
    public bool interact;
    public bool travel;

    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Jump"))
            jump = true;
        jumpHeld = Input.GetButton("Jump");

        if (Input.GetButtonDown("Interact"))
            interact = true;

        if (Input.GetButtonDown("Travel"))
            travel = true;
    }
}

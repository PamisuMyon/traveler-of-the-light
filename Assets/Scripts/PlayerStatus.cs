using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{

    public float lpMax;
    public float lpDecrement;
    public float lpIncrement;

    float lp;
    List<GameObject> lights;

    PlayerMovement playerMovement;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();

        lights = new List<GameObject>();
        lp = lpMax;
    }

    void Update()
    {
        if (lights.Count > 0)
        {
            lp += lpIncrement * Time.deltaTime;
            lp = Mathf.Min(lpMax, lp);
        }
        else
        {
            lp -= lpDecrement * Time.deltaTime;
        }
        if (lp <= 0)
            playerMovement.Die();

        UpdateLpSlider();
    }

    void UpdateLpSlider()
    {
        UiManager.Instance.lpSlider.maxValue = lpMax;
        UiManager.Instance.lpSlider.value = lp;
    }

    public void TakeDamage(float value, Vector3 force)
    {
        if (playerMovement.state == PlayerMovement.State.Traveling) return;
        lp -= value;
        UpdateLpSlider();
        playerMovement.TakeDamage(force);
        if (lp <= 0)
            playerMovement.Die();
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.tag == "Light")
        {
            if (!lights.Contains(other.gameObject))
            {
                lights.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.tag == "Light")
        {
            if (lights.Contains(other.gameObject))
            {
                lights.Remove(other.gameObject);
            }
        } 
    }
}

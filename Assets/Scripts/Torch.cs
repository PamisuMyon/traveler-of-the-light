using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;


public class Torch : MonoBehaviour
{

    public Light2D pointLight;
    public Collider2D pointLightCollider;
    public ParticleSystem[] particles;

    [Space]
    public bool isOn;
    public float innerRadiusOff;
    public float outerRadiusOff;
    public float innerRadiusOn;
    public float outerRadiusOn;

    Animator animator;
    GameObject hint;
    GameObject keyToggle;
    GameObject keyTravel;
    
    GameObject nearestTorch;
    public GameObject NearestTorch { get { return nearestTorch; } }

    void Start()
    {
        animator = GetComponent<Animator>();    
        hint = transform.Find("Canvas").gameObject;
        keyToggle = transform.Find("Canvas/KeyToggle").gameObject;
        keyTravel = transform.Find("Canvas/KeyTravel").gameObject;

        HideHint();
        Toggle(isOn);
    }

    public void Toggle()
    {
        Toggle(!this.isOn);
    }

    public void Toggle(bool isOn)
    {
        this.isOn = isOn;
        if (animator != null)
            animator.SetBool("IsOn", isOn);
        if (isOn)
        {
            pointLight.pointLightInnerRadius = innerRadiusOn;
            pointLight.pointLightOuterRadius = outerRadiusOn;
            pointLightCollider.enabled = true;
            foreach (var item in particles)
            {
                item.Play();
            }
        }
        else
        {
            pointLight.pointLightInnerRadius = innerRadiusOff;
            pointLight.pointLightOuterRadius = outerRadiusOff;
            pointLightCollider.enabled = false;
            foreach (var item in particles)
            {
                item.Stop();
            }
        }
    }

    void OnValidate() 
    {
        Toggle(this.isOn);
    }

    public void ShowHint()
    {
        hint.SetActive(true);
        keyToggle.SetActive(true);
        if (isOn)
        {
            nearestTorch = FindNearestTorch();
            if (nearestTorch != null)
                keyTravel.SetActive(true);
            else
                keyTravel.SetActive(false);
        }
        else
        {
            keyTravel.SetActive(false);
        }
    }

    public void HideHint()
    {
        hint.SetActive(false);
        nearestTorch = null;
    }

    GameObject FindNearestTorch()
    {
        var torches = GameObject.FindGameObjectsWithTag("Torch");
        if (torches.Length > 0)
        {
            float distance = float.PositiveInfinity;
            GameObject nearest = null;
            foreach (var item in torches)
            {
                if (item != this.gameObject && item.GetComponent<Torch>().isOn)
                {
                    var dis = (item.transform.position - transform.position).sqrMagnitude;
                    if (dis < distance)
                    {
                        distance = dis;
                        nearest = item;
                    }
                }
            }
            return nearest;
        }
        return null;
    }
    
}

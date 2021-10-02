using UnityEngine;

public class Sign : MonoBehaviour 
{

    public GameObject hide;
    public GameObject hint;

    private void Start() 
    {
        hint.SetActive(false);    
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.tag == "Player" && !other.isTrigger)
        {
            hint.SetActive(true);
            if (hide)
                hide.SetActive(false);
        }
    }
    
    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.tag == "Player" && !other.isTrigger)
        {
            hint.SetActive(false);
            if (hide)
                hide.SetActive(true);
        }
    }
}
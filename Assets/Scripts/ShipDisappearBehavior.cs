using UnityEngine;

public class ShipDisappearBehavior : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject GetGameObject;
    void Start()
    {
        // ship set to disappear from screen after 30 sec 
        //Destroy(gameObject, 30);
        GetGameObject.SetActive(true);
        Invoke("DeactivateObject", 30f);
    }
    void DeactivateObject()
    {
        GetGameObject.SetActive(false); // deactivate the object
    }
    // U
    // Update is called once per frame
    void Update()
    {
        
    }
}

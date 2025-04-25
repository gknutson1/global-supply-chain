using UnityEngine;

public class SubAppear : MonoBehaviour
{
    public GameObject GetGameObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetGameObject.SetActive(false);
        Invoke("ActivateObject", 60f);
    }
    void ActivateObject()
    {
        GetGameObject.SetActive(true); // Activate the object
    }
    // Update is called once per frame
    void Update()
    {

    }
}

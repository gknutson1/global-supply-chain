using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [SerializeField] private LayerMask shipLayer;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            List<RaycastHit2D> data = new List<RaycastHit2D>();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.Log(ray);

            var filter = new ContactFilter2D();
            filter.layerMask = shipLayer;
            
            if (0 != Physics2D.Raycast(new Vector2(ray.origin.x, ray.origin.y), Vector2.down, filter, data)) {
                PlayerShip target = data[0].collider.gameObject.GetComponent<PlayerShip>();
                target.Select();
            }
            Debug.Log(data);
        }
    }
}
using UnityEngine;

public class GameManager : MonoBehaviour {

    // Layer that selectable ships live on
    [SerializeField] private LayerMask shipLayer;

    // The collider that is used to detect when the user has selected a ship
    private GameObject _selector;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        _camera = Camera.main; // for performance reasons
        
        // Grab the selector object, and make sure it is disabled and has a zeroed transform
        _selector = GameObject.Find("SelectorBox");
        _selector.SetActive(false);
        _selector.transform.localScale = Vector3.zero;
    }
    
    // Get position of the mouse as a unity coordinate (zeroed out)
    private Vector3 MousePosition() {
        Vector3 cam = _camera!.ScreenToWorldPoint(Input.mousePosition);
        cam.z = 0;
        return cam;
    }
    
    private bool _selecting = false;
    private Vector3 _originPos;
    private Camera _camera;

    // Update is called once per frame
    void Update()
    {
        // Enter into selection mode when user clicks LMB
        if (Input.GetMouseButtonDown(0)) {
            _selector.SetActive(true);
            _selector.transform.position = _originPos = MousePosition();
            _selecting = true;
        } 
        // Finalize selection when user lets go of LMB
        else if (Input.GetMouseButtonUp(0)) {
            // Reset the selector
            _selecting = false;
            _selector.SetActive(false);
            _selector.transform.localScale = Vector3.zero;
        } 
        // Move the selector box as the user moves the mouse around
        else if (_selecting) {
            Vector3 mousePos = MousePosition();
            _selector.transform.localScale = _originPos - mousePos;
            // Position the selector directly in the middle between origin and mouse
            _selector.transform.position = ((mousePos - _originPos) / 2) + _originPos;
        }
    }
}
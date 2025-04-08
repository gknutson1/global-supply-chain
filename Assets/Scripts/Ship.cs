using UnityEngine;

public class Ship : MonoBehaviour {
    public Color shipColor;
    public Sprite shipSprite;
    
    private LineRenderer _selectionRing;
    
    private bool _selected = false;

    private bool Selected {
        get => _selected;
        set => SetSelected(value);
    }

    private void SetSelected(bool state) {
        // If the new state is the same as the old, return early
        if (state == Selected) return;
        
        // If selected -> deselected
        if (Selected) _selectionRing.enabled = false;
        
        // If deselected -> selected
        else _selectionRing.enabled = true;
        
        _selected = state; // Actually write the new value
    }

    void Start() {
        gameObject.GetComponent<SpriteRenderer>().sprite = shipSprite;

        // Get the height and width of the ship in unity units
        float scaleX = shipSprite.texture.width / shipSprite.pixelsPerUnit;
        float scaleY = shipSprite.texture.height / shipSprite.pixelsPerUnit;
        
        // Set the collider to the correct size
        gameObject.GetComponent<CapsuleCollider2D>().size = new Vector2(scaleX, scaleY);
        
        // Get the selector ring object and init it's circle
        GameObject selector = gameObject.transform.Find("SelectionRing").gameObject;
        _selectionRing = selector.GetComponent<LineRenderer>();
        DrawCircle(_selectionRing, Vector3.zero, 0.5f, 0.3f);
        
        // Set selector ring's size, color, and disable it
        selector.transform.localScale *= Mathf.Max(scaleX, scaleY) * 1.2f;
        _selectionRing.startColor = _selectionRing.endColor = shipColor;
        _selectionRing.enabled = false;
    }

    private void DrawCircle(LineRenderer line, Vector3 pos, float radius, float thickness = 0.02f, int segments = 32) {
        Vector3[] points = new Vector3[segments]; // Array of LineRenderer points.
        
        // Configure color
        line.startColor = line.endColor = shipColor;
        line.colorGradient.mode = GradientMode.Fixed;
        
        // If we load points into the LineRenderer w/o updating positionCount, the array will be truncated to length 1
        line.positionCount = segments;

        line.widthMultiplier = thickness; // Set line thickness
        
        float dAngle = 2 * Mathf.PI / segments; // Rate at which the angle changes between every segment
        float len = 2 * Mathf.PI * radius / segments; // Length of each line segment

        // Copy the center position and move it to the bottom of the circle (this will be our first point)
        pos.y -= radius;
        points[0] = pos;

        float angle = dAngle / 2; // Loop variable for storing the current angle
        for (int i = 1; i < segments; i++, angle += dAngle) {
            pos.y += Mathf.Sin(angle) * len; // Calculate Opposite + update point position
            pos.x += Mathf.Cos(angle) * len; // Calculate Adjacent + update point position
            
            points[i] = pos; // Save the point in the point array
        }
        
        line.SetPositions(points);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("Trigger enter", other); // Debug
        Selected = !Selected; // Flip the circle on/off
    }
}
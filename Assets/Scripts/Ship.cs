using JetBrains.Annotations;
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
    
    private static float DifferenceToStop(float cur, float decel) => 
        Mathf.Sign(cur) * ((cur * cur / decel) - (0.5f * decel * Mathf.Pow(cur / decel, 2)));

    public const float SpeedMax = 2f;
    public const float TurnMax = 10f;
    public const float TurnAccel = 4f;
    public const float TurnSnap = 0.001f;
    private float TurnCur = 0f;
    
    private void Move(Vector3 target) {
        Vector3 position = gameObject.transform.position;
        
        // ===== ROTATION =====
        float targetAngle = Vector2.SignedAngle(position, target - position);
        float toMove = Mathf.DeltaAngle(gameObject.transform.eulerAngles.z, targetAngle);

        // If both our turning speed and our desired angle is below TurnSnap, stop processing rotation commands
        if (Mathf.Abs(toMove) <= TurnSnap && Mathf.Abs(TurnCur) <= TurnSnap) {
            // Just set our rotation to the target
            var qAngles = gameObject.transform.eulerAngles;
            qAngles.z += targetAngle;
            gameObject.transform.eulerAngles = qAngles;
            return;
        }

        // Increase or decrease the turn speed, based off of if we need to start slowing down
        TurnCur = Mathf.MoveTowards(
            TurnCur, 
            (DifferenceToStop(TurnCur, TurnAccel) > toMove) ? -TurnMax : TurnMax, 
            TurnAccel * Time.deltaTime
        ); 
        
        // Update the actual angle
        var angles = gameObject.transform.eulerAngles;
        angles.z += TurnCur * Time.deltaTime;
        gameObject.transform.eulerAngles = angles;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("Trigger enter", other); // Debug
        Selected = !Selected; // Flip the circle on/off
    }
    
    private Camera _camera;
    
    void Start() {
        _camera = Camera.main;
        
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

    private Vector3? _target = null;
    [CanBeNull] private GameObject _objTarget = null;

    private Vector3 MousePosition() {
        Vector3 cam = _camera!.ScreenToWorldPoint(Input.mousePosition);
        cam.z = 0;
        return cam;
    }
    
    void Update() {
        _target = MousePosition();
        // If we are following a GameObject, replace our target with that object
        if (_objTarget is not null) _target = _objTarget.transform.position;
        // If we have no target at all, stop.
        if (_target is null) return;
        
        Move((Vector3) _target);
    }
}
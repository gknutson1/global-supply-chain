using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

public class Ship : MonoBehaviour
{
    public Color shipColor;
    public Sprite shipSprite;

    private LineRenderer _selectionRing;

    private bool _selected = false;

    private bool Selected
    {
        get => _selected;
        set => SetSelected(value);
    }

    private void SetSelected(bool state)
    {
        // If the new state is the same as the old, return early
        if (state == Selected) return;

        // If selected -> deselected
        if (Selected) _selectionRing.enabled = false;

        // If deselected -> selected
        else _selectionRing.enabled = true;

        _selected = state; // Actually write the new value
    }

    private void DrawCircle(LineRenderer line, Vector3 pos, float radius, float thickness = 0.02f, int segments = 32)
    {
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
        for (int i = 1; i < segments; i++, angle += dAngle)
        {
            pos.y += Mathf.Sin(angle) * len; // Calculate Opposite + update point position
            pos.x += Mathf.Cos(angle) * len; // Calculate Adjacent + update point position

            points[i] = pos; // Save the point in the point array
        }

        line.SetPositions(points);
    }

    private static float DifferenceToStop(float cur, float decel) =>
        Mathf.Sign(cur) * ((cur * cur / decel) - (0.5f * decel * Mathf.Pow(cur / decel, 2)));

    public float TurnMax = 10f;
    public float TurnAccel = 4f;
    public float TurnSnap = 0.001f;
    public float TurnCur = 0f;

    private float Rotate(Vector3 target) {
        if (!move) return 180;
        Vector3 position = gameObject.transform.position;
        
        float targetAngle = Mathf.Rad2Deg * Mathf.Atan2(target.y - position.y, target.x - position.x) + 180;
        float toMove = Mathf.DeltaAngle(gameObject.transform.eulerAngles.z, targetAngle);
        //print($"{position}, {target}, {targetAngle}");

        // If both our turning speed and our desired angle is below TurnSnap, stop processing rotation commands
        if (Mathf.Abs(toMove) <= TurnSnap && Mathf.Abs(TurnCur) <= TurnSnap)
        {   
            // Just set our rotation to the target
            var qAngles = gameObject.transform.eulerAngles;
            qAngles.z = targetAngle;
            gameObject.transform.eulerAngles = qAngles;
            return toMove;
        }
        
        // Do we need to start slowing down?
        float tgt = DifferenceToStop(TurnCur, TurnAccel) >= toMove ? -TurnMax : TurnMax;
        if (SpeedSnap >= distToDest) tgt = 0;

        // Increase or decrease the turn speed, based off of if we need to start slowing down
        TurnCur = Mathf.MoveTowards(TurnCur, tgt, TurnAccel * Time.deltaTime);

        // Update the actual angle
        var angles = gameObject.transform.eulerAngles;
        angles.z += TurnCur * Time.deltaTime;
        gameObject.transform.eulerAngles = angles;

        fixHealthBarLocation();

        return toMove;
    }

    private void fixHealthBarLocation() {
        _healthBarCanvas.transform.eulerAngles = Vector3.zero;

        Vector3 position = gameObject.transform.position;
        position.y -= 1;
        _healthBarCanvas.transform.position = position;
    }

    public float SpeedMax = 10f;
    public float SpeedAccel = .8f;
    public float SpeedSnap = 0.001f;
    public float SpeedCur = 0f;

    public bool move = true;

    private void Move(Vector3 target, float remain) {
        if (!move) return;
        Vector3 position = gameObject.transform.position;
                
        if (Mathf.Abs(remain) > Mathf.Lerp(45, 0, SpeedCur / SpeedMax) || distToDest / SpeedCur <= remain / TurnCur ) {
            SpeedCur = Mathf.MoveTowards(SpeedCur, 0, SpeedAccel * Time.deltaTime);
        }
        else {
            SpeedCur = Mathf.MoveTowards(
                SpeedCur,
                (DifferenceToStop(SpeedCur, SpeedAccel) >= distToDest || SpeedSnap >= distToDest) ? 0 : SpeedMax,
                SpeedAccel * Time.deltaTime
            );
        }

        float rotation = gameObject.transform.eulerAngles.z;
        position.y += Mathf.Sin(rotation * Mathf.Deg2Rad) * -1 * SpeedCur * Time.deltaTime;
        position.x += Mathf.Cos(rotation * Mathf.Deg2Rad) * -1 * SpeedCur * Time.deltaTime;

        
        gameObject.transform.position = position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger enter", other); // Debug
        Selected = !Selected; // Flip the circle on/off
    }

    private Camera _camera;

    void Start()
    {
        target = gameObject.transform.position;
        
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

        _healthBarCanvas = GetComponentInChildren<Canvas>();
        _healthBar = _healthBarCanvas.GetComponentInChildren<HealthBar>();
        _currentHealth = MaxHealth;
        fixHealthBarLocation();

        StartCoroutine(Attack());
    }

    public Vector3 target;
    [CanBeNull] private GameObject _objTarget = null;

    private Vector3 MousePosition()
    {
        Vector3 cam = _camera!.ScreenToWorldPoint(Input.mousePosition);
        cam.z = 0;
        return cam;
    }
    
    
    // Pythagorean theorem
    private float distToDest;

    void Update() {
        if (Selected && Input.GetMouseButtonDown(0))
            Selected = false;
        
        if (Selected && Input.GetMouseButtonDown(1))
            target = MousePosition();
        
        //target = MousePosition();
        // If we are following a GameObject, replace our target with that object
        if (_objTarget is not null) target = _objTarget.transform.position;
        
        var pos = gameObject.transform.position;

        distToDest = Mathf.Sqrt(Mathf.Abs(target.x - pos.x) + Mathf.Abs(target.y - pos.y));

        float remain = Rotate(target);

        Move(target, remain);
    }

    public GameObject ProjectilePrefab;
    public Sprite ProjectileSprite;
    public float AttackRadius = 5f;
    public float AttackRate = 1f;
    public int Strength = 10;
    public float Accuracy = 1f;
    public int MaxAttackCount = 1;
    public int MaxHealth = 50;
    public float Evasion = 0f;
    int _currentHealth;


    IEnumerator Attack()
    {
        yield return new WaitForSeconds(1);

        while (true)
        {
            var hitColliders = Physics2D.OverlapCircleAll(transform.position, AttackRadius, LayerMask.GetMask("Ships"));
            var attackCount = 0;

            foreach (var collider in hitColliders)
            {
                if (collider.tag != tag && attackCount < MaxAttackCount)
                {
                    var projectileAngle = Vector2.SignedAngle(Vector3.left, collider.transform.position - transform.position);
                    var projectileRotation = Quaternion.Euler(0, 0, projectileAngle);
                    var projectileObject = Instantiate(ProjectilePrefab, transform.position, projectileRotation);
                    var projectile = projectileObject.GetComponent<Projectile>();

                    projectile.Fire(collider, ProjectileSprite, Random.value <= Accuracy * (1f - Evasion), Strength);
                    
                    attackCount++;
                }
            }

            yield return new WaitForSeconds(AttackRate);
        }
    }

    private Canvas _healthBarCanvas;
    private HealthBar _healthBar;

    public void Hit(int damage)
    {
        _currentHealth -= damage;
        _healthBar.UpdateHealthBar((float)_currentHealth / MaxHealth);
        if (_currentHealth <= 0) Destroy(gameObject);   
    }
}
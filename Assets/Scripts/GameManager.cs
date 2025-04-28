using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{

    // Layer that selectable ships live on
    [SerializeField] private LayerMask shipLayer;

    // The collider that is used to detect when the user has selected a ship
    private GameObject _selector;

    private PersistentVariables _persistentVariables;
    
    public Vector2 PlayerSpawn;
    public float SpawnRotation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _camera = Camera.main; // for performance reasons

        // Grab the selector object, and make sure it is disabled and has a zeroed transform
        _selector = GameObject.Find("SelectorBox");
        _selector.SetActive(false);
        _selector.transform.localScale = Vector3.zero;

        _moveAction = InputSystem.actions.FindAction("Move");
        _scrollAction = InputSystem.actions.FindAction("Zoom");

        _persistentVariables = FindAnyObjectByType<PersistentVariables>();

        _minBounds = tilemap.LocalToWorld(tilemap.localBounds.min);
        _maxBounds = tilemap.LocalToWorld(tilemap.localBounds.max);
        ScrollMax = Mathf.Min(ScrollMax, (_maxBounds.x - _minBounds.x) * Screen.height / Screen.width / 2, (_maxBounds.y - _minBounds.y) / 2);

        // For the first level, we need to immediately load level 0 ships, as there is no UpgradeManager to do so for us
        if (_persistentVariables.level == 0) {
            _persistentVariables.ships = _persistentVariables.level0NewShips;
        }

        float distanceFrom = 5;
        Vector3 pos = PlayerSpawn;
        pos.z = 0;
        
        var posChange = new Vector3(
            Mathf.Cos(((SpawnRotation + 90) * Mathf.Deg2Rad)) * distanceFrom,
            Mathf.Sin(((SpawnRotation + 90) * Mathf.Deg2Rad)) * distanceFrom,
            0
        );
        
        Debug.Log(posChange);


        foreach (Ship ship in _persistentVariables.ships) {
            Debug.Log("here");
            GameObject obj = Instantiate(ship.gameObject, pos, Quaternion.Euler(0, 0, SpawnRotation), gameObject.transform);
            obj.transform.SetParent(gameObject.transform);
            pos += posChange;
        }
    }

    /// Get position of the mouse as a unity coordinate (zeroed out)
    private Vector3 MousePosition()
    {
        Vector3 cam = _camera!.ScreenToWorldPoint(Input.mousePosition);
        cam.z = 0;
        return cam;
    }

    private bool _selecting = false;
    private bool _moving = false;
    private Vector3 _originPos;
    private Camera _camera;

    // Update is called once per frame
    void Update()
    {
        // Enter into selection mode when user clicks LMB
        if (Input.GetMouseButtonDown(0))
        {
            _selector.SetActive(true);
            _selector.transform.position = _originPos = MousePosition();
            _selecting = true;
        }
        // Finalize selection when user lets go of LMB
        else if (Input.GetMouseButtonUp(0))
        {
            // Reset the selector
            _selecting = false;
            _selector.SetActive(false);
            _selector.transform.localScale = Vector3.zero;
        }
        // Move the selector box as the user moves the mouse around
        else if (_selecting)
        {
            Vector3 mousePos = MousePosition();
            _selector.transform.localScale = _originPos - mousePos;
            // Position the selector directly in the middle between origin and mouse
            _selector.transform.position = ((mousePos - _originPos) / 2) + _originPos;
        }

        if (Input.GetMouseButtonDown(2))
        {
            _originPos = MousePosition();
            _moving = true;
        }
        else if (Input.GetMouseButtonUp(2))
        {
            _moving = false;
        }
        else if (_moving)
        {
            Vector3 mousePos = MousePosition();

            _camera.transform.position += _originPos - mousePos;

            _camera.transform.position = new Vector3(
                Mathf.Clamp(_camera.transform.position.x, _minBounds.x + _camera.orthographicSize * Screen.width / Screen.height, _maxBounds.x - _camera.orthographicSize * Screen.width / Screen.height),
                Mathf.Clamp(_camera.transform.position.y, _minBounds.y + _camera.orthographicSize, _maxBounds.y - _camera.orthographicSize),
                _camera.transform.position.z
            );
        }


        if (_moveAction.inProgress) OnMove();
        if (_scrollAction.inProgress) OnScroll();

        if (Input.GetKeyDown(KeyCode.Escape)) TogglePauseGame();
    }

    private InputAction _moveAction;
    private const float MoveSpeed = 3;

    private void OnMove()
    {
        _camera.transform.position += (Vector3)(MoveSpeed * _camera.orthographicSize * Time.deltaTime * _moveAction.ReadValue<Vector2>());

        _camera.transform.position = new Vector3(
            Mathf.Clamp(_camera.transform.position.x, _minBounds.x + _camera.orthographicSize * Screen.width / Screen.height, _maxBounds.x - _camera.orthographicSize * Screen.width / Screen.height),
            Mathf.Clamp(_camera.transform.position.y, _minBounds.y + _camera.orthographicSize, _maxBounds.y - _camera.orthographicSize),
            _camera.transform.position.z
        );
    }

    private InputAction _scrollAction;
    private const float ScrollSpeed = 100;
    private const float ScrollMin = 2;
    private float ScrollMax = 80;
    public Tilemap tilemap;
    private Vector2 _minBounds;
    private Vector2 _maxBounds;

    private void OnScroll()
    {
        // Get the old world position of mouse
        Vector3 prevPos = MousePosition();

        _camera.orthographicSize = Mathf.Clamp(
            // -1 because otherwise the scroll direction is inverted.
            _camera.orthographicSize + ScrollSpeed * Time.deltaTime * (-1 * _scrollAction.ReadValue<float>()),
            ScrollMin,
            ScrollMax
        );

        // Get the new world position of mouse
        Vector3 newPos = MousePosition();

        // Adjust the position of the camera based off of the difference between the new and old mouse positions
        _camera.transform.position += prevPos - newPos;

        _camera.transform.position = new Vector3(
            Mathf.Clamp(_camera.transform.position.x, _minBounds.x + _camera.orthographicSize * Screen.width / Screen.height, _maxBounds.x - _camera.orthographicSize * Screen.width / Screen.height),
            Mathf.Clamp(_camera.transform.position.y, _minBounds.y + _camera.orthographicSize, _maxBounds.y - _camera.orthographicSize),
            _camera.transform.position.z
        );
    }

    public Canvas pauseMenu;
    public Canvas optionsMenu;
    public Canvas GameOverCanvas;
    bool isPaused = false;

    // Pause the game and show the pause menu, or unpause the game and hide the pause menu.
    public void TogglePauseGame()
    {
        isPaused ^= true;
        // TODO: Add logic for actually pausing the game while the pause menu is displayed (or resuming when it is not).
        Time.timeScale = isPaused ? 0f : 1f;
        pauseMenu.enabled = isPaused;
        optionsMenu.enabled = false;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene($"Level{_persistentVariables.level}");
    }

    public void GoToMainMenu()
    {
        Destroy(_persistentVariables.gameObject);
        SceneManager.LoadScene("MainMenu");
    }
}
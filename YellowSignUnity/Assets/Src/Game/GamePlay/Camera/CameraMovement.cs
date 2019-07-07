using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float speed;
    public float drag;
    public float edgeLimit;

    private Vector3 _velocity;
    private Vector3 _acceleration;
    // Use this for initialization
    private Vector3 _currentPos;
    private Vector3 _startPos;
    //private Transform _movementTransform;
    private Rect _worldLimit;
    private Camera _camera;
    private bool _isLocked;
    private bool _isDebugLocked;

    public PlayerSpawn playerSpawn { get; set; }

    public bool isLocked
    {
        get { return _isLocked || _isDebugLocked; }
        set { _isLocked = value; }
    }
    
    public void Setup(PlayerSpawn spawn)
    {
        if(spawn != null)
        {
            playerSpawn = spawn;
            //playerNumber = spawn.playerNumber;

            Rect temp = spawn.cameraLimit;
            _worldLimit.x = Mathf.Min(temp.x, temp.width);
            _worldLimit.width = Mathf.Max(temp.x, temp.width);
            _worldLimit.y = Mathf.Min(temp.y, temp.height);
            _worldLimit.height = Mathf.Max(temp.y, temp.height);
            
            float pitch = transform.localEulerAngles.x;

            Vector3 angle = spawn.cameraHook.eulerAngles;
            angle.x = pitch;
            Quaternion rot = Quaternion.Euler(angle);

            transform.SetPositionAndRotation(spawn.cameraHook.position, rot);
            _currentPos = transform.localPosition;
            _startPos = _currentPos;

            _isDebugLocked = true;
        }
    }

    public Camera camera
    {
        get { return _camera; }
    }

	void Awake ()
    {
        _acceleration = Vector3.zero;

        _camera = GetComponent<Camera>();
        _currentPos = transform.localPosition;
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            _isDebugLocked = !_isDebugLocked;
        }

        if(isLocked || playerSpawn == null || playerSpawn.cameraHook == null)
        {
            return;
        }

        //Debug
        if(Input.GetButtonDown("Jump"))
        {
            Cursor.lockState = (Cursor.lockState == CursorLockMode.None) ? CursorLockMode.Confined : CursorLockMode.None;
        }

        if(Input.GetKeyDown(KeyCode.C))
        {
            _velocity = Vector3.zero;
            _currentPos = _startPos;
            transform.localPosition = _startPos;
        }
        
        if(transform != null)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, _currentPos, Time.deltaTime);
        }
    }
    // Not getting input from TrueSyncInput. Don't need to sync camera movement
    public void FixedUpdate()
    {
        if(playerSpawn == null || playerSpawn.cameraHook == null)
        {
            return;
        }

        Vector3 camPos = playerSpawn.cameraHook.position + transform.position;
        
        int mouseX = (int)Input.mousePosition.x;
        int mouseY = (int)Input.mousePosition.y;


        //Debug.Log("CamPos: " + camPos);
        //Debug.Log("WorldLim: " + _worldLimit);

        if(mouseX < edgeLimit)// && camPos.x > _worldLimit.x )
        {
            //Debug.Log("Left");
            _acceleration.x = (-transform.right).x;
        }
        else if(mouseX > Screen.width - edgeLimit)// && camPos.x < _worldLimit.width)
        {
            //Debug.Log("Right");
            _acceleration.x = (transform.right).x;
        }

        if(mouseY < edgeLimit)// && camPos.z > _worldLimit.y)
        {
            //Debug.Log("Back");
            _acceleration.z = (-transform.forward).z;
        }
        else if (mouseY > Screen.height - edgeLimit)// && camPos.z < _worldLimit.height)
        {
            //Debug.Log("Forward");
            _acceleration.z = (transform.forward).z;
        }

        float deltaTime = Time.fixedDeltaTime;
        _acceleration = _acceleration.normalized * speed;
        _currentPos = transform.localPosition + _velocity * deltaTime;

        float dragForce = (1.0f - drag * deltaTime);
        _velocity = (_velocity + _acceleration * deltaTime) * dragForce;
        _velocity.y = 0;
        
        _acceleration = Vector3.zero;
    }
}

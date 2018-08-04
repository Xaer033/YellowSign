using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;
using Sirenix.OdinInspector;

public class CameraMovement : MonoBehaviour
{
    public Rect[] worldLimits;

    public float speed;
    public float drag;
    public float edgeLimit;

    private Vector3 _velocity;
    private Vector3 _acceleration;
    // Use this for initialization
    private Vector3 _currentPos;
    //private Transform _movementTransform;
    private Rect _worldLimit;
    private Camera _camera;

    public PlayerNumber playerNumber { get; set; }

    public void Setup(PlayerSpawn spawn)
    {
        if(spawn != null)
        {
            playerNumber = spawn.playerNumber;
            _worldLimit = worldLimits[(byte)playerNumber - 1];

            float pitch = transform.localEulerAngles.x;

            Vector3 angle = spawn.cameraHook.eulerAngles;
            angle.x = pitch;
            Quaternion rot = Quaternion.Euler(angle);

            transform.SetPositionAndRotation(spawn.cameraHook.position, rot);
            _currentPos = transform.localPosition;
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
    }

    public void Update()
    {
        //Debug
        if (Input.GetButtonDown("Jump"))
        {
            Cursor.lockState = (Cursor.lockState == CursorLockMode.None) ? CursorLockMode.Confined : CursorLockMode.None;
        }

        if(transform != null)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, _currentPos, Time.deltaTime);
        }
    }
    // Not getting input from TrueSyncInput. Don't need to sync camera movement
    public void FixedUpdate()
    {
        if(transform == null)
        {
            return;
        }

        Vector3 camPos = transform.position;
        
        int mouseX = (int)Input.mousePosition.x;
        int mouseY = (int)Input.mousePosition.y;


        if (mouseX < edgeLimit && camPos.x > _worldLimit.xMin )
        {
            _acceleration.x = -1.0f;
        }
        else if(mouseX > Screen.width - edgeLimit && camPos.x < _worldLimit.xMax)
        {
            _acceleration.x = 1.0f;
        }

        if (mouseY < edgeLimit && camPos.z > _worldLimit.yMin)
        {
            _acceleration.z = -1.0f;
        }
        else if (mouseY > Screen.height - edgeLimit && camPos.z < _worldLimit.yMax)
        {
            _acceleration.z = 1.0f;
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

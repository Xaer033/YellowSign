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
    private Transform _movementTransform;
    //private Rect worldLimit;
    private Camera _camera;

    public PlayerNumber playerNumber { get; set; }

    public Camera camera
    {
        get { return _camera; }
    }

	void Awake ()
    {
        _movementTransform = transform.parent;
        _acceleration = Vector3.zero;
        _currentPos = _movementTransform.localPosition;

        _camera = GetComponent<Camera>();
    }

    public void Update()
    {
        //Debug
        if (Input.GetButtonDown("Jump"))
        {
            Cursor.lockState = (Cursor.lockState == CursorLockMode.None) ? CursorLockMode.Confined : CursorLockMode.None;
        }
        _movementTransform.localPosition = Vector3.Lerp(_movementTransform.localPosition, _currentPos, Time.deltaTime);
    }
    // Not getting input from TrueSyncInput. Don't need to sync camera movement
    public void FixedUpdate()
    {
        Vector3 camPos = transform.position;
        
        int mouseX = (int)Input.mousePosition.x;
        int mouseY = (int)Input.mousePosition.y;
        Rect worldLimit = worldLimits[(byte)playerNumber - 1];

        if (mouseX < edgeLimit && camPos.x > worldLimit.x )
        {
            _acceleration.x = -1.0f;
        }
        else if(mouseX > Screen.width - edgeLimit && camPos.x < worldLimit.width)
        {
            _acceleration.x = 1.0f;
        }

        if (mouseY < edgeLimit && camPos.z > worldLimit.y)
        {
            _acceleration.z = -1.0f;
        }
        else if (mouseY > Screen.height - edgeLimit && camPos.z < worldLimit.height)
        {
            _acceleration.z = 1.0f;
        }

        float deltaTime = Time.fixedDeltaTime;
        _acceleration = _acceleration.normalized * speed;
        _currentPos = _movementTransform.localPosition + _velocity * deltaTime;

        float dragForce = (1.0f - drag * deltaTime);
        _velocity = (_velocity + _acceleration * deltaTime) * dragForce;

        _acceleration = Vector3.zero;
    }
}

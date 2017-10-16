using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;

public class CameraMovement : MonoBehaviour
{
    public Rect worldLimit;

    public float speed;
    public float drag;
    public float edgeLimit;

    private Vector3 _velocity;
    private Vector3 _acceleration;
    // Use this for initialization
    private Vector3 _currentPos;
        
	void Awake ()
    {
        _acceleration = new Vector3();
        _currentPos = transform.position;
    }

    public void Update()
    {
        //Debug
        if (Input.GetButtonDown("Jump"))
        {
            Cursor.lockState = (Cursor.lockState == CursorLockMode.None) ? CursorLockMode.Confined : CursorLockMode.None;
        }
        transform.position = Vector3.Lerp(transform.position, _currentPos, Time.deltaTime);
    }
    // Not getting input from TrueSyncInput. Don't need to sync camera movement
    public void FixedUpdate()
    {
        Vector3 camPos = transform.position;
        int mouseX = (int)Input.mousePosition.x;
        int mouseY = (int)Input.mousePosition.y;
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
        _acceleration = _acceleration.normalized * speed;
        _currentPos = transform.position + _velocity * Time.fixedDeltaTime;
        
        Vector3 friction = _velocity * -drag;
        _velocity = _velocity + friction +  _acceleration * Time.fixedDeltaTime;

        _acceleration = Vector3.zero;
    }
}

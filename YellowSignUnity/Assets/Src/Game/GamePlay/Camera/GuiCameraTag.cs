using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GuiCameraTag : MonoBehaviour
{
    [SerializeField]
    private Camera _camera;

    [SerializeField]
    private Canvas _canvas;

    [SerializeField]
    private EventSystem _eventSystem;
    
    public Camera camera
    {
        get { return _camera; }
    }

    public Canvas canvas
    {
        get { return _canvas; }
    }

    public EventSystem eventSystem
    {
        get { return _eventSystem; }
    }
}

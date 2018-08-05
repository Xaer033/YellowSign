using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CreepHealthUISystem : MonoBehaviour
{
    public const int kMaxUIView = 200;


    public Camera cam
    {
        get
        {
            if(_camera == null)
            {
                GameObject camObj = GameObject.FindWithTag("MainCamera");
                _camera = camObj.GetComponent<Camera>();
            }
            return _camera;
        }
    }

    public Canvas canvas { get; set; }

    public float scaleConst = 12.0f;

    private Stack<CreepHealthView> _viewPool = new Stack<CreepHealthView>(kMaxUIView);
    private Dictionary<Creep, CreepHealthView> _inUseMap= new Dictionary<Creep, CreepHealthView>(kMaxUIView);
    private RectTransform _canvasRectTransform;
    private Camera _camera;
    private GameplayResources _gameplayResources;
    private Vector3 _savedScale;

    [Inject]
    public void Construct(GameplayResources gameplayResources)
    {
        _gameplayResources = gameplayResources;
    }

	// Use this for initialization
	void Start ()
    {
        canvas = Singleton.instance.gui.mainCanvas;

        _canvasRectTransform = canvas.transform as RectTransform;

        _savedScale = _gameplayResources.CreepHealthUIPrefab.transform.localScale;

        for(int i = 0; i < kMaxUIView; ++i)
        {
            CreepHealthView view = GameObject.Instantiate<CreepHealthView>(
                _gameplayResources.CreepHealthUIPrefab, 
                canvas.transform, false);

            view.gameObject.SetActive(false);

            _viewPool.Push(view);
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(cam == null || canvas == null)
        {
            return;
        }

        foreach(var pair in _inUseMap)
        {
            Creep c = pair.Key;
            CreepHealthView view = pair.Value;

            view.healthFill = (float)c.state.health / (float)c.stats.maxHealth;

            Vector3 worldHealthPos = c.view.healthPosition;
            Vector3 anchorPos = GetAnchoredPositionFromWorldPosition(worldHealthPos, cam, canvas);
            view.rectTransform.anchoredPosition = anchorPos;


            float scaleMod = scaleConst * (1.0f / Vector3.Distance(cam.transform.position, worldHealthPos));
            view.transform.localScale = _savedScale * scaleMod;
        }
    }

    public Vector2 GetAnchoredPositionFromWorldPosition(Vector3 _worldPostion, Camera _camera, Canvas _canvas)
    {
        //Vector2 myPositionOnScreen = _camera.WorldToScreenPoint(_worldPostion); // for transform.position?
        Vector2 viewportPosition = _camera.WorldToViewportPoint(_worldPostion); //for RectTransform.AnchoredPosition?
        Vector2 screenPos = new Vector2(
             ((viewportPosition.x * _canvasRectTransform.sizeDelta.x) - (_canvasRectTransform.sizeDelta.x * 0.5f)),
             ((viewportPosition.y * _canvasRectTransform.sizeDelta.y) - (_canvasRectTransform.sizeDelta.y * 0.5f)));

        float scaleFactor = 1.0f; //_canvas.scaleFactor;
        return new Vector2(screenPos.x / scaleFactor, screenPos.y / scaleFactor);
    }

    public void CleanUp()
    {
        foreach(var pair in _inUseMap)
        {
            GameObject.Destroy(pair.Value.gameObject);        
        }

        CreepHealthView v = null;
        while(v = _viewPool.Pop())
        {
            GameObject.Destroy(v.gameObject);
        }
    }

    public void AddCreep(Creep c)
    {
        if(_viewPool.Count > 0)
        {
            CreepHealthView view = _viewPool.Pop();
            _inUseMap.Add(c, view);
            //Debug.Log("Creep Added");

            view.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("No More CreepHealthView!");
        }
    }

    public void RemoveCreep(Creep c)
    {
        CreepHealthView view;
        if(_inUseMap.TryGetValue(c, out view))
        {
            _inUseMap.Remove(c);
            _viewPool.Push(view);
        
            view.gameObject.SetActive(false);

        }
        
        //Debug.Log("Creep Remove");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CreepHealthUISystem : MonoBehaviour
{
    public const int kMaxUIView = 200;

    private GuiCameraTag _guiCameraTag;
    
    public Canvas canvas
    {
        get { return _guiCameraTag.dynamicCanvas; }
    }

    public float scaleConst = 15.0f;

    private Stack<CreepHealthUIView> _viewPool = new Stack<CreepHealthUIView>(kMaxUIView);
    private Dictionary<Creep, CreepHealthUIView> _inUseMap= new Dictionary<Creep, CreepHealthUIView>(kMaxUIView);
    private RectTransform _canvasRectTransform;
    private Camera _camera;
    private GameplayResources _gameplayResources;
    private Vector3 _savedScale;

    [Inject]
    public void Construct(GameplayResources gameplayResources)
    {
        _gameplayResources = gameplayResources;
    }

    private void Awake()
    {
        _guiCameraTag = GameObject.FindObjectOfType<GuiCameraTag>();
        _canvasRectTransform = _guiCameraTag.canvas.transform as RectTransform;
    }

    // Use this for initialization
    void Start ()
    {

        _savedScale = _gameplayResources.creepHealthUIPrefab.transform.localScale;

        for(int i = 0; i < kMaxUIView; ++i)
        {
            CreepHealthUIView view = GameObject.Instantiate<CreepHealthUIView>(
                _gameplayResources.creepHealthUIPrefab, 
                canvas.transform, false);

            //view.gameObject.SetActive(false);

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
            CreepHealthUIView view = pair.Value;

            Vector3 worldHealthPos = c.view.healthPosition;
            Vector3 anchorPos = getScreenPositionFromWorldPosition(worldHealthPos, cam, canvas);
            view.rectTransform.anchoredPosition = anchorPos;

            float dist = Vector3.Distance(cam.transform.position, worldHealthPos);
            dist = dist < 0.001f ? 0.001f : dist;

            float scaleMod = scaleConst * (1.0f / dist);
            view.transform.localScale = _savedScale * scaleMod;
        }
    }

    private Vector2 getScreenPositionFromWorldPosition(Vector3 worldPostion, Camera camera, Canvas _canvas)
    {
        Vector2 viewportPosition = camera.WorldToViewportPoint(worldPostion);
        Vector2 screenPos = new Vector2(
             ((viewportPosition.x * _canvasRectTransform.sizeDelta.x) - (_canvasRectTransform.sizeDelta.x * 0.5f)),
             ((viewportPosition.y * _canvasRectTransform.sizeDelta.y) - (_canvasRectTransform.sizeDelta.y * 0.5f)));
        
        return new Vector2(screenPos.x, screenPos.y);
    }

    private Camera cam
    {
        get
        {
            if(_camera == null)
            {
                GameObject camObj = GameObject.FindGameObjectWithTag("MainCamera");
                _camera = camObj.GetComponent<Camera>();
            }
            return _camera;
        }
    }

    public void CleanUp()
    {
        foreach(var pair in _inUseMap)
        {
            GameObject.Destroy(pair.Value.gameObject);        
        }

        CreepHealthUIView v = null;
        while(v = _viewPool.Pop())
        {
            GameObject.Destroy(v.gameObject);
        }
    }

    public void AddCreep(Creep c)
    {
        //popView(c);
    }

    public void ShowHealthOnCreep(Creep c)
    {
        CreepHealthUIView view = getView(c);

        if(view != null)
        {
            float health = (float)c.state.health / (float)c.stats.maxHealth;
            view.SetHealthFill(health, ()=> recycleView(c));
        }

    }

    public void RemoveCreep(Creep c)
    {

        recycleView(c);
        //Debug.Log("Creep Remove");
    }

    private CreepHealthUIView popView(Creep c)
    {
        CreepHealthUIView view = null;
        if(_viewPool.Count > 0)
        {
            view = _viewPool.Pop();
            //Debug.Log("Creep Added");

            //view.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("No More CreepHealthView!");
        }
        return view;
    }

    private void recycleView(Creep c)
    {
        CreepHealthUIView view;
        if(_inUseMap.TryGetValue(c, out view))
        {
            _inUseMap.Remove(c);
            _viewPool.Push(view);

            view.KillTween(false);
            view._canvasGroup.alpha = 0;
            //view.gameObject.SetActive(false);
        }
    }

    private CreepHealthUIView getView(Creep c)
    {
        CreepHealthUIView view;
        if(!_inUseMap.TryGetValue(c, out view))
        {
            view = popView(c);
            if(view != null)
            {
                _inUseMap.Add(c, view);
            }
        }
        return view;
    }
}

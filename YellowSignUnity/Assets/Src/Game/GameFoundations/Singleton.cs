using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using GhostGen;


public class Singleton : MonoBehaviour 
{
    [SerializeField]
    private bool firstScene = false;

    public GameConfig           gameConfig          { get; private set; }
    public SessionFlags         sessionFlags        { get; private set; }
    public GameStateMachine<YellowSignState> gameStateMachine    { get; private set; }

    public GuiManager           gui                 { get { return gameConfig.guiManager; } }
    public GameplayResources    gameplayResources   { get { return gameConfig.gameplayResources; } }
    public NetworkManager       networkManager      { get; private set; }

    public EventDispatcher      notificationDispatcher { get; private set; }
    
    private IStateFactory<YellowSignState> _stateFactory;

    private static object _lock = new object();

    private static bool applicationIsQuitting = false;
    private static Singleton _instance = null;

    public void Awake()
    {
        if(firstScene)
        {
            _instance = this;
            _initialize();
        }
        else
        {
            if (_instance != null)
            {
                Destroy(gameObject);
            }
        }
    }
    public void Start()
    {
        gameStateMachine.ChangeState(gameConfig.initalState);
    }


    public void Update()
    {
        gameStateMachine.Step(Time.deltaTime);
        gui.Step(Time.deltaTime);
    }


    public static Singleton instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                Debug.LogWarning("[Singleton] Instance " +
                    "already destroyed on application quit." +
                    " Won't create again - returning null.");
                return null;
            }

            if (_instance == null)
            {
                _instance = FindObjectOfType<Singleton>();

                if (FindObjectsOfType<Singleton>().Length > 1)
                {
                    Debug.LogError("[Singleton] Something went really wrong " +
                        " - there should never be more than 1 singleton!" +
                        " Reopenning the scene might fix it.");
                    return _instance;
                }

                if (_instance == null)
                {
                    lock (_lock)
                    {
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<Singleton>();
                        _instance._initialize();
                    }
                }
                else
                {
                    Debug.Log("[Singleton] Using instance already created: " +
                        _instance.gameObject.name);
                }
            }
            return _instance;
        }
    }

    private void _initialize()
    {
        name = "(singleton)";
        DontDestroyOnLoad(gameObject);

        gameConfig = Resources.Load<GameConfig>("GameConfig");

        _stateFactory = new YellowSignStateFactory();
        gameStateMachine = new GameStateMachine<YellowSignState>(_stateFactory);
        sessionFlags = new SessionFlags();
        notificationDispatcher = new EventDispatcher();

        networkManager = gameObject.AddComponent<NetworkManager>();
        Input.multiTouchEnabled = false; //This needs to go elsewere 

        _postInit();
    }

    private void _postInit()
    {
        //AbstractPostInit[] postInits = GameObject.FindObjectsOfType<AbstractPostInit>();

        gui.PostInit();
        networkManager.PostInit();
        //System.Type typeOfMainClass = this.GetType();
        
        //PropertyInfo[] infoList = typeOfMainClass.GetProperties();
        //for(int i = 0; i < infoList.Length; ++i)
        //{
        //    PropertyInfo propertyInfo = infoList[i];
        //    if(typeof(IPostInit).IsAssignableFrom(propertyInfo.GetType()))
        //    {
        //        object instanceOfProperty = propertyInfo.GetValue(this);
                
        //        System.Type typeofMainProperty = instanceOfProperty.GetType();
        //        MethodInfo methodOfMainProperty = typeofMainProperty.GetMethod("PostInit");
        //        methodOfMainProperty.Invoke(instanceOfProperty, new object[0]);
        //    }
        //}
    }

    public void OnDestroy()
    {
        applicationIsQuitting = true;
    }
}

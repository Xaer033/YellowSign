using System.Collections;
using System.Collections.Generic;
using Zenject;
using UnityEngine;
using GhostGen;


public class Singleton : IInitializable, ITickable, ILateDisposable
{
    public GameConfig           gameConfig          { get; private set; }
    public SessionFlags         sessionFlags        { get; private set; }
    
    public GameStateMachine<YellowSignState> gameStateMachine    { get; private set; }

    public GuiManager           gui                 { get { return gameConfig.guiManager; } }
    public NetworkManager       networkManager      { get; private set; }

    public EventDispatcher      notificationDispatcher { get; private set;}

    public DiContainer          diContainer { get; private set; }
    
    private GameObject _singleGameObject;

    private static object _lock = new object();

    private static bool applicationIsQuitting = false;
    private static Singleton _instance = null;
    

    public Singleton(DiContainer container, GameStateMachine<YellowSignState> gsMachine)
    {
        diContainer = container;
        gameStateMachine = gsMachine;
        
        _initialize();
        _instance = this;
    }

    public void Initialize()
    {
        gameStateMachine.ChangeState(gameConfig.initalState);
    }

    public void Tick()
    {
        gameStateMachine.Step(Time.deltaTime);
        gui.Step(Time.deltaTime);
    }

    public static Singleton instance
    {
        get
        {

            if(applicationIsQuitting)
            {
                Debug.LogWarning("[Singleton] Instance " +
                    "already destroyed on application quit." +
                    " Won't create again - returning null.");
                return null;
            }

            if(_instance == null)
            {
                    Debug.LogError("[Singleton] Something went really wrong " +
                        " - there should never be more than 1 singleton!" +
                        " Reopenning the scene might fix it.");
                    return _instance;          
            }
            return _instance;
        }
    }

    private void _initialize()
    {
        Debug.Log("Booted");

        _singleGameObject = new GameObject();
        _singleGameObject.name = "(singleton)";
        GameObject.DontDestroyOnLoad(_singleGameObject);

        gameConfig = Resources.Load<GameConfig>("GameConfig");

        sessionFlags = new SessionFlags();
        notificationDispatcher = new EventDispatcher();

        networkManager = _singleGameObject.AddComponent<NetworkManager>();
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

    public void LateDispose()
    {
        applicationIsQuitting = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using GhostGen;


public class Singleton : MonoBehaviour 
{

    public GameStateMachine     gameStateMachine    { get; private set; }
    public SessionFlags         sessionFlags        { get; private set; }

    public GuiManager           gui                 { get { return _guiManager; } }
    public GameplayResources    gameplayResources   { get { return _gameplayResources; } }
    public NetworkManager       networkManager      { get; private set; }

    public NotificationDispatcher notificationDispatcher { get; private set; }


    [SerializeField]
    private GuiManager _guiManager;

    [SerializeField]
    private GameplayResources _gameplayResources;

    private IStateFactory _stateFactory;


    public void Awake()
    {
        _instance = this;
        GameObject.DontDestroyOnLoad(_instance);

        _stateFactory = new YellowSignStateFactory();
        gameStateMachine = new GameStateMachine(_stateFactory);

        sessionFlags = new SessionFlags();

        networkManager = gameObject.AddComponent<NetworkManager>();
        notificationDispatcher = new NotificationDispatcher();

        Input.multiTouchEnabled = false; //This needs to go elsewere 
    }

    public void Start()
    {
        _postInit();

        gameStateMachine.ChangeState(YellowSignState.INTRO);
    }


    public void Update()
    {
        gameStateMachine.Step(Time.deltaTime);
        gui.Step(Time.deltaTime);
    }

    private static Singleton _instance = null;
    public static Singleton instance
    {
        get
        {
            return _instance;
        }
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
}

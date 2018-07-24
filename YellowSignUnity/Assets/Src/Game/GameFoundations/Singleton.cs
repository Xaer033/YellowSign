using System.Collections;
using System.Collections.Generic;
using Zenject;
using UnityEngine;
using GhostGen;


public class Singleton : IInitializable, ILateDisposable
{
    public GameConfig           gameConfig          { get; private set; }
    public SessionFlags         sessionFlags        { get; private set; }
    
    public GameStateMachine<YellowSignStateType> gameStateMachine    { get; private set; }

    public GuiManager           gui                 { get; private set; }
    public NetworkManager       networkManager      { get; private set; }


    //[Inject(Id = GameInstaller.GLOBAL_DISPATCHER)]
    public IEventDispatcher     notificationDispatcher { get; private set;}

    public DiContainer          diContainer { get; private set; }
    
    private GameObject _singleGameObject;

    private static object _lock = new object();
    private static bool applicationIsQuitting = false;
    private static Singleton _instance = null;
    

    public Singleton(
        DiContainer container,
        GameStateMachine<YellowSignStateType> gsMachine,
        SessionFlags pSessionFlags,
        [Inject(Id = GameInstaller.GLOBAL_DISPATCHER)]
        IEventDispatcher eventDispatcher,
        GameConfig pGameConfig,
        NetworkManager pNetworkManager,
        GuiManager guiManager)
    {
        diContainer = container;
        sessionFlags = pSessionFlags;
        gameConfig = pGameConfig;
        gameStateMachine = gsMachine;
        notificationDispatcher = eventDispatcher;
        networkManager = pNetworkManager;
        gui = guiManager;

        _initialize();
        _instance = this;
    }

    public void Initialize()
    {
        YellowSignStateType initialState = gameConfig.initialState;
        gameStateMachine.ChangeState(initialState);
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
        _singleGameObject = new GameObject();
        _singleGameObject.name = "(singleton)";
        GameObject.DontDestroyOnLoad(_singleGameObject);
        
        Input.multiTouchEnabled = false; //This needs to go elsewere         
    }

    public void LateDispose()
    {
        applicationIsQuitting = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GhostGen;


[CreateAssetMenu(menuName = "GhostGen/Game Config")]
public class GameConfig : ScriptableObject, IPostInit
{
    public YellowSignState initalState;

    public GuiManager guiManager;
    public GameplayResources gameplayResources;

    public void PostInit()
    {

    }
}

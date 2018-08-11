using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName ="YellowSign/Gameplay Resources")]
public class GameplayResources : ScriptableObject
{
    public GameObject gameplayCamera;

    public CreepHealthUIView creepHealthUIPrefab;
    public TowerHighlighter highlighterPrefab;
}

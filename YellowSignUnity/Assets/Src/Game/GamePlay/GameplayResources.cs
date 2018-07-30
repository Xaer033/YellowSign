using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName ="YellowSign/Gameplay Resources")]
public class GameplayResources : ScriptableObject
{
    public GameObject basicCreep;
    public CreepStats basicCreepStats;
    public GameObject gameplayCamera;

    public GameObject highlighterPrefab;
}

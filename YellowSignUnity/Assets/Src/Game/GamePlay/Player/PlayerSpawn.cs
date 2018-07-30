using UnityEngine;

public enum PlayerNumber : byte
{
    P1 = 1,
    P2,
    P3,
    P4
}

public class PlayerSpawn : MonoBehaviour
{
    public PlayerNumber playerNumber;
    public Transform cameraHook;
}

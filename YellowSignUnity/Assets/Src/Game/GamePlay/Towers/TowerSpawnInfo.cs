using TrueSync;

public struct TowerSpawnInfo
{
    public static TowerSpawnInfo Create(
       byte ownerId,
       TSVector position,
       TSQuaternion rotation)
    {
        TowerSpawnInfo spawnInfo = new TowerSpawnInfo();
        spawnInfo.ownerId = ownerId;
        spawnInfo.position = position;
        spawnInfo.rotation = rotation;
        return spawnInfo;
    }

    public byte ownerId;
    public TSVector position;
    public TSQuaternion rotation;
}

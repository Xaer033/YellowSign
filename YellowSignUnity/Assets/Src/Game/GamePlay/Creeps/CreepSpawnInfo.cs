using TrueSync;

public struct CreepSpawnInfo
{
    public byte         ownerId;
    public byte         targetOwnerId;
    public TSVector     targetPosition;
    public TSVector     position;
    public TSQuaternion rotation;

    public static CreepSpawnInfo Create(
        byte ownerId, 
        TSVector position, 
        TSQuaternion rotation, 
        byte targetOwnerId, 
        TSVector targetPosition)
    {
        CreepSpawnInfo spawnInfo = new CreepSpawnInfo();
        spawnInfo.ownerId = ownerId;
        spawnInfo.position = position;
        spawnInfo.rotation = rotation;
        spawnInfo.targetOwnerId = targetOwnerId;
        spawnInfo.targetPosition = targetPosition;
        return spawnInfo;
    }
}

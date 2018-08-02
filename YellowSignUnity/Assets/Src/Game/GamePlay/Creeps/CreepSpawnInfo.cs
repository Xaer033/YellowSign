using TrueSync;

public struct CreepSpawnInfo
{
    //public CreepSpawnInfo()
    //{
    //    ownerId = 0;
    //    targetOwnerId = 0;
    //    position = TSVector.zero;
    //    rotation = TSQuaternion.identity;
    //    targetPosition = TSVector.zero;
    //}

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

    public byte         ownerId;
    public byte         targetOwnerId;
    public TSVector     targetPosition;
    public TSVector     position;
    public TSQuaternion rotation;
}

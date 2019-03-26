using System.Collections.Generic;

public class GameState
{
    public List<Tower> towerList = new List<Tower>(200);
    public List<Creep> creepList = new List<Creep>(200);

    public string GetMd5()
    {
        List<byte> resultList = new List<byte>();

        for(int i = 0; i < towerList.Count; ++i)
        {
            Tower t = towerList[i];
            byte[] towerByte = StateChecker.ToByteArray(t.state);
            resultList.AddRange(towerByte);
        }

        for(int i = 0; i < creepList.Count; ++i)
        {
            Creep c = creepList[i];
            byte[] creepByte = StateChecker.ToByteArray(c.state);
            resultList.AddRange(creepByte);
        }

        byte[] result = resultList.ToArray();
        return StateChecker.GetMd5Hash(result);
    }
}

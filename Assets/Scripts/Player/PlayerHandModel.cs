using System.Collections.Generic;

// 이 클래스는 MonoBehaviour가 아닙니다.
public class PlayerHandModel
{
    // Key: 유닛의 원본 데이터(ScriptableObject)
    // Value: 해당 유닛의 보유 개수
    public Dictionary<UnitData, int> ReservedUnits { get; private set; } = new Dictionary<UnitData, int>();

    public void AddUnit(UnitData unitData)
    {
        if (ReservedUnits.ContainsKey(unitData))
        {
            ReservedUnits[unitData]++;
        }
        else
        {
            ReservedUnits[unitData] = 1;
        }
    }

    public void RemoveUnit(UnitData unitData)
    {
        if (ReservedUnits.ContainsKey(unitData))
        {
            ReservedUnits[unitData]--;
            if (ReservedUnits[unitData] <= 0)
            {
                ReservedUnits.Remove(unitData);
            }
        }
    }
}
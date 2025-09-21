using System;

public enum UnitPosition
{
    OnBoard,
    InReserved,
    InPool
}

[Serializable]
public class UnitModel
{
    public UnitData Data { get; private set; }

    public readonly string uniqueId;       // 모든 유닛을 구분할 고유 ID
    public string membership;        // 이 유닛을 소유한 플레이어 또는 정부
    public UnitPosition position;
    public string locationId;            // 위치에 대한 구체적인 정보 (도시 이름 또는 플레이어 ID)

    private static int nextId = 0;

    public UnitModel(UnitData data)
    {
        Data = data; // 원본 데이터 참조 연결
        uniqueId = $"{data.unitName}_{nextId++}";
        locationId = data.defaultSpawnPosition.ToString();

        membership = data.spawnMembership;
        position = data.defaultSpawnPosition;
    }
}
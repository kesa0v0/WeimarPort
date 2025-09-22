using System;

public enum UnitPosition
{
    OnBoard,
    InReserved,
    InPool,
    Disposed
}

[Serializable]
public class UnitModel
{
    public UnitData Data { get; private set; }

    public readonly string uniqueId;       // 모든 유닛을 구분할 고유 ID
    public string membership;        // 이 유닛을 소유한 플레이어 또는 정부
    public UnitPosition position;
    public string locationId;            // 위치에 대한 구체적인 정보 (도시 이름 또는 플레이어 ID)

    public UnitModel(UnitData data)
    {
        Data = data; // 원본 데이터 참조 연결
        // 자동 생성되는 인스턴스 고유 ID
        uniqueId = Guid.NewGuid().ToString("N");
        locationId = data.defaultSpawnPosition.ToString();

        membership = data.spawnMembership;
        position = data.defaultSpawnPosition;
    }

    public UnitModel(UnitData data, string membership, UnitPosition position, string locationId)
    {
        Data = data;
        uniqueId = Guid.NewGuid().ToString("N");
        this.membership = string.IsNullOrEmpty(membership) ? data.spawnMembership : membership;
        this.position = position;
        this.locationId = string.IsNullOrEmpty(locationId) ? data.defaultLocationId : locationId;
    }

    // 저장/시나리오에서 고정 인스턴스 ID를 지정하고 싶을 때 사용하는 생성자
    public UnitModel(UnitData data, string instanceId, string membership, UnitPosition position, string locationId)
    {
        Data = data;
        uniqueId = string.IsNullOrEmpty(instanceId) ? Guid.NewGuid().ToString("N") : instanceId;
        this.membership = string.IsNullOrEmpty(membership) ? data.spawnMembership : membership;
        this.position = position;
        this.locationId = string.IsNullOrEmpty(locationId) ? data.defaultLocationId : locationId;
    }
}
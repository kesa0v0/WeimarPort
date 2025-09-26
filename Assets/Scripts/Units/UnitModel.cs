using System;

public enum UnitLocationState
{
    Unavailable,
    InSupply,
    OnBoard,
    Disposed
}

[Serializable]
public class UnitModel
{
    public UnitData Data { get; private set; }

    public string InstanceId;       // 유닛의 고유 인스턴스 ID (e.g., "kpd_s_1")
    public string DataId;           // 원본 데이터 ID (e.g., "UnitData_KPD_Soldiers")
    
    public UnitLocationState CurrentState;
    public LocationData CurrentLocation;        // 현재 유닛의 정확한 위치 정보
    
    public FactionType ControllerPartyId;


    public UnitModel(UnitData data)
    {
        Data = data; // 원본 데이터 참조 연결
        // 자동 생성되는 인스턴스 고유 ID
        InstanceId = Guid.NewGuid().ToString("N");
    }

    public UnitModel(UnitData data, FactionType controllerPartyId, UnitLocationState position, LocationData location)
    {
        Data = data;
        InstanceId = Guid.NewGuid().ToString("N");
        this.CurrentState = position;
        this.CurrentLocation = location;
        this.ControllerPartyId = controllerPartyId;
    }

    // 저장/시나리오에서 고정 인스턴스 ID를 지정하고 싶을 때 사용하는 생성자
    public UnitModel(UnitData data, string instanceId, FactionType controllerPartyId, UnitLocationState position, LocationData location)
    {
        Data = data;
        InstanceId = string.IsNullOrEmpty(instanceId) ? Guid.NewGuid().ToString("N") : instanceId;
        this.CurrentState = position;
        this.CurrentLocation = location;
        this.ControllerPartyId = controllerPartyId;
    }
}
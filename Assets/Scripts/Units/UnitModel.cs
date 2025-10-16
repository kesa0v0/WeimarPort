using System;

[Serializable]
public class UnitModel
{
    [NonSerialized]
    public UnitData Data;

    public string InstanceId;       // 유닛의 고유 인스턴스 ID (e.g., "kpd_s_1")
    public string DataId;           // 원본 데이터 ID (e.g., "UnitData_KPD_Soldiers")

    public LocationType CurrentState;
    public LocationData CurrentLocation;        // 현재 유닛의 정확한 위치 정보

    public FactionType ControllerPartyId;


}
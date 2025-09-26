using System.Collections.Generic;

// JSON 파일의 최상위 구조와 일치
[System.Serializable]
public class ScenarioData
{
    public string scenarioName;
    public string description;
    public List<SetupInstruction> setupScript;
}

// "command"와 "args"를 담는 명령어 단위 클래스
[System.Serializable]
public class SetupInstruction
{
    public string command;
    public Arguments args;
}

// 명령어에 필요한 상세 인자들
[System.Serializable]
public class Arguments
{
    // 대상 식별용 ID
    public string partyId;         // 액션의 주체 또는 대상이 되는 정당 ID (e.g., "SPD")
    public string targetPartyId;   // 액션의 두 번째 대상이 되는 정당 ID
    public string instanceId;      // 유닛, 마커 등 특정 인스턴스의 고유 ID
    public string dataId;          // 생성할 대상의 원본 데이터 ID (e.g., "UnitData_KPD_Soldiers", "Threat_Poverty")

    // 수량 및 값
    public int count;             // 생성하거나 제거할 개수
    public int value;             // VP, 예비 점수, 트랙 이동 값 등 숫자 값

    // 위치 정보
    public LocationInfo location;  // 대상이 배치되거나 효과가 적용될 위치

    // 기타 특정 타입
    public string flagType;        // 외교 관계 깃발 타입 (e.g., "USA_UK", "France")
}

// 위치 정보를 담는 클래스
[System.Serializable]
public class LocationInfo
{
    public string type;
    public string name;
    public LocationParams @params; // 'params'는 C# 예약어이므로 '@'를 붙여 사용
}

[System.Serializable]
public class LocationParams
{
    public bool unique;
}
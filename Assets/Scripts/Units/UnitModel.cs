// 유닛이 어디에 있는지 명확히 하기 위한 enum
public enum UnitLocation
{
    OnBoard,       // 게임 보드 위 (특정 도시에 속함)
    InPlayerHand,  // 플레이어의 손 (아직 배치되지 않음)
    InPool         // 사용 가능한 유닛 풀 (아직 아무도 소유하지 않음)
}

public class UnitModel
{
    public readonly int uniqueId;       // 모든 유닛을 구분할 고유 ID
    public readonly string unitType;   // 유닛 종류 (예: "Freikorps", "Reichswehr")
    public Party ownerParty;
    
    public UnitLocation currentLocation; // 현재 유닛의 위치
    public string locationId;            // 위치에 대한 구체적인 정보 (도시 이름 또는 플레이어 ID)

    private static int nextId = 0;

    public UnitModel(string type)
    {
        uniqueId = nextId++;
        unitType = type;
        currentLocation = UnitLocation.InPool; // 기본값은 풀
    }
}
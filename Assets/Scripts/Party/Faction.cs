using UnityEngine;

public enum FactionType
{
    None,
    KPD,
    SPD,
    Z,
    DNVP,
    Government
}

[CreateAssetMenu(fileName = "NewFactionData", menuName = "Weimar/Faction Data")]
public class FactionData : ScriptableObject
{
    public FactionType factionType; // 빠른 구분을 위한 enum (예: Faction.KPD)

    [Header("기본 정보")]
    public string factionName = "정당 이름";
    public Color factionColor = Color.white;
    public Sprite factionLogo;

    [Header("고유 게임 규칙")]
    public int coupActionCost = 4; // 쿠데타 액션 비용 (KPD는 3, 나머지는 4)
    // 여기에 각 정당의 고유한 승리 조건, 특별 능력 등
    // 다양한 데이터를 추가할 수 있습니다.
    // 예: public VictoryCondition victoryCondition;
}
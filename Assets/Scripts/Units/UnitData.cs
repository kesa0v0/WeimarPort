using UnityEngine;

[CreateAssetMenu(fileName = "New UnitData", menuName = "Weimar/Unit Data")]
public class UnitData : ScriptableObject
{
    [Header("기본 정보")]
    public string unitName; // "자유군단", "국가방위군" 등
    public Texture2D unitIcon; // UI에 표시될 아이콘
    public Material unitMaterial; // 유닛의 외형을 나타내는 머티리얼
    // Deprecated: 런타임 초기화는 시나리오/불러오기 데이터(UnitSpawnSpec)로 처리합니다.
    public UnitPosition defaultSpawnPosition; // (Deprecated) 기본 생성 위치
    public string defaultLocationId; // (Deprecated) 기본 생성 위치 ID (도시 이름 또는 플레이어 ID)
    public string spawnMembership; // (Deprecated) 생성시 소속 (KPD, SPD 등)

    [Header("게임 능력치")]
    public int combatStrength; // 전투력
}
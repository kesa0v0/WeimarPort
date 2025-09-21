using UnityEngine;

[CreateAssetMenu(fileName = "New UnitData", menuName = "Weimar/Unit Data")]
public class UnitData : ScriptableObject
{
    [Header("기본 정보")]
    public string unitName; // "자유군단", "국가방위군" 등
    public Sprite unitIcon; // UI에 표시될 아이콘
    public Material unitMaterial; // 유닛의 외형을 나타내는 머티리얼
    public UnitPosition defaultSpawnPosition; // 기본 생성 위치

    [Header("게임 능력치")]
    public int combatStrength; // 전투력
    public string spawnMembership;
}
using UnityEngine;

[CreateAssetMenu(fileName = "New UnitData", menuName = "Weimar/Unit Data")]
public class UnitData : ScriptableObject
{
    [Header("기본 정보")]
    public string unitName; // "자유군단", "국가방위군" 등
    public Material unitMaterial; // 유닛의 외형을 나타내는 머티리얼
    public int combatStrength; // 전투력
    public FactionType initialAffiliation;  

    [Header("시각 정보")]
    public Sprite unitSprite; // 유닛의 기본 이미지
    public Sprite flippedSprite; // DNVP가 장악한 Reichswehr 유닛 이미지

    [Header("게임 규칙")]
    public bool isGovernmentUnit = false; // 정부 유닛 여부
    public bool canBeFlipped = false; // DNVP가 장악 가능한 Reichswehr 유닛인지 여부
}
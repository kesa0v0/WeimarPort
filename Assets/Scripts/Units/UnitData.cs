using UnityEngine;

[CreateAssetMenu(fileName = "New UnitData", menuName = "Weimar/Unit Data")]
public class UnitData : ScriptableObject
{
    [Header("기본 정보")]
    public string DataId; // 예: "UnitData_KPD_Soldiers"
    public string UnitName; // "자유군단", "국가방위군" 
    [TextArea] public string Description; // 유닛에 대한 설명
    
    [Header("게임 규칙")]
    public FactionType Affiliation;  // 이 유닛의 기본 소속 정당
    [Range(1, 2)] public int Strength; // 전투력
    public bool IsGovernmentUnit = false; // 정부 유닛 여부
    public bool IsFlippableReichswehr = false; // DNVP가 장악 가능한 Reichswehr 유닛인지 여부

    [Header("시각 정보")]
    public GameObject ModelPrefab; // 이 유닛을 표현할 3D 모델 프리팹
    public Material DefaultMaterial; // 유닛의 외형을 나타내는 
    public Material FlippedMaterial; // DNVP가 장악했을 때의 머티리얼 (Reichswehr용)
    public Sprite Icon; // 유닛의 기본 이미지
}

using UnityEngine;


/// <summary>
/// 위협 마커의 변하지 않는 원본 데이터를 정의하는 ScriptableObject입니다.
/// (룰북 p. 24, 29 참고)
/// </summary>
[CreateAssetMenu(fileName = "ThreatData_", menuName = "Weimar/Threat Marker Data")]
public class ThreatMarkerData : ScriptableObject
{
    // 마커의 종류를 더 명확하게 구분하기 위한 enum
    public enum MarkerCategory
    {
        OneSidedThreat,      // 일반적인 단면 위협 마커 (불안, 봉쇄 등)
        TwoSidedPoverty,     // 빈곤/번영 양면 마커
        PartySpecificThreat  // 정당 전용 위협 마커 (체제, 평의회)
    }
    
    [Header("기본 정보")]
    public string DataId; // 예: "Threat_Poverty", "Threat_Councils"
    public MarkerCategory Category = MarkerCategory.OneSidedThreat;

    [Header("위협(기본) 면 정보")]
    public string ThreatName; // 예: "Poverty", "Unrest"
    public Material ThreatMaterial;
    public Sprite ThreatIcon;

    [Header("번영(뒷면) 정보 (TwoSidedPoverty일 경우)")]
    public string ProsperityName;
    public Material ProsperityMaterial;
    public Sprite ProsperityIcon;
    
    [Header("비활성 면 정보 (PartySpecificThreat일 경우)")]
    public string InactiveName;
    public Material InactiveMaterial;
    public Sprite InactiveIcon;

    [Header("정당 연관 정보 (PartySpecificThreat일 경우)")]
    // 이 마커가 특정 정당과 연관되어 있는지 여부 (체제: DNVP, 평의회: KPD)
    public FactionData AssociatedParty; 
    
    [Header("시각적 에셋")]
    public GameObject ModelPrefab; // 마커를 표현할 3D 모델 프리팹
}
using UnityEngine;

/// <summary>
/// 위협 마커의 시각적 표현(3D 모델)을 제어합니다.
/// </summary>
public class MarkerView : MonoBehaviour
{
    // Inspector에서 마커의 3D 모델 렌더러를 연결해주세요.
    [SerializeField] private Renderer markerRenderer;

    public string InstanceId { get; private set; }
    private ThreatMarkerData markerData; // 원본 데이터 참조

    /// <summary>
    /// 이 View를 초기화하고, 모델 데이터에 따라 외형을 설정합니다.
    /// </summary>
    public void Initialize(ThreatMarkerModel model)
    {
        this.InstanceId = model.InstanceId;
        this.markerData = model.Data; // 원본 데이터(SO) 저장

        if (markerData.ModelPrefab != null)
        {
            // TODO: 만약 프리팹을 동적으로 생성해야 한다면, 여기서 생성하고 렌더러를 할당
        }

        UpdateAppearance(model.IsFlipped); // 모델의 현재 상태에 따라 초기 외형 설정
    }

    /// <summary>
    /// 마커의 상태(뒤집혔는지 여부)에 따라 머티리얼을 변경합니다.
    /// </summary>
    public void UpdateAppearance(bool isFlipped)
    {
        if (markerRenderer == null || markerData == null) return;

        // 원본 데이터(markerData)의 카테고리에 따라 다른 로직을 적용
        switch (markerData.Category)
        {
            case ThreatMarkerData.MarkerCategory.OneSidedThreat:
                markerRenderer.material = markerData.ThreatMaterial;
                break;

            case ThreatMarkerData.MarkerCategory.TwoSidedPoverty:
                // isFlipped가 true이면 번영(Prosperity) 머티리얼, 아니면 활성(Poverty) 머티리얼
                markerRenderer.material = isFlipped ? markerData.ProsperityMaterial : markerData.ThreatMaterial;
                break;

            case ThreatMarkerData.MarkerCategory.PartySpecificThreat:
                // isFlipped가 true이면 비활성(Inactive) 머티리얼, 아니면 활성(Threat) 머티리얼
                markerRenderer.material = isFlipped ? markerData.InactiveMaterial : markerData.ThreatMaterial;
                break;
        }
    }
}

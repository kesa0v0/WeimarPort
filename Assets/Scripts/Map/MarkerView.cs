using UnityEngine;

/// <summary>
/// 위협 마커의 시각적 표현(3D 모델)을 제어합니다.
/// </summary>
public class MarkerView : MonoBehaviour
{
    // Inspector에서 마커의 3D 모델 렌더러를 연결해주세요.
    [SerializeField] private Renderer markerRenderer;
    
    public string InstanceId { get; private set; }

    /// <summary>
    /// 마커 데이터에 따라 외형(텍스처/머티리얼)을 설정합니다.
    /// </summary>
    public void Initialize(string instanceId, ThreatMarkerData markerData )
    {
        this.InstanceId = instanceId;

        // TODO: markerData의 종류(Poverty, Unrest 등)에 따라 다른 머티리얼을 설정
        // 예: markerRenderer.material = markerData.material;
    }
}

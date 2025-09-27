using UnityEngine;
using UnityEngine.UI;


public class UnitView : MonoBehaviour
{
    public string InstanceId { get; private set; }
    private UnitData unitData; // 원본 데이터 참조

    [Header("UI Components")]
    [SerializeField] private Renderer unitRenderer;
    [SerializeField] private Image unitIcon;
    [SerializeField] private MeshRenderer highlightRenderer;
    

    /// <summary>
    /// 이 View를 초기화하고, 모델 데이터에 따라 외형을 설정합니다.
    /// </summary>
    public void Initialize(string instanceId, UnitData unitData, FactionType initialControllerData)
    {
        this.InstanceId = instanceId;
        this.unitData = unitData; // 원본 데이터 저장
        
        // TODO: UnitData에 텍스처/머티리얼 정보가 있다면 그것을 사용해 렌더러의 머티리얼을 설정
        // 예: unitRenderer.material = unitData.defaultMaterial;

        UpdateAppearance(initialControllerData); // 초기 외형 설정
    }

    /// <summary>
    /// 유닛의 제어권자 변경에 따라 외형을 업데이트합니다. (예: Reichswehr)
    /// 이 메서드는 이제 이벤트에 의해 내부적으로 호출됩니다.
    /// </summary>
    private void UpdateAppearance(FactionType currentController)
    {
        if (unitRenderer == null || unitData == null) return;

        // DNVP가 장악한 Reichswehr인 경우 FlippedMaterial 사용
        if (unitData.IsFlippableReichswehr && currentController == FactionType.DNVP)
        {
            unitRenderer.material = unitData.FlippedMaterial;
        }
        else
        {
            unitRenderer.material = unitData.DefaultMaterial;
        }
    }
}

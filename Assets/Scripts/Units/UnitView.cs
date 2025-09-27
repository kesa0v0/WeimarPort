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

    // 이 오브젝트가 활성화될 때 이벤트 버스에 리스너를 등록합니다.
    private void OnEnable()
    {
        // EventBus와 UnitControllerChangedEvent 같은 이벤트 데이터 구조가 있다고 가정합니다.
        // 이벤트를 수신하면 HandleUnitControllerChanged 메서드가 호출됩니다.
        // 예: EventBus.OnUnitControllerChanged += HandleUnitControllerChanged;
    }

    // 이 오브젝트가 비활성화될 때 이벤트 버스에서 리스너를 해제합니다.
    private void OnDisable()
    {
        // 예: EventBus.OnUnitControllerChanged -= HandleUnitControllerChanged;
    }
    
    // 유닛 제어권자 변경 이벤트를 처리하는 핸들러 메서드
    private void HandleUnitControllerChanged(/* UnitControllerChangedEvent evt */)
    {
        // 이 이벤트가 자신에게 해당하는지 확인합니다.
        // if (evt.InstanceId == this.InstanceId)
        // {
        //     // 이벤트에 담겨온 새로운 제어권자 데이터로 외형을 업데이트합니다.
        //     UpdateAppearance(evt.NewControllerData);
        // }
    }

    /// <summary>
    /// 유닛의 제어권자 변경에 따라 외형을 업데이트합니다. (예: Reichswehr)
    /// 이 메서드는 이제 이벤트에 의해 내부적으로 호출됩니다.
    /// </summary>
    private void UpdateAppearance(FactionType factionType)
    {
        // TODO: DNVP가 제어하는 Reichswehr일 경우 다른 머티리얼/텍스처로 변경하는 로직
        // 예: if (unitData.isReichswehr && factionType == FactionType.DNVP) { ... }
    }

    // 유저가 이 유닛을 클릭했을 때의 처리
    void OnMouseDown()
    {
        Debug.Log($"Unit Clicked: {InstanceId}");
        // TODO: GameManager나 InputManager에 클릭 이벤트를 전달하는 로직
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // 클릭 이벤트를 위해 필요
using DG.Tweening; // DOTween 네임스페이스

// IPointerClickHandler: UI 요소에 대한 클릭을 감지하는 인터페이스
public class UnitView : MonoBehaviour, IPointerClickHandler
{
    [Header("UI Components")]
    [SerializeField] private Image unitIcon;         // 유닛의 종류를 나타내는 아이콘 (예: 병사, 탱크)
    [SerializeField] private Image baseColorImage;   // 유닛의 소속(정당)을 나타내는 배경/베이스 색상
    [SerializeField] private GameObject selectionOutline; // 유닛이 선택되었을 때 켤 외곽선

    private UnitPresenter presenter;

    /// <summary>
    /// Factory나 Manager가 이 View를 생성한 직후 호출하여 초기화합니다.
    /// </summary>
    /// <param name="presenter">이 View를 제어할 Presenter입니다.</param>
    public void Initialize(UnitPresenter presenter)
    {
        this.presenter = presenter;

        // Presenter로부터 Model 데이터를 받아와서 초기 비주얼 설정
        SetVisuals(presenter.Model.Data.unitName, presenter.Model.ownerParty.partyColor);
        
        // 초기 선택 상태는 꺼둠
        ShowAsSelected(false);
    }

    /// <summary>
    /// 유닛의 아이콘과 색상을 설정합니다.
    /// </summary>
    public void SetVisuals(string unitType, Color partyColor)
    {
        // 리소스 폴더나 다른 에셋 관리 시스템에서 유닛 타입에 맞는 스프라이트를 불러옵니다.
        // unitIcon.sprite = ResourceManager.GetUnitSprite(unitType); 
        baseColorImage.color = partyColor;
    }

    /// <summary>
    /// 지정된 위치로 부드럽게 이동시킵니다.
    /// </summary>
    /// <param name="targetPosition">목표 월드 좌표</param>
    /// <param name="duration">이동에 걸리는 시간</param>
    public void MoveTo(Vector3 targetPosition, float duration)
    {
        // DOTween을 사용하여 부드러운 이동 애니메이션 실행
        transform.DOMove(targetPosition, duration).SetEase(Ease.OutQuad);
    }

    /// <summary>
    /// 선택되었을 때의 시각적 피드백을 켜고 끕니다.
    /// </summary>
    public void ShowAsSelected(bool isSelected)
    {
        if (selectionOutline != null)
        {
            selectionOutline.SetActive(isSelected);
        }
    }

    /// <summary>
    /// 이 유닛 View가 클릭되었을 때 호출됩니다. (IPointerClickHandler 인터페이스 구현)
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        // View는 스스로 판단하지 않고, 클릭되었다는 사실을 Presenter에게 '보고'만 합니다.
        Debug.Log($"View of unit '{presenter.Model.Data.unitName}' was clicked.");
        presenter?.OnViewClicked();
    }
}
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // 클릭 이벤트를 위해 필요
using DG.Tweening; // DOTween 네임스페이스

// UI 전용 유닛 View
public class UnitUIView : BaseUnitView, IPointerClickHandler
{
    [Header("UI Components")]
    [SerializeField] private Image unitIcon;         // 유닛의 종류를 나타내는 아이콘 (예: 병사, 탱크)
    [SerializeField] private Image baseColorImage;   // 유닛의 소속(정당)을 나타내는 배경/베이스 색상
    [SerializeField] private GameObject selectionOutline; // 유닛이 선택되었을 때 켤 외곽선

    public override void Initialize(UnitPresenter presenter)
    {
        base.Initialize(presenter);

        // Presenter로부터 Model 데이터를 받아와서 초기 비주얼 설정
        if (string.Equals(presenter.Model.membership, "Government", System.StringComparison.OrdinalIgnoreCase))
        {
            SetVisuals(presenter.Model.Data.unitName, GameManager.Instance.gameState.government.Color);
        }
        else
        {
            var party = PartyRegistry.GetPartyByName(presenter.Model.membership);
            var color = party != null ? party.partyColor : Color.white;
            SetVisuals(presenter.Model.Data.unitName, color);
        }
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

    public override void MoveTo(Vector3 targetPosition, float duration)
    {
        // DOTween을 사용하여 부드러운 이동 애니메이션 실행
        transform.DOMove(targetPosition, duration).SetEase(Ease.OutQuad);
    }

    public override void ShowAsSelected(bool isSelected)
    {
        if (selectionOutline != null)
        {
            selectionOutline.SetActive(isSelected);
        }
    }

    public override void ShowAsCandidate(bool isCandidate)
    {
        if (selectionOutline != null)
        {
            selectionOutline.SetActive(isCandidate);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // View는 스스로 판단하지 않고, 클릭되었다는 사실을 Presenter에게 '보고'만 합니다.
        Debug.Log($"View of unit '{presenter.Model.Data.unitName}' was clicked.");
        presenter?.OnViewClicked();
    }

    public override void AttachToCity(CityPresenter city)
    {
        Transform cityTransform = city.view.GetCityIndicator().transform;
        transform.SetParent(cityTransform, false);
        transform.localPosition = Vector3.zero;
    }
}
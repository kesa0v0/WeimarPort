using UnityEngine;

// 월드(보드)에 표시되는 유닛 View
public class UnitGameView : BaseUnitView
{
    [Header("World Components")]
    [SerializeField] private Renderer highlightRenderer; // 선택 표시용(선택 시 머터리얼 색 변경 등)

    public override void ShowAsSelected(bool isSelected)
    {
        if (highlightRenderer != null)
        {
            highlightRenderer.enabled = isSelected;
        }
    }

    public override void AttachToCity(CityPresenter city)
    {
        // CityView의 cityIndicator를 기준 부모로 사용
        Transform anchor = city.view.GetCityIndicator().transform;
        transform.SetParent(anchor, true);
        // 세부 배치는 CityPresenter/CityView 쪽 레이아웃 로직이 담당
    }
}

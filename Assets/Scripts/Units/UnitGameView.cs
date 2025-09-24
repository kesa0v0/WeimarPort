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

    public override void ShowAsCandidate(bool isCandidate)
    {
        if (highlightRenderer != null)
        {
            highlightRenderer.enabled = isCandidate;
        }
    }

    public override void AttachToCity(CityPresenter city)
    {
        // CityView의 cityIndicator를 기준 부모로 사용
        Transform anchor = city.view.GetCityIndicator().transform;
        // 로컬 좌표계를 사용하여 도시 기준으로 붙이고, 기본 위치는 (0,0,0)
        transform.SetParent(anchor, false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(90, 0, 180); // 원하는 각도
        transform.localScale = ScaleFix.FixScale(transform.parent, new Vector3(0.1f, 0.1f, 0.1f));
    }
}

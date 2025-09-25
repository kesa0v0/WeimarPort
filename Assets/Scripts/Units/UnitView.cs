using UnityEngine;
using UnityEngine.UI;


public class UnitView : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image unitIcon;
    [SerializeField] private Material unitMaterial;
    [SerializeField] private MeshRenderer highlightRenderer;
    protected Unit presenter;

    /// <summary>
    /// Factory나 Manager가 이 View를 생성한 직후 호출하여 초기화합니다.
    /// </summary>
    public void Initialize(Unit presenter)
    {
        this.presenter = presenter;

        // Model에서 아이콘 정보를 가져와서 UI에 반영
        if (unitIcon != null && presenter.Model.Data.unitSprite != null)
        {
            unitIcon.sprite = presenter.Model.Data.unitSprite;
        }
        else
        {
            Debug.LogWarning("UnitView: Unit icon or presenter model icon is null.");
        }

        // 
    }

    /// <summary>
    /// 도시로 부착(부모 설정 등).
    /// </summary>

    public Unit Presenter => presenter;
    public void AttachToCity(City city)
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

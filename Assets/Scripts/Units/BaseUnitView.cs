using UnityEngine;

/// <summary>
/// Unit의 공통 View 베이스 클래스.
/// UI용(UnitUIView)과 월드용(UnitGameView)이 이를 상속받아 구현합니다.
/// </summary>
public abstract class BaseUnitView : MonoBehaviour
{
    protected UnitPresenter presenter;

    /// <summary>
    /// Factory나 Manager가 이 View를 생성한 직후 호출하여 초기화합니다.
    /// </summary>
    public virtual void Initialize(UnitPresenter presenter)
    {
        this.presenter = presenter;
        ShowAsSelected(false);
    }

    /// <summary>
    /// 선택 상태의 시각적 표현을 토글합니다. (서브클래스에서 구현)
    /// </summary>
    public virtual void ShowAsSelected(bool isSelected) { }

    /// <summary>
    /// 후보(선택 가능) 상태의 시각적 표현을 토글합니다. 기본 구현은 선택 효과와 동일하게 동작합니다.
    /// </summary>
    public virtual void ShowAsCandidate(bool isCandidate)
    {
        ShowAsSelected(isCandidate);
    }

    /// <summary>
    /// 위치 이동. 기본은 즉시 이동, 서브클래스에서 애니메이션 등으로 오버라이드 가능.
    /// </summary>
    public virtual void MoveTo(Vector3 targetPosition, float duration)
    {
        transform.position = targetPosition;
    }

    /// <summary>
    /// 도시로 부착(부모 설정 등). 컨텍스트(UI/월드)에 따라 서브클래스에서 구현.
    /// </summary>
    public abstract void AttachToCity(CityPresenter city);

    public UnitPresenter Presenter => presenter;
}

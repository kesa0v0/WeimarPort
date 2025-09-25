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
    }

    /// <summary>
    /// 도시로 부착(부모 설정 등). 컨텍스트(UI/월드)에 따라 서브클래스에서 구현.
    /// </summary>
    public abstract void AttachToCity(CityPresenter city);

    public UnitPresenter Presenter => presenter;
}

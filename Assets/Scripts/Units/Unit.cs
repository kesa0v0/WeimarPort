public class Unit
{
    public UnitModel Model { get; private set; }
    private UnitView view;
    private IUnitContainer currentContainerCache;

    public Unit(UnitModel model, UnitView view)
    {
        Model = model;
        this.view = view;

        if (view == null)
        {
            return;
        }

        // View에게 자신을 알려주고 초기화하도록 지시
        view.Initialize(this);
    }

    // UnitManager 등이 유닛을 이동시킬 때 이 메소드를 호출해준다.
    public void UpdateLocation(IUnitContainer newContainer)
    {
        // 이전 컨테이너에서 유닛 제거
        currentContainerCache?.RemoveUnit(this);

        // 새 컨테이너에 유닛 추가
        newContainer?.AddUnit(this);

        // 캐시 업데이트
        currentContainerCache = newContainer;
    }

    // 런타임 중 View를 생성/교체할 때 사용
    public void BindView(UnitView newView)
    {
        this.view = newView;
        if (this.view != null)
        {
            this.view.Initialize(this);
        }
    }
}
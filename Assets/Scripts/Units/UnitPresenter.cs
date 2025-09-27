public class UnitPresenter
{
    public UnitModel Model { get; private set; }
    public UnitView View;
    private IUnitContainer currentContainerCache;

    public UnitPresenter(UnitModel model, UnitView view)
    {
        Model = model;
        this.View = view;

        if (view == null)
        {
            return;
        }

        // View에게 자신을 알려주고 초기화하도록 지시
        view.Initialize(model.InstanceId, model.Data, model.ControllerPartyId);
    }

    // UnitManager 등이 유닛을 이동시킬 때 이 메소드를 호출해준다.
    public void UpdateLocation(IUnitContainer newContainer)
    {
        // 이전 컨테이너에서 유닛 제거
        currentContainerCache?.RemoveUnit(Model);

        // 새 컨테이너에 유닛 추가
        newContainer?.AddUnit(Model);

        // 캐시 업데이트
        currentContainerCache = newContainer;
    }

    // 런타임 중 View를 생성/교체할 때 사용
    public void BindView(UnitView newView)
    {
        this.View = newView;
        if (this.View != null)
        {
            this.View.Initialize(Model.InstanceId, Model.Data, Model.ControllerPartyId);
        }
    }
}
public class UnitPresenter
{
    public UnitModel Model { get; private set; }
    private UnitView view;
    private IUnitContainer currentContainerCache;

    public UnitPresenter(UnitModel model, UnitView view)
    {
        Model = model;
        this.view = view;
        
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

    // View로부터 클릭되었다는 보고를 받는 메소드
    public void OnViewClicked()
    {
        // 여기에 유닛이 클릭되었을 때의 실제 '로직'이 들어갑니다.
        // 예: 이 유닛을 선택 상태로 만든다.
        // 예: GameManager에게 이 유닛이 선택되었다고 알린다.

        // GameManager.Instance.SelectUnit(this);
        view.ShowAsSelected(true); // View에게 선택된 것처럼 보이게 하라고 지시
    }
}
public class UnitPresenter
{
    public UnitModel Model { get; private set; }
    private UnitView view;

    public UnitPresenter(UnitModel model, UnitView view)
    {
        Model = model;
        this.view = view;
        
        // View에게 자신을 알려주고 초기화하도록 지시
        view.Initialize(this);
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
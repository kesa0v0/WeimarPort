using UnityEngine;

public class UnitFactory
{
    // Resources/Prefabs/UnitUIView_Default 프리팹을 로드해 UI 뷰를 만든다고 가정
    // 필요 시 프로젝트 사양에 맞게 교체하세요.
    internal static BaseUnitView SpawnUnitView(UnitPresenter presenter)
    {
        var prefab = Resources.Load<GameObject>("Prefabs/unitObj");
        if (prefab == null)
        {
            Debug.LogError("UnitFactory: Could not find UI Unit View prefab at Resources/Prefabs/unitObj");
            return null;
        }
        var go = Object.Instantiate(prefab);
        var view = go.GetComponent<BaseUnitView>();
        if (view == null)
        {
            Debug.LogError("UnitFactory: Prefab does not contain a BaseUnitView component.");
            return null;
        }
        presenter.BindView(view);
        return view;
    }
    
}

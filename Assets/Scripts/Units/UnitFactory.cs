using UnityEngine;

public class UnitFactory
{
    // Resources/Prefabs/UnitUIView_Default 프리팹을 로드해 UI 뷰를 만든다고 가정
    // 필요 시 프로젝트 사양에 맞게 교체하세요.
    internal static UnitView SpawnUnitView(Unit presenter)
    {
        var prefab = Resources.Load<GameObject>("Prefabs/unitObj");
        if (prefab == null)
        {
            Debug.LogError("UnitFactory: Could not find UI Unit View prefab at Resources/Prefabs/unitObj");
            return null;
        }
        var go = Object.Instantiate(prefab);
        var view = go.GetComponent<UnitView>();
        if (view == null)
        {
            Debug.LogError("UnitFactory: Prefab does not contain a BaseUnitView component.");
            return null;
        }
        presenter.BindView(view);
        return view;
    }

    // 컨테이너 타입에 따라 적절한 뷰(UI/World)를 생성
    internal static UnitView SpawnUnitViewForContainer(Unit presenter, IUnitContainer container)
    {
        if (container is UnitView)
        {
            // 핸드 UI는 PlayerHandPanel에서 그려주므로 뷰 생성 안 함
            return null;
        }
        else if (container is Government)
        {
            // 정부 보유 유닛도 별도 핸드/패널에서 그리거나 논-월드이므로 뷰 생성 안 함
            return null;
        }
        else if (container is City city)
        {
            // 보드용 3D 뷰 생성
            var worldPrefab = Resources.Load<GameObject>("Prefabs/World/UnitGameView");
            UnitView viewComponent = null;
            GameObject go = null;
            if (worldPrefab != null)
            {
                go = Object.Instantiate(worldPrefab);
                viewComponent = go.GetComponent<UnitView>();
            }
            if (viewComponent == null)
            {
                // 폴백: 기존 경로 사용
                var legacy = Resources.Load<GameObject>("Prefabs/unitObj");
                if (legacy != null)
                {
                    go = Object.Instantiate(legacy);
                    viewComponent = go.GetComponent<UnitView>();
                }
            }
            if (viewComponent == null)
            {
                Debug.LogError("UnitFactory: Could not find suitable world unit view prefab with UnitView.");
                return null;
            }
            // 보드 평면(XZ)에 눕히기: 필요시 각도 조정 (예: -90,0,0)
            if (go != null)
            {
                go.transform.localRotation = Quaternion.Euler(-90, 0, 0);
            }
            presenter.BindView(viewComponent);
            // 도시 앵커에 부착
            // viewComponent.AttachToCity(city);
            return viewComponent;
        }

        // 기본 폴백
        return SpawnUnitView(presenter);
    }
    
}

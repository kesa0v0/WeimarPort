using System;
using System.Collections.Generic;
using UnityEngine;
using IngameDebugConsole;
using System.Linq;
using Unity.VisualScripting;

public class UnitManager : MonoBehaviour
{
    #region Singleton & Fields
    public static UnitManager Instance { get; private set; }

    
    [Header("프리팹 및 데이터 참조")]
    [Tooltip("모든 UnitView 프리팹이 생성될 부모 Transform")]
    [SerializeField] private Transform unitViewParent;

    // --- 내부 데이터베이스 ---
    // 게임에 존재하는 모든 UnitData SO의 원본 저장소
    private Dictionary<string, UnitData> unitDataRegistry = new Dictionary<string, UnitData>();

    // Runtime caches
    private Dictionary<string, UnitModel> modelIdMap = new Dictionary<string, UnitModel>();
    private Dictionary<UnitModel, UnitPresenter> presenterMap = new Dictionary<UnitModel, UnitPresenter>();
    #endregion

    #region Unity Lifecycle
    void Awake()
    {
        // 싱글톤 인스턴스가 이미 있는지 확인
        if (Instance != null && Instance != this)
        {
            // 이미 존재하면, 이 인스턴스는 중복이므로 파괴
            Destroy(gameObject);
            return;
        }
        // 이 인스턴스를 유일한 싱글톤 인스턴스로 설정
        Instance = this;
        AddDebugCommands();
    }
    #endregion


    #region Initialization / Scenario Loading

    public void Initialize()
    {
        Debug.Log("UnitManager 초기화 시작...");
        LoadAllUnitDataFromResources();
        CreateAllUnitInstances();
        Debug.Log("UnitManager 초기화 완료. 총 " + modelIdMap.Count + "개의 유닛 인스턴스 생성됨.");
    }
    
    /// <summary>
    /// "Resources/Data/Units" 폴더에서 모든 UnitData SO를 불러와 레지스트리에 등록합니다.
    /// </summary>
    private void LoadAllUnitDataFromResources()
    {
        UnitData[] allUnitData = Resources.LoadAll<UnitData>("ScriptableObjects/Units");
        foreach (var data in allUnitData)
        {
            if (!unitDataRegistry.ContainsKey(data.DataId))
            {
                unitDataRegistry.Add(data.DataId, data);
            }
            else
            {
                Debug.LogWarning($"중복된 UnitData ID가 발견되었습니다: {data.DataId}");
            }
        }
    }

    /// <summary>
    /// 룰북 규칙에 따라 게임에 필요한 모든 유닛 인스턴스(Model, Presenter, View)를 생성합니다.
    /// (룰북 p. 28 참고)
    /// </summary>
    private void CreateAllUnitInstances()
    {
        // 룰북에 명시된 각 유닛의 총 개수
        // TODO: 이 데이터는 외부 파일(e.g., GameConfig.json)에서 읽어오는 것이 더 좋습니다.
        Dictionary<string, int> unitCounts = new Dictionary<string, int>
        {
            // 정부 유닛
            { "UnitData_Govt_Police", 5 },
            { "UnitData_Govt_Reichswehr_S1", 3 },
            { "UnitData_Govt_Reichswehr_S2", 3 },
            // SPD 유닛
            { "UnitData_SPD_Reichsbanner", 3 },
            // KPD 유닛
            { "UnitData_KPD_Soldiers", 3 },
            { "UnitData_KPD_Workers", 3 },
            { "UnitData_KPD_Spartacist", 3 },
            { "UnitData_KPD_RoteRuhrarmee", 2 },
            { "UnitData_KPD_RoterFront", 3 },
            // DNVP 유닛
            { "UnitData_DNVP_Freikorps_S1", 3 },
            { "UnitData_DNVP_Freikorps_S2", 3 },
            { "UnitData_DNVP_Stahlhelm", 3 }
        };

        foreach (var pair in unitCounts)
        {
            string dataId = pair.Key;
            int count = pair.Value;

            if (!unitDataRegistry.ContainsKey(dataId))
            {
                Debug.LogError($"UnitData를 찾을 수 없습니다: {dataId}");
                continue;
            }

            UnitData data = unitDataRegistry[dataId];

            for (int i = 1; i <= count; i++)
            {
                // 1. 고유 인스턴스 ID 생성 (e.g., "kpd_soldiers_1")
                string instanceId = $"{data.Affiliation.ToString().ToLower()}_{data.DataId.Split('_').Last().ToLower()}_{i}";
                
                // 2. Model, View, Presenter 생성 및 연결
                UnitModel model = new UnitModel()
                {
                    InstanceId = instanceId,
                    DataId = data.DataId,
                    Data = data,
                    CurrentState = UnitLocationState.Unavailable, // 모든 유닛은 비활성 상태로 시작
                    ControllerPartyId = data.Affiliation
                };

                // View 프리팹 인스턴스화
                GameObject viewObject = Instantiate(data.ModelPrefab, unitViewParent);
                UnitView view = viewObject.GetComponent<UnitView>();
                view.gameObject.name = $"UnitView_{instanceId}";
                view.gameObject.SetActive(false); // 초기에는 비활성

                UnitPresenter presenter = new UnitPresenter(model, view);
                
                // 3. 딕셔너리에 등록
                modelIdMap.Add(instanceId, model);
                presenterMap.Add(model, presenter);
            }
        }
    }

    #endregion

    #region Public API

    /// <summary>
    /// 인스턴스 ID로 해당 유닛의 Presenter를 찾습니다.
    /// </summary>
    public UnitPresenter GetPresenter(string instanceId)
    {
        if (modelIdMap.TryGetValue(instanceId, out UnitModel model))
        {
            if (presenterMap.TryGetValue(model, out UnitPresenter presenter))
            {
                return presenter;
            }
        }
        Debug.LogWarning($"Presenter를 찾을 수 없습니다: {instanceId}");
        return null;
    }

    public UnitModel GetModel(string instanceId)
    {
        if (modelIdMap.TryGetValue(instanceId, out UnitModel model))
        {
            return model;
        }
        Debug.LogWarning($"Model를 찾을 수 없습니다: {instanceId}");
        return null;
    }

    /// <summary>
    /// 시나리오/세이브 데이터로 모든 유닛의 상태를 설정(Hydrate)합니다.
    /// 이 메서드는 ScenarioExecutor나 SaveStateLoader에 의해 호출됩니다.
    /// </summary>
    public void HydrateUnitStates(List<UnitModel> unitStates)
    {
        // TODO: 전달받은 unitStates 목록을 기반으로,
        // modelIdMap에서 각 유닛의 Model을 찾아 상태(CurrentState, CurrentLocation 등)를 업데이트하고,
        // 해당 Presenter를 찾아 UpdateView()를 호출하는 로직 구현.
    }

    public void MoveUnitById(string instanceId, string containerId)
    {
        var unit = GetModel(instanceId);
        if (unit == null)
        {
            Debug.LogWarning($"이동하려는 유닛을 찾을 수 없습니다: {instanceId}");
            return;
        }
        if (CityManager.Instance.GetPresenter(containerId) is not IUnitContainer container)
        {
            Debug.LogWarning($"이동하려는 컨테이너를 찾을 수 없습니다: {containerId}");
            return;
        }
        MoveUnit(unit, container);
    }
    public void MoveUnit(UnitModel unit, IUnitContainer newContainer)
    {
        if (presenterMap.TryGetValue(unit, out UnitPresenter presenter))
        {
            presenter.UpdateLocation(newContainer);
        }
        else
        {
            Debug.LogWarning($"이동하려는 유닛의 Presenter를 찾을 수 없습니다: {unit.InstanceId}");
        }
    }

    #endregion


    #region Debug Command Registration
    public void ListUnits()
    {
        Debug.Log("=== Registered UnitData ===");
        foreach (var data in unitDataRegistry.Values)
        {
            Debug.Log($"DataID: {data.DataId}, Affiliation: {data.Affiliation}, ModelPrefab: {data.ModelPrefab}");
        }
        Debug.Log("=== Spawned Units ===");
        foreach (var pair in modelIdMap)
        {
            var model = pair.Value;
            Debug.Log($"ID: {model.InstanceId}, DataID: {model.DataId}, State: {model.CurrentState}, Controller: {model.ControllerPartyId}");
        }
        Debug.Log("=====================");
    }

    private void AddDebugCommands()
    {
        DebugLogConsole.AddCommandInstance("debug.listUnits", "Lists all spawned units with IDs.", "ListUnits", this);
        DebugLogConsole.AddCommandInstance("debug.moveUnit", "Moves a unit to a specified container. Usage: debug.moveUnit <unitInstanceId> <containerId>", "MoveUnitById", this);
    }

    #endregion
}


public interface IUnitContainer
{
    void AddUnit(UnitModel unit);
    void RemoveUnit(UnitModel unit);
    List<UnitModel> GetUnits();
    string GetContainerName();
}

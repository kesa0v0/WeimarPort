using System;
using System.Collections.Generic;
using UnityEngine;
using IngameDebugConsole;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance { get; private set; }

    public List<UnitData> unitDataList; // 에디터에서 할당
    public Dictionary<string, UnitModel> spawnedUnits = new Dictionary<string, UnitModel>();
    public Dictionary<string, UnitPresenter> spawnedPresenter = new Dictionary<string, UnitPresenter>();

    // 화면에 생성된 '시각적 표현' 목록
    // Key: UnitModel의 uniqueId, Value: 생성된 UnitView 게임오브젝트
    public Dictionary<string, BaseUnitView> spawnedUnitViews = new Dictionary<string, BaseUnitView>();

    public void Awake()
    {
        // 간단한 싱글톤 설정
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        AddDebugCommands();
    }

    // 모든 유닛 데이터를 기준으로 최초 스폰/등록 처리
    public void InitializeUnitsFromDataList()
    {
        if (unitDataList == null || unitDataList.Count == 0)
        {
            Debug.Log("UnitManager: unitDataList is empty; nothing to initialize.");
            return;
        }

        foreach (var data in unitDataList)
        {
            if (data == null) continue;
            var model = CreateUnitData(data);
            if (model == null) continue;

            var presenter = GetPresenterByModel(model);

            // 기본 스폰 위치에 따라 컨테이너 결정 및 배치
            switch (data.defaultSpawnPosition)
            {
                case UnitPosition.InPool:
                    // 풀은 실제 컨테이너가 없으므로 아무 컨테이너도 연결하지 않음 (뷰 미생성)
                    break;
                case UnitPosition.InReserved:
                {
                    if (string.Equals(data.defaultLocationId, "Government", StringComparison.OrdinalIgnoreCase))
                    {
                        presenter.Model.position = UnitPosition.InReserved;
                        presenter.Model.locationId = "Government";
                        presenter.Model.membership = "Government";
                        presenter.UpdateLocation(GameManager.Instance.gameState.government);
                        EnsureViewForContainer(presenter, GameManager.Instance.gameState.government);
                    }
                    else
                    {
                        var party = PartyRegistry.GetPartyByName(data.defaultLocationId) as MainParty;
                        if (party == null)
                        {
                            Debug.LogWarning($"Unit {data.unitName} default InReserved location '{data.defaultLocationId}' not found as MainParty.");
                            break;
                        }
                        presenter.Model.position = UnitPosition.InReserved;
                        presenter.Model.locationId = party.partyName;
                        presenter.UpdateLocation(party);

                        EnsureViewForContainer(presenter, party);
                    }
                    break;
                }
                case UnitPosition.OnBoard:
                {
                    var city = CityManager.Instance.GetCity(data.defaultLocationId);
                    if (city == null)
                    {
                        Debug.LogWarning($"Unit {data.unitName} default OnBoard city '{data.defaultLocationId}' not found.");
                        break;
                    }
                    presenter.Model.position = UnitPosition.OnBoard;
                    presenter.Model.locationId = city.model.cityName;
                    presenter.UpdateLocation(city);

                    EnsureViewForContainer(presenter, city);
                    spawnedUnitViews[presenter.Model.uniqueId].AttachToCity(city);
                    break;
                }
                default:
                    break;
            }
        }

        // 플레이어 핸드 UI 새로고침 (있다면)
        TryRedrawLocalPlayerHand();
    }

    private void TryRedrawLocalPlayerHand()
    {
        if (UIManager.Instance != null && UIManager.Instance.playerHandPanel != null && GameManager.Instance != null && GameManager.Instance.gameState != null)
        {
            var local = GameManager.Instance.gameState.playerParty;
            if (local != null)
            {
                UIManager.Instance.playerHandPanel.Redraw(local);
            }
        }
    }

    private void EnsureViewForContainer(UnitPresenter presenter, IUnitContainer container)
    {
        if (presenter == null) return;
        if (spawnedUnitViews.ContainsKey(presenter.Model.uniqueId)) return;

        var newView = UnitFactory.SpawnUnitViewForContainer(presenter, container);
        if (newView != null)
        {
            spawnedUnitViews[presenter.Model.uniqueId] = newView;
        }
    }

    // --- Scenario / Load Init ---

    public void ClearAllUnits()
    {
        foreach (var view in spawnedUnitViews.Values)
        {
            if (view != null) Destroy(view.gameObject);
        }
        spawnedUnitViews.Clear();
        spawnedPresenter.Clear();
        spawnedUnits.Clear();
    }

    public void InitializeUnitsFromSpecs(IEnumerable<UnitSpawnSpec> specs)
    {
        if (specs == null) return;
        foreach (var spec in specs)
        {
            if (spec == null || string.IsNullOrEmpty(spec.unitName)) continue;
            for (int i = 0; i < Mathf.Max(1, spec.count); i++)
            {
                CreateUnitFromSpec(spec);
            }
        }
        TryRedrawLocalPlayerHand();
    }

    public void InitializeUnitsFromJson(string resourcesPath)
    {
        // Resources.Load<TextAsset>(path) expects path without extension and relative to Resources root
        var ta = Resources.Load<TextAsset>(resourcesPath);
        if (ta == null)
        {
            Debug.LogError($"UnitManager: Could not load TextAsset at Resources/{resourcesPath}");
            return;
        }
        try
        {
            var list = JsonUtility.FromJson<UnitSpawnSpecList>(ta.text);
            if (list == null || list.units == null)
            {
                Debug.LogWarning("UnitManager: Parsed empty unit spec list.");
                return;
            }
            ClearAllUnits();
            InitializeUnitsFromSpecs(list.units);
            Debug.Log($"UnitManager: Initialized {list.units.Count} spec lines from JSON.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"UnitManager: Failed to parse unit specs JSON. {ex.Message}");
        }
    }

    private UnitPresenter CreateUnitFromSpec(UnitSpawnSpec spec)
    {
        var data = GetUnitDataByName(spec.unitName);
        if (data == null)
        {
            Debug.LogWarning($"UnitManager: UnitData '{spec.unitName}' not found.");
            return null;
        }
        var model = new UnitModel(data, spec.membership, spec.position, spec.locationId);
        spawnedUnits.Add(model.uniqueId, model);
        var presenter = new UnitPresenter(model, null);
        spawnedPresenter[model.uniqueId] = presenter;

        switch (model.position)
        {
            case UnitPosition.InPool:
                // no container
                break;
            case UnitPosition.InReserved:
            {
                var party = PartyRegistry.GetPartyByName(model.locationId) as MainParty;
                if (party == null)
                {
                    Debug.LogWarning($"UnitManager: Spec InReserved location '{model.locationId}' not found as MainParty.");
                    break;
                }
                presenter.UpdateLocation(party);
                // no UI view, PlayerHandPanel renders icons from party.ContainedUnits
                break;
            }
            case UnitPosition.OnBoard:
            {
                var city = CityManager.Instance.GetCity(model.locationId);
                if (city == null)
                {
                    Debug.LogWarning($"UnitManager: Spec OnBoard city '{model.locationId}' not found.");
                    break;
                }
                presenter.UpdateLocation(city);
                EnsureViewForContainer(presenter, city);
                spawnedUnitViews[model.uniqueId]?.AttachToCity(city);
                break;
            }
        }

        return presenter;
    }


    // --- Public API ---

    public UnitData GetUnitDataByName(string unitName)
    {
        return unitDataList.Find(unit => unit.unitName == unitName);
    }

    // 새로운 유닛 데이터를 생성하고 관제 목록에 추가
    public UnitModel CreateUnitDataByString(string unitName)
    {
        UnitData unitData = GetUnitDataByName(unitName);
        if (unitData == null)
        {
            Debug.LogWarning($"UnitData with name '{unitName}' not found.");
            return null;
        }
        return CreateUnitData(unitData);
    }
    public UnitModel CreateUnitData(UnitData unitData)
    {
        var newUnit = new UnitModel(unitData);
        
        spawnedUnits.Add(newUnit.uniqueId, newUnit);
        
        // Presenter 즉시 생성 및 캐시 (뷰는 필요할 때 생성)
        var presenter = new UnitPresenter(newUnit, null);
        spawnedPresenter[newUnit.uniqueId] = presenter;

        Debug.Log($"Unit created: {newUnit.Data.name} (ID: {newUnit.uniqueId}) with presenter cached");
        return newUnit;
    }

    // 유닛을 플레이어의 손으로 이동시키는 '명령'
    public void MoveUnitToHandByName(string unitId, string playerId)
    {
        UnitPresenter presenter = GetPresenterById(unitId);
        if (presenter == null)
        {
            Debug.LogWarning($"No presenter found for Unit ID: {unitId}. Tip: use 'debug.listUnits' to see available IDs, or 'debug.moveUnitTypeToHand <unitName> <playerId>' to pick by type.");
            return;
        }
        MainParty player = PartyRegistry.GetPartyByName(playerId) as MainParty;
        if (player == null)
        {
            Debug.LogWarning($"No main party found with ID: {playerId}");
            return;
        }
        MoveUnitToHand(presenter, player);
    }

    // 타입(이름) 기준으로 InPool 상태의 유닛을 찾아 손으로 이동
    public void MoveUnitTypeToHand(string unitName, string playerId)
    {
        MainParty player = PartyRegistry.GetPartyByName(playerId) as MainParty;
        if (player == null)
        {
            Debug.LogWarning($"No main party found with ID: {playerId}");
            return;
        }

        // InPool 상태이면서 해당 타입인 첫 번째 유닛 선택
        foreach (var kv in spawnedUnits)
        {
            var model = kv.Value;
            if (model.Data != null && model.Data.unitName == unitName && model.position == UnitPosition.InPool)
            {
                var presenter = GetPresenterByModel(model);
                if (presenter == null)
                {
                    Debug.LogWarning($"Presenter not found for Unit ID: {model.uniqueId}");
                    return;
                }
                MoveUnitToHand(presenter, player);
                Debug.Log($"Moved {model.uniqueId} to {player.partyName}'s hand.");
                return;
            }
        }
        Debug.LogWarning($"No unit in pool found with type '{unitName}'.");
    }

    // 디버그용: 현재 관리 중인 유닛을 나열
    public void ListUnits()
    {
        if (spawnedUnits.Count == 0)
        {
            Debug.Log("No units spawned.");
            return;
        }
        foreach (var kv in spawnedUnits)
        {
            var m = kv.Value;
            string nm = m.Data != null ? m.Data.unitName : "(null)";
            Debug.Log($"{m.uniqueId} | {nm} | pos={m.position} | loc={m.locationId}");
        }
    }

    public void MoveUnitToHand(UnitPresenter unit, MainParty player)
    {
        if (unit.Model.position != UnitPosition.InPool)
        {
            Debug.LogWarning($"Unit {unit.Model.uniqueId} is not in pool and cannot be moved to hand.");
            return;
        }

    // 1. 데이터(Model)의 상태 변경
    unit.Model.locationId = player.partyName;
        unit.Model.position = UnitPosition.InReserved;

        // 1.5. Presenter에 cache update
        unit.UpdateLocation(player);

        // 2. 시각적 표현(View): 손(UI)용 뷰 보장. 기존 월드 뷰가 있다면 제거 후 UI 뷰 생성
        if (spawnedUnitViews.TryGetValue(unit.Model.uniqueId, out var existing))
        {
            if (!(existing is UnitUIView))
            {
                Destroy(existing.gameObject);
                spawnedUnitViews.Remove(unit.Model.uniqueId);
            }
        }
        if (!spawnedUnitViews.ContainsKey(unit.Model.uniqueId))
        {
            var newView = UnitFactory.SpawnUnitViewForContainer(unit, player);
            if (newView != null)
            {
                spawnedUnitViews[unit.Model.uniqueId] = newView;
            }
        }

        // 3. 로컬 플레이어의 경우 핸드 UI 갱신
        TryRedrawLocalPlayerHand();
    }

    // 유닛을 특정 도시로 이동시키는 '명령'
    public void MoveUnitToCityByName(string unitId, string cityName)
    {
        if (!spawnedUnits
.TryGetValue(unitId, out UnitModel unit)) return;
        UnitPresenter presenter = GetPresenterById(unitId);
        if (presenter == null)
        {
            Debug.LogWarning($"No presenter found for Unit ID: {unitId}");
            return;
        }
        CityPresenter city = CityManager.Instance.GetCity(cityName);
        if (city == null)
        {
            Debug.LogWarning($"No city found with name: {cityName}");
            return;
        }

        MoveUnitToCity(presenter, city);
    }

    public void MoveUnitToCity(UnitPresenter unit, CityPresenter city)
    {
        // 1. 데이터(Model)의 상태를 먼저 변경
        unit.Model.position = UnitPosition.OnBoard;
        unit.Model.locationId = city.model.cityName;

        // 1.5. Presenter에 cache update
        unit.UpdateLocation(city);

        // 2. 시각적 표현(View)을 처리
        // 손(UI) 뷰가 있으면 파괴 후 월드 뷰로 교체
        if (spawnedUnitViews.TryGetValue(unit.Model.uniqueId, out var existing))
        {
            if (!(existing is UnitGameView))
            {
                Destroy(existing.gameObject);
                spawnedUnitViews.Remove(unit.Model.uniqueId);
            }
        }
        if (!spawnedUnitViews.ContainsKey(unit.Model.uniqueId))
        {
            var newView = UnitFactory.SpawnUnitViewForContainer(unit, city);
            if (newView != null)
            {
                spawnedUnitViews[unit.Model.uniqueId] = newView;
            }
        }

        // 3. View에게 도시로 이동하라고 지시 (뷰가 있으면)
        if (spawnedUnitViews.TryGetValue(unit.Model.uniqueId, out var view) && view != null)
        {
            view.AttachToCity(city);
        }

        // 4. 이벤트 발행: "유닛이 도시로 이동했다!"
        // EventBus.Instance.UnitMovedToCity(unit, cityName);

        // 5. 핸드에서 빠졌을 수 있으니 로컬 플레이어 핸드 UI 업데이트
        TryRedrawLocalPlayerHand();
    }

    public UnitPresenter GetPresenterById(string unitId)
    {
        if (spawnedPresenter.TryGetValue(unitId, out var presenter))
            return presenter;

        if (spawnedUnits.TryGetValue(unitId, out UnitModel unit))
        {
            return GetPresenterByModel(unit);
        }
        return null;
    }

    public UnitPresenter GetPresenterByModel(UnitModel unitModel)
    {
        // 캐시된 Presenter가 있으면 반환
        if (spawnedPresenter.TryGetValue(unitModel.uniqueId, out var presenter))
            return presenter;

        // 없으면 새 Presenter를 만들되, 뷰가 있으면 바인딩
        presenter = new UnitPresenter(unitModel, null);
        if (spawnedUnitViews.TryGetValue(unitModel.uniqueId, out BaseUnitView view))
        {
            presenter.BindView(view);
        }
        spawnedPresenter[unitModel.uniqueId] = presenter;
        return presenter;
    }

    private void AddDebugCommands()
    {
        DebugLogConsole.AddCommandInstance("debug.addUnitToPool", "Adds a unit to the pool. usage: debug.addUnitToPool <unitName>", "CreateUnitDataByString", this);
        DebugLogConsole.AddCommandInstance("debug.moveUnitToHand", "Moves a unit from pool to player's hand. usage: debug.moveUnitToHand <unitId> <playerId>", "MoveUnitToHandByName", this);
        DebugLogConsole.AddCommandInstance("debug.moveUnitToCity", "Moves a unit to a city. usage: debug.moveUnitToCity <unitId> <cityName>", "MoveUnitToCityByName", this);
        DebugLogConsole.AddCommandInstance("debug.moveUnitTypeToHand", "Moves a unit (by type) from pool to player's hand. usage: debug.moveUnitTypeToHand <unitName> <playerId>", "MoveUnitTypeToHand", this);
        DebugLogConsole.AddCommandInstance("debug.listUnits", "Lists all spawned units with IDs.", "ListUnits", this);
        DebugLogConsole.AddCommandInstance("debug.initUnits", "Initializes all units from UnitManager.unitDataList", "InitializeUnitsFromDataList", this);
        DebugLogConsole.AddCommandInstance("debug.initUnitsFromJson", "Initializes units from Resources JSON. usage: debug.initUnitsFromJson <ResourcesPath>", "InitializeUnitsFromJson", this);
    }
}


/// <summary>
/// 게임 내에서 유닛을 포함할 수 있는 모든 객체(도시, 플레이어 핸드 등)가 구현해야 하는 인터페이스입니다.
/// </summary>
public interface IUnitContainer
{
    /// <summary>
    /// 이 컨테이너가 현재 담고 있는 유닛들의 목록입니다. (읽기 전용)
    /// </summary>
    Dictionary<UnitPresenter, int> ContainedUnits { get; }


    /// <summary>
    /// 이 컨테이너에 유닛을 추가합니다.
    /// </summary>
    /// <param name="unit">추가할 유닛입니다.</param>
    void AddUnit(UnitPresenter unit);

    /// <summary>
    /// 이 컨테이너에서 유닛을 제거합니다.
    /// </summary>
    /// <param name="unit">제거할 유닛입니다.</param>
    void RemoveUnit(UnitPresenter unit);
}
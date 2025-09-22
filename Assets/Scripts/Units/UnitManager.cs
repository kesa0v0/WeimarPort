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
            int spawnCount = Mathf.Max(1, spec.count);
            // id가 지정되어 있고 count>1이면, id-1, id-2... 형태로 부여. id 미지정이면 unitName-1.. 로 부여
            for (int i = 0; i < spawnCount; i++)
            {
                string baseId = string.IsNullOrEmpty(spec.id) ? spec.unitName : spec.id;
                string desired = spawnCount == 1 ? baseId : $"{baseId}-{i+1}";
                string instanceId = EnsureUniqueInstanceId(desired);
                CreateUnitFromSpec(spec, instanceId);
            }
        }
        TryRedrawLocalPlayerHand();
        UIManager.Instance?.disposedPanel?.Redraw();
    }

    // 동일한 ID가 이미 존재하면 -2, -3...을 붙여 유니크 보장
    private string EnsureUniqueInstanceId(string desired)
    {
        if (!spawnedUnits.ContainsKey(desired)) return desired;
        int suffix = 2;
        string candidate = $"{desired}-{suffix}";
        while (spawnedUnits.ContainsKey(candidate))
        {
            suffix++;
            candidate = $"{desired}-{suffix}";
        }
        Debug.LogWarning($"Duplicate unit id '{desired}' detected. Using '{candidate}' instead.");
        return candidate;
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
            // 1) 문자열 기반 DTO로 먼저 파싱 (enum 문자열 안전 처리)
            var raw = JsonUtility.FromJson<UnitSpawnSpecJsonList>(ta.text);
            if (raw == null || raw.units == null)
            {
                Debug.LogWarning("UnitManager: Parsed empty unit spec list.");
                return;
            }
            // 2) 안전 변환: 문자열 position -> UnitPosition, 누락 시 InPool
            var cooked = new List<UnitSpawnSpec>(raw.units.Count);
            foreach (var r in raw.units)
            {
                if (r == null || string.IsNullOrEmpty(r.unitName)) continue;
                UnitPosition pos;
                if (!Enum.TryParse<UnitPosition>(r.position, true, out pos))
                {
                    // 숫자로 들어온 경우 등도 커버
                    if (int.TryParse(r.position, out int ival) && Enum.IsDefined(typeof(UnitPosition), ival))
                        pos = (UnitPosition)ival;
                    else
                        pos = UnitPosition.InPool; // 안전 기본값
                }
                cooked.Add(new UnitSpawnSpec
                {
                    id = r.id,
                    unitName = r.unitName,
                    membership = r.membership,
                    position = pos,
                    locationId = r.locationId,
                    count = Math.Max(1, r.count)
                });
            }

            ClearAllUnits();
            InitializeUnitsFromSpecs(cooked);
            Debug.Log($"UnitManager: Initialized {cooked.Count} spec lines from JSON.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"UnitManager: Failed to parse unit specs JSON. {ex.Message}");
        }
    }

    private UnitPresenter CreateUnitFromSpec(UnitSpawnSpec spec, string instanceId = null)
    {
        var data = GetUnitDataByName(spec.unitName);
        if (data == null)
        {
            Debug.LogWarning($"UnitManager: UnitData '{spec.unitName}' not found.");
            return null;
        }
        var model = new UnitModel(data, instanceId, spec.membership, spec.position, spec.locationId);
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
                if (string.Equals(model.locationId, "Government", StringComparison.OrdinalIgnoreCase))
                {
                    // 정부 보유로 처리 (별도 뷰 없음)
                    presenter.Model.membership = "Government";
                    presenter.UpdateLocation(GameManager.Instance.gameState.government);
                }
                else
                {
                    var party = PartyRegistry.GetPartyByName(model.locationId) as MainParty;
                    if (party == null)
                    {
                        Debug.LogWarning($"UnitManager: Spec InReserved location '{model.locationId}' not found as MainParty.");
                        break;
                    }
                    presenter.UpdateLocation(party);
                }
                // no UI view, PlayerHandPanel/Government panel renders icons from container.ContainedUnits
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
        if (spawnedUnits.Count == 0) { Debug.Log("No units spawned."); return; }
        foreach (var kv in spawnedUnits)
        {
            var m = kv.Value;
            Debug.Log($"{m.uniqueId} | {m.Data.unitName} | {m.position} | {m.locationId} | owner={m.membership}");
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

    // Pool -> Hand: 자동(선택 UI 없음). 대상 파티는 명시적으로 전달.
    public void MoveUnitToHandAutoById(string unitId, string partyId)
    {
        var presenter = GetPresenterById(unitId);
        if (presenter == null) { Debug.LogWarning($"Presenter not found for {unitId}"); return; }
        var party = PartyRegistry.GetPartyByName(partyId) as MainParty;
        if (party == null) { Debug.LogWarning($"Party not found or not MainParty: {partyId}"); return; }
        MoveUnitToHand(presenter, party);
    }

    public void MoveUnitTypeToHandAuto(string unitName, string partyId)
    {
        var party = PartyRegistry.GetPartyByName(partyId) as MainParty;
        if (party == null) { Debug.LogWarning($"Party not found or not MainParty: {partyId}"); return; }
        foreach (var kv in spawnedUnits)
        {
            var m = kv.Value;
            if (m.position == UnitPosition.InPool && m.Data != null && m.Data.unitName == unitName)
            {
                var presenter = GetPresenterByModel(m);
                MoveUnitToHand(presenter, party);
                Debug.Log($"Moved one {unitName} from Pool to {party.partyName} hand.");
                return;
            }
        }
        Debug.LogWarning($"No unit in pool found with type '{unitName}'.");
    }

    // Membership 변경: OnBoard에서만, 특정 유닛만 가능
    public void TryChangeMembershipOnBoard(string unitId, string newMembership)
    {
        var presenter = GetPresenterById(unitId);
        if (presenter == null) { Debug.LogWarning($"Presenter not found for {unitId}"); return; }
        var model = presenter.Model;
        if (model.position != UnitPosition.OnBoard)
        {
            Debug.LogWarning("Membership can be changed only when the unit is OnBoard.");
            return;
        }
        if (model.Data == null || !model.Data.membershipMutableOnBoard)
        {
            Debug.LogWarning("This unit does not allow membership change on board.");
            return;
        }
        var party = PartyRegistry.GetPartyByName(newMembership);
        if (party == null)
        {
            Debug.LogWarning($"Target membership party '{newMembership}' not found.");
            return;
        }
        model.membership = newMembership;
        Debug.Log($"Unit {model.uniqueId} membership changed to {newMembership}.");
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

    public void MoveUnitToDisposed(UnitPresenter unit)
    {
        // 모델 상태 업데이트
        unit.Model.position = UnitPosition.Disposed;
        unit.Model.locationId = "Disposed";

        // 컨테이너 업데이트
        unit.UpdateLocation(DisposedBin.Instance);

        // 뷰 정리: 폐기 상태에서는 뷰 없음
        if (spawnedUnitViews.TryGetValue(unit.Model.uniqueId, out var existing) && existing != null)
        {
            Destroy(existing.gameObject);
            spawnedUnitViews.Remove(unit.Model.uniqueId);
        }

        TryRedrawLocalPlayerHand();
        UIManager.Instance?.disposedPanel?.Redraw();
    }

    public void RestoreDisposedToCity(UnitPresenter unit, CityPresenter city)
    {
        if (unit.Model.position != UnitPosition.Disposed)
        {
            Debug.LogWarning($"Unit {unit.Model.uniqueId} is not disposed.");
            return;
        }

        MoveUnitToCity(unit, city);
        UIManager.Instance?.disposedPanel?.Redraw();
    }

    // --- Convenience debug wrappers ---

    public void MoveUnitCityToCityById(string unitId, string toCity)
    {
        var presenter = GetPresenterById(unitId);
        if (presenter == null) { Debug.LogWarning($"Presenter not found for {unitId}"); return; }
        var city = CityManager.Instance.GetCity(toCity);
        if (city == null) { Debug.LogWarning($"City not found: {toCity}"); return; }
        MoveUnitToCity(presenter, city);
    }

    public void DisposeUnitById(string unitId)
    {
        var presenter = GetPresenterById(unitId);
        if (presenter == null) { Debug.LogWarning($"Presenter not found for {unitId}"); return; }
        MoveUnitToDisposed(presenter);
    }

    public void RestoreDisposedToCityById(string unitId, string toCity)
    {
        var presenter = GetPresenterById(unitId);
        if (presenter == null) { Debug.LogWarning($"Presenter not found for {unitId}"); return; }
        var city = CityManager.Instance.GetCity(toCity);
        if (city == null) { Debug.LogWarning($"City not found: {toCity}"); return; }
        RestoreDisposedToCity(presenter, city);
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

    // 핸드 아이콘 클릭 시: 해당 타입의 유닛 하나를 찾아 도시로 이동
    public bool TryMoveOneUnitFromHandTypeToCity(MainParty party, UnitData unitData, CityPresenter city)
    {
        if (party == null || unitData == null || city == null) return false;

        foreach (var kv in party.ContainedUnits)
        {
            var presenter = kv.Key;
            int count = kv.Value;
            if (presenter != null && presenter.Model != null && presenter.Model.Data == unitData && count > 0)
            {
                MoveUnitToCity(presenter, city);
                return true;
            }
        }
        Debug.LogWarning($"No unit of type '{unitData.unitName}' found in {party.partyName}'s hand.");
        return false;
    }

    private void AddDebugCommands()
    {
        DebugLogConsole.AddCommandInstance("debug.addUnitToPool", "Adds a unit to the pool. usage: debug.addUnitToPool <unitName>", "CreateUnitDataByString", this);
        DebugLogConsole.AddCommandInstance("debug.moveUnitToHand", "Moves a unit from pool to player's hand. usage: debug.moveUnitToHand <unitId> <playerId>", "MoveUnitToHandByName", this);
        DebugLogConsole.AddCommandInstance("debug.moveUnitToCity", "Moves a unit to a city. usage: debug.moveUnitToCity <unitId> <cityName>", "MoveUnitToCityByName", this);
        DebugLogConsole.AddCommandInstance("debug.moveUnitTypeToHand", "Moves a unit (by type) from pool to player's hand. usage: debug.moveUnitTypeToHand <unitName> <playerId>", "MoveUnitTypeToHand", this);
        DebugLogConsole.AddCommandInstance("debug.moveUnitToHandAuto", "Moves a unit from pool to party hand without selection. usage: debug.moveUnitToHandAuto <unitId> <partyId>", "MoveUnitToHandAutoById", this);
        DebugLogConsole.AddCommandInstance("debug.moveUnitTypeToHandAuto", "Moves a unit (by type) from pool to party hand. usage: debug.moveUnitTypeToHandAuto <unitName> <partyId>", "MoveUnitTypeToHandAuto", this);
        DebugLogConsole.AddCommandInstance("debug.changeMembership", "Changes membership for allowed units OnBoard. usage: debug.changeMembership <unitId> <party>", "TryChangeMembershipOnBoard", this);
        DebugLogConsole.AddCommandInstance("debug.listUnits", "Lists all spawned units with IDs.", "ListUnits", this);
        DebugLogConsole.AddCommandInstance("debug.initUnitsFromJson", "Initializes units from Resources JSON. usage: debug.initUnitsFromJson <ResourcesPath>", "InitializeUnitsFromJson", this);
        DebugLogConsole.AddCommandInstance("debug.moveUnitCityToCity", "Moves a unit from city to city. usage: debug.moveUnitCityToCity <unitId> <toCity>", "MoveUnitCityToCityById", this);
        DebugLogConsole.AddCommandInstance("debug.disposeUnit", "Moves a unit to disposed. usage: debug.disposeUnit <unitId>", "DisposeUnitById", this);
        DebugLogConsole.AddCommandInstance("debug.restoreDisposedToCity", "Restores a disposed unit to city. usage: debug.restoreDisposedToCity <unitId> <toCity>", "RestoreDisposedToCityById", this);
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
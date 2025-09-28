using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using System;
using IngameDebugConsole;

/// <summary>
/// 게임에 존재하는 모든 위협 마커의 생성, 생명주기, 상태 조회를 책임지는 중앙 관리자입니다.
/// 이 클래스는 싱글톤으로 구현되어 어디서든 쉽게 접근할 수 있습니다.
/// </summary>
public class ThreatManager : MonoBehaviour
{
    // --- 싱글톤 구현 ---
    public static ThreatManager Instance { get; private set; }

    // --- 이벤트 정의 ---
    /// <summary>
    /// DR Box의 내용물이 변경될 때마다 호출되는 이벤트입니다.
    /// 변경된 후의 전체 마커 ID 리스트를 전달합니다.
    /// </summary>
    public static event Action<List<string>> OnDRBoxChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    // ---------------------

    [Header("프리팹 및 데이터 참조")]
    [Tooltip("모든 MarkerView 프리팹이 생성될 부모 Transform")]
    [SerializeField] private Transform markerViewParent;

    [Tooltip("DR Box에 얼마나 마커가 있는지 알려주는 Text")]
    [SerializeField] private TextMeshProUGUI drBoxCountText;

    // --- 내부 데이터베이스 ---
    private Dictionary<string, ThreatMarkerData> threatDataRegistry = new Dictionary<string, ThreatMarkerData>();
    private Dictionary<string, ThreatMarkerModel> modelIdMap = new Dictionary<string, ThreatMarkerModel>();
    private Dictionary<ThreatMarkerModel, ThreatMarkerPresenter> presenterMap = new Dictionary<ThreatMarkerModel, ThreatMarkerPresenter>();

    #region 초기화
    /// <summary>
    /// 게임 시작 시 ThreatManager를 초기화합니다.
    /// 모든 ThreatMarkerData SO를 로드하고, 규칙에 따라 모든 마커 인스턴스를 생성합니다.
    /// </summary>
    public void Initialize()
    {
        Debug.Log("ThreatManager 초기화 시작...");
        LoadAllThreatMarkerDataFromResources();
        CreateAllMarkerInstances();
        AddDebugCommands();
        Debug.Log("ThreatManager 초기화 완료. 총 " + modelIdMap.Count + "개의 마커 인스턴스 생성됨.");
    }

    private void LoadAllThreatMarkerDataFromResources()
    {
        ThreatMarkerData[] allMarkerData = Resources.LoadAll<ThreatMarkerData>("ScriptableObjects/ThreatMarkers");
        foreach (var data in allMarkerData)
        {
            if (!threatDataRegistry.ContainsKey(data.DataId))
            {
                threatDataRegistry.Add(data.DataId, data);
            }
        }
    }

    /// <summary>
    /// 룰북 규칙에 따라 게임에 필요한 모든 마커 인스턴스(Model, Presenter, View)를 생성합니다.
    /// (룰북 p. 29 참고)
    /// </summary>
    private void CreateAllMarkerInstances()
    {
        Dictionary<string, int> markerCounts = new Dictionary<string, int>
        {
            { "Threat_Poverty", 12 },
            { "Threat_Unrest", 11 },
            { "Threat_Inflation", 3 },
            { "Threat_Blockade", 3 },
            { "Threat_ViolentPeace", 2 },
            { "Threat_InstableState", 2 },
            { "Threat_MinorityCabinet", 1 },
            { "Threat_BlackFriday", 2 },
            { "Threat_Regime", 4 },
            { "Threat_Councils", 4 },
            { "Threat_Uprising", 4 }
        };

        foreach (var pair in markerCounts)
        {
            string dataId = pair.Key;
            int count = pair.Value;

            if (!threatDataRegistry.ContainsKey(dataId))
            {
                Debug.LogError($"ThreatMarkerData를 찾을 수 없습니다: {dataId}");
                continue;
            }

            ThreatMarkerData data = threatDataRegistry[dataId];

            for (int i = 1; i <= count; i++)
            {
                string instanceId = $"{data.DataId.ToLower()}_{i}";

                ThreatMarkerModel model = new ThreatMarkerModel
                {
                    InstanceId = instanceId,
                    DataId = data.DataId,
                    Data = data,
                    IsFlipped = false // 모든 마커는 기본(활성) 상태로 시작
                };

                GameObject viewObject = Instantiate(data.ModelPrefab, markerViewParent);
                ThreatMarkerView view = viewObject.GetComponent<ThreatMarkerView>();
                view.gameObject.name = $"MarkerView_{instanceId}";
                view.gameObject.SetActive(false);

                ThreatMarkerPresenter presenter = new ThreatMarkerPresenter(model, view);

                modelIdMap.Add(instanceId, model);
                presenterMap.Add(model, presenter);
            }
        }
    }
    #endregion

    #region 공개 API
    /// <summary>
    /// 인스턴스 ID로 해당 마커의 Presenter를 찾습니다.
    /// </summary>
    public ThreatMarkerPresenter GetPresenter(string instanceId)
    {
        if (modelIdMap.TryGetValue(instanceId, out ThreatMarkerModel model))
        {
            if (presenterMap.TryGetValue(model, out ThreatMarkerPresenter presenter))
            {
                return presenter;
            }
        }
        Debug.LogWarning($"Presenter를 찾을 수 없습니다: {instanceId}");
        return null;
    }

    /// <summary>
    /// 특정 종류(DataId)의 마커 중 현재 사용 가능(게임 보드 위에 없는)한 마커 하나를 찾아 반환합니다.
    /// </summary>
    /// <param name="dataId">찾고 싶은 마커의 원본 데이터 ID (e.g., "Threat_Unrest")</param>
    /// <returns>사용 가능한 마커의 Presenter, 없으면 null</returns>
    public ThreatMarkerPresenter GetAvailableMarkerPresenter(string dataId)
    {
        // modelIdMap의 모든 모델들을 순회하며 조건에 맞는 첫 번째 모델을 찾습니다.
        var availableModel = modelIdMap.Values
            .FirstOrDefault(model => model.DataId == dataId && model.CurrentLocation == null);

        if (availableModel != null)
        {
            return presenterMap[availableModel];
        }

        Debug.LogWarning($"사용 가능한 '{dataId}' 마커가 없습니다.");
        return null;
    }

    /// <summary>
    /// 특정 종류의 마커를 찾아 DR Box에 배치합니다.
    /// </summary>
    public void CreateAndPlaceInDRBox(string dataId)
    {
        ThreatMarkerPresenter markerPresenter = GetAvailableMarkerPresenter(dataId);
        if (markerPresenter == null) return;

        // 1. Model 상태 업데이트
        markerPresenter.Model.CurrentLocation = new LocationData { Type = LocationType.DR_Box, Name = "DR_Box" };

        // 2. GameStateModel의 DRBoxMarkerInstanceIds 리스트에 ID 추가
        var drBoxList = GameManager.Instance.gameState.DRBoxMarkerInstanceIds;
        drBoxList.Add(markerPresenter.Model.InstanceId);

        // 3. View는 보이지 않으므로 비활성화 상태 유지
        markerPresenter.View.gameObject.SetActive(false);

        // 4. 이벤트 발생! UI에게 변경 사항을 알립니다.
        OnDRBoxChanged?.Invoke(drBoxList);

        Debug.Log($"{dataId} 마커가 DR Box 데이터에 추가되었습니다.");
    }

    /// <summary>
    /// 특정 종류의 마커를 찾아 지정된 도시에 배치합니다.
    /// </summary>
    public void CreateAndPlaceInCity(string dataId, CityPresenter targetCity)
    {
        PlaceMarkerInCity(dataId, targetCity, false);
    }

    /// <summary>
    /// 지정된 도시에 번영(Prosperity) 마커를 배치합니다. 룰북의 상쇄 규칙을 처리합니다.
    /// </summary>
    public void PlaceProsperityMarkerInCity(CityPresenter targetCity)
    {
        if (targetCity == null) return;

        // 1. 도시에 이미 Poverty/Prosperity 마커가 있는지 확인합니다.
        var existingPovertyMarker = targetCity.Model.ThreatMarkerInstanceIds
            .Select(id => GetPresenter(id))
            .FirstOrDefault(p => p != null && p.Model.Data.Category == ThreatMarkerData.MarkerCategory.TwoSidedPoverty);

        if (existingPovertyMarker != null)
        {
            // 2. 이미 있다면, 해당 마커를 뒤집어 번영 상태로 만듭니다.
            if (existingPovertyMarker.Model.IsFlipped == false) // 이미 번영 상태가 아니라면
            {
                existingPovertyMarker.Model.IsFlipped = true;
                existingPovertyMarker.UpdateView();
                Debug.Log($"{targetCity.Model.cityName}의 빈곤 마커를 번영으로 뒤집습니다.");
            }
        }
        else
        {
            // 3. 없다면, 새로운 빈곤 마커를 '번영 상태(뒤집힌 채)'로 놓습니다.
            PlaceMarkerInCity("Threat_Poverty", targetCity, true);
        }
    }

    // --- 내부 로직을 처리할 private 메서드를 추가합니다 ---
    /// <summary>
    /// 마커를 도시에 배치하는 내부 로직. 뒤집힌 상태로 놓을지 결정할 수 있습니다.
    /// </summary>
    private void PlaceMarkerInCity(string dataId, CityPresenter targetCity, bool startFlipped)
    {
        ThreatMarkerPresenter markerPresenter = GetAvailableMarkerPresenter(dataId);
        if (markerPresenter == null || targetCity == null) return;

        // 1. Model 상태 업데이트 (뒤집힘 상태 포함)
        markerPresenter.Model.CurrentLocation = new LocationData { Type = LocationType.City, Name = targetCity.Model.cityName };
        markerPresenter.Model.IsFlipped = startFlipped;

        // 2. 도시 Presenter에게 마커 추가 요청
        targetCity.AddThreatMarker(markerPresenter);

        // 3. View 활성화 및 외형 업데이트
        markerPresenter.View.gameObject.SetActive(true);
        markerPresenter.UpdateView(); // 이 메서드가 IsFlipped 상태를 보고 올바른 Material을 적용합니다.

        Debug.Log($"{dataId} 마커가 {(startFlipped ? "(뒤집힌 상태)" : "")} (으)로 {targetCity.Model.cityName}에 배치되었습니다.");
    }
    #endregion
    

    private void AddDebugCommands()
    {
        DebugLogConsole.AddCommand("list_markers", "", () =>
        {
            Debug.Log("=== 현재 마커 목록 ===");
            foreach (var model in modelIdMap.Values)
            {
                string location = model.CurrentLocation != null ? $"{model.CurrentLocation.Type}({model.CurrentLocation.Name})" : "None";
                Debug.Log($"- {model.InstanceId}: {model.DataId}, Location: {location}, Flipped: {model.IsFlipped}");
            }
        });
    }
}

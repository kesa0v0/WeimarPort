using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 게임에 존재하는 모든 위협 마커의 생성, 생명주기, 상태 조회를 책임지는 중앙 관리자입니다.
/// 이 클래스는 싱글톤으로 구현되어 어디서든 쉽게 접근할 수 있습니다.
/// </summary>
public class ThreatManager : MonoBehaviour
{
    // --- 싱글톤 구현 ---
    public static ThreatManager Instance { get; private set; }

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
        Debug.Log("ThreatManager 초기화 완료. 총 " + modelIdMap.Count + "개의 마커 인스턴스 생성됨.");
    }

    private void LoadAllThreatMarkerDataFromResources()
    {
        ThreatMarkerData[] allMarkerData = Resources.LoadAll<ThreatMarkerData>("Data/ThreatMarkers");
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
    #endregion
}

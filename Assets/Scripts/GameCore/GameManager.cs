using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Assets")]
    public GameObject cityPrefab; // 도시에 사용할 프리팹을 Inspector에서 직접 할당
    public Transform cityParent;  // 도시들이 생성될 부모 Transform

    // 생성된 모든 도시를 관리하는 딕셔너리
    private readonly Dictionary<string, CityView> cities = new();

    private void Awake()
    {
        // 간단한 싱글톤 설정
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // 게임이 시작될 때 이벤트 구독 설정
        AddEventSubscriptions();
    }

    private void OnDestroy()
    {
        // 오브젝트 파괴 시 이벤트 구독 해제 (메모리 누수 방지)
        RemoveEventSubscriptions();
    }

    // public API: 도시를 생성하는 유일한 창구
    public void CreateCity(string cityName, Vector2 position, int seatCount)
    {
        if (cities.ContainsKey(cityName))
        {
            Debug.LogWarning($"City '{cityName}' already exists.");
            return;
        }
        
        // GameManager가 직접 CityParameters를 만듭니다.
        // 왜냐하면 cityPrefab 같은 핵심 에셋은 GameManager만 알고 있기 때문입니다.
        var parameters = new CityParameters(cityName, position, seatCount, cityPrefab, cityParent);

        var cityPresenter = CityFactory.SpawnCity(parameters);
        
        // 생성된 도시를 딕셔너리에 저장하여 관리합니다.
        cities.Add(cityName, cityPresenter);
        Debug.Log($"City '{cityName}' has been created and registered.");
    }

    public void RemoveCity(string cityName)
    {
        if (cities.TryGetValue(cityName, out _))
        {
            // 실제 게임 오브젝트 파괴 등...
            // city.Destroy(); 
            cities.Remove(cityName);
            Debug.Log($"City '{cityName}' has been removed.");
        }
        else
        {
            Debug.LogWarning($"City '{cityName}' not found.");
        }
    }

    // 이벤트 구독 설정
    void AddEventSubscriptions()
    {
        EventBus.Instance.OnAddCityRequested += HandleCityAdded;
        EventBus.Instance.OnRemoveCityRequested += HandleCityRemoved;
    }

    void RemoveEventSubscriptions()
    {
        EventBus.Instance.OnAddCityRequested -= HandleCityAdded;
        EventBus.Instance.OnRemoveCityRequested -= HandleCityRemoved;
    }

    // 이벤트 핸들러
    private void HandleCityAdded(string name, Vector2 pos, int seats)
    {
        CreateCity(name, pos, seats);
    }
    private void HandleCityRemoved(string name)
    {
        RemoveCity(name);
    }
}

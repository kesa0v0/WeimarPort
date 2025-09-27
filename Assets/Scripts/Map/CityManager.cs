using System;
using System.Collections.Generic;
using System.Linq;
using IngameDebugConsole;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CityManager : MonoBehaviour
{
    private bool _initialized = false;

    public static CityManager Instance { get; private set; }
    

    [Header("Game Assets")]
    public GameObject cityPrefab; // 도시에 사용할 프리팹을 Inspector에서 직접 할당
    public Transform cityParent;  // 도시들이 생성될 부모 Transform

    private Dictionary<string, CityPresenter> presenterMap = new Dictionary<string, CityPresenter>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Initialize()
    {
        CreateCities();

    }

    private void CreateCity(string cityName, Vector2 position, int seatCount)
    {
        if (presenterMap.ContainsKey(cityName))
        {
            Debug.LogWarning($"City '{cityName}' already exists.");
            return;
        }

        // CityModel 생성
        var model = new CityModel(cityName, position, seatCount);

        // CityView 생성
        var viewObj = Instantiate(cityPrefab, position, Quaternion.identity, cityParent);
        viewObj.transform.localRotation = Quaternion.identity; // 로컬 회전값 초기화

        CityPresenter presenter = new CityPresenter(model, viewObj.GetComponent<CityView>());

        // 생성된 도시를 딕셔너리에 저장하여 관리합니다.
        presenterMap.Add(cityName, presenter);
    }

    public void CreateCities()
    {
        Instance.CreateCity("Rostock", new Vector2(-2, 5), 2);
        Instance.CreateCity("Konigsberg", new Vector2(3, 5), 2);
        Instance.CreateCity("Essen", new Vector2(-7, 2), 3);
        Instance.CreateCity("Hamburg", new Vector2(-2, 2), 4);
        Instance.CreateCity("Berlin", new Vector2(3, 2), 5);
        Instance.CreateCity("Koln", new Vector2(-7, -1), 3);
        Instance.CreateCity("Frankfurt", new Vector2(-3, -1), 2);
        Instance.CreateCity("Leipzig", new Vector2(1, -1), 3);
        Instance.CreateCity("Breslau", new Vector2(5, -1), 3);
        Instance.CreateCity("Stuttgart", new Vector2(-3, -4), 2);
        Instance.CreateCity("Munchen", new Vector2(1, -4), 3);
    }


    public CityPresenter GetPresenter(string cityName)
    {
        if (presenterMap.TryGetValue(cityName, out var presenter))
        {
            return presenter;
        }
        Debug.LogWarning($"City '{cityName}' not found.");
        return null;
    }

    internal CityPresenter GetRandomCity(List<CityPresenter> exclude = null)
    {
        var availableCities = presenterMap.Values.AsEnumerable();
        if (exclude != null)
        {
            availableCities = availableCities.Except(exclude);
        }

        if (!availableCities.Any()) return null;

        return availableCities.ElementAt(UnityEngine.Random.Range(0, availableCities.Count()));
    }

    #region Seat Management

    public void AddSeatToCity(string cityName, string partyName, int count)
    {
        if (!presenterMap.TryGetValue(cityName, out var presenter))
        {
            Debug.LogWarning($"City '{cityName}' not found.");
            return;
        }
        var result = Enum.TryParse<FactionType>(partyName, out var faction);
        if (!result)
        {
            Debug.LogWarning($"Party '{partyName}' not found.");
            return;
        }
        presenter.AddPartyBase(faction, count);
    }

    public void RemoveSeatFromCity(string cityName, string partyName, int count)
    {
        if (!presenterMap.TryGetValue(cityName, out var presenter))
        {
            Debug.LogWarning($"City '{cityName}' not found.");
            return;
        }
        var result = Enum.TryParse<FactionType>(partyName, out var faction);
        if (!result)
        {
            Debug.LogWarning($"Party '{partyName}' not found.");
            return;
        }
        presenter.RemoveSeatFromParty(faction, count);
    }

    #endregion

    private void RegisterDebugCommands()
    {
        DebugLogConsole.AddCommandInstance("addCitySeat", "Add seats to a city. Usage: addCitySeat [CityName] [PartyName] [Count]", "AddSeatToCity", this);
        DebugLogConsole.AddCommandInstance("removeCitySeat", "Remove seats from a city. Usage: removeCitySeat [CityName] [PartyName] [Count]", "RemoveSeatFromCity", this);
    }

}

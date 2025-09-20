using System.Collections.Generic;
using IngameDebugConsole;
using UnityEngine;

public class CityManager : MonoBehaviour
{
    public static CityManager Instance { get; private set; }
    

    [Header("Game Assets")]
    public GameObject cityPrefab; // 도시에 사용할 프리팹을 Inspector에서 직접 할당
    public Transform cityParent;  // 도시들이 생성될 부모 Transform
    
    private Dictionary<string, CityView> cities = new();
    private Dictionary<string, CityPresenter> cityPresenters = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        RegisterDebugCommands();
    }


    // public API: 도시를 생성하는 유일한 창구
    public void CreateCity(string cityName, Vector2 position, int seatCount)
    {
        if (cities.ContainsKey(cityName))
        {
            Debug.LogWarning($"City '{cityName}' already exists.");
            return;
        }

        var parameters = new CityParameters(cityName, position, seatCount, cityPrefab, cityParent);
        var cityView = CityFactory.SpawnCity(parameters);
        var cityModel = new CityModel(cityName, position, seatCount);

        RegisterCity(cityName, cityModel, cityView);
    }

    public void RemoveCity(string cityName)
    {
        if (cities.TryGetValue(cityName, out var cityView))
        {
            cities.Remove(cityName);
            cityPresenters.Remove(cityName);

            Destroy(cityView.gameObject);
            UnregisterCity(cityName);
            Debug.Log($"City '{cityName}' has been removed.");
        }
        else
        {
            Debug.LogWarning($"City '{cityName}' not found.");
        }
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

    internal void RegisterCity(string cityName, CityModel model, CityView view)
    {
        // 이미 등록된 경우 덮어쓰지 않음
        if (cities.ContainsKey(cityName)) return;
        var presenter = new CityPresenter(model, view);
        cityPresenters[cityName] = presenter;
        cities[cityName] = view;
    }

    public void UnregisterCity(string cityName)
    {
        cityPresenters.Remove(cityName);
        cities.Remove(cityName);
    }

    public void AddSeatToCity(string cityName, string partyName, int count)
    {
        if (!cityPresenters.TryGetValue(cityName, out var presenter))
        {
            Debug.LogWarning($"City '{cityName}' not found.");
            return;
        }
        var party = PartyRegistry.GetPartyByName(partyName);
        if (party == null)
        {
            Debug.LogWarning($"Party '{partyName}' not found.");
            return;
        }
        presenter.AddSeatToParty(party, count);
    }

    public void RemoveSeatFromCity(string cityName, string partyName, int count)
    {
        if (!cityPresenters.TryGetValue(cityName, out var presenter))
        {
            Debug.LogWarning($"City '{cityName}' not found.");
            return;
        }
        var party = PartyRegistry.GetPartyByName(partyName);
        if (party == null)
        {
            Debug.LogWarning($"Party '{partyName}' not found.");
            return;
        }
        presenter.RemoveSeatFromParty(party, count);
    }

    private void RegisterDebugCommands()
    {
        DebugLogConsole.AddCommandInstance("debug.addCitySeat", "Add seats to a city. Usage: debug.addCitySeat [CityName] [PartyName] [Count]", "AddSeatToCity", this);
        DebugLogConsole.AddCommandInstance("debug.removeCitySeat", "Remove seats from a city. Usage: debug.removeCitySeat [CityName] [PartyName] [Count]", "RemoveSeatFromCity", this);
    }
}

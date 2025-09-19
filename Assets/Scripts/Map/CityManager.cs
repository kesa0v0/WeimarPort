using System.Collections.Generic;
using IngameDebugConsole;
using UnityEngine;

public class CityManager : MonoBehaviour
{
    public static CityManager Instance { get; private set; }

    private Dictionary<string, CityPresenter> cityPresenters = new();
    private Dictionary<string, CityView> cityViews = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        RegisterDebugCommands();
    }

    public void RegisterCity(string cityName, CityModel model, CityView view)
    {
        var presenter = new CityPresenter(model, view);
        cityPresenters[cityName] = presenter;
        cityViews[cityName] = view;
    }

    public void UnregisterCity(string cityName)
    {
        cityPresenters.Remove(cityName);
        cityViews.Remove(cityName);
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

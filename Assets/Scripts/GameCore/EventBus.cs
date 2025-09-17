using System;
using UnityEngine;
using IngameDebugConsole; 


public class EventBus : MonoBehaviour
{
    public static EventBus Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);

            RegisterDebugCommands();
        }
        else
        {
            Destroy(gameObject);
        }
    }



    public event Action<string, Vector2, int> OnAddCityRequested;
    public event Action<string, Vector2, int> OnCityAdded;

    public event Action<string> OnRemoveCityRequested;
    public event Action<string> OnCityRemoved;

    public event Action<string, Party, int> OnAddCitySeatRequested;
    public event Action<string, Party, int> OnCitySeatAdded;

    public event Action<string, Party, int> OnRemoveCitySeatRequested;
    public event Action<string, Party, int> OnCitySeatRemoved;


    public void AddCityRequested(string cityName, float x, float y, int seatCount) => OnAddCityRequested?.Invoke(cityName, new Vector2(x, y), seatCount);
    public void CityAdded(string cityName, float x, float y, int seatCount) => OnCityAdded?.Invoke(cityName, new Vector2(x, y), seatCount);

    public void RemoveCityRequested(string cityName) => OnRemoveCityRequested?.Invoke(cityName);
    public void CityRemoved(string cityName) => OnCityRemoved?.Invoke(cityName);

    public void AddCitySeatRequested(string cityName, string party, int count)
    {
        Debug.Log($"EventBus: AddCitySeatRequested for {cityName}, Party: {party}, Count: {count}");
        // Debug.Log(PartyRegistry.GetPartyByName(party) != null ? $"Found party: {PartyRegistry.GetPartyByName(party).partyName}" : "Party not found");
        OnAddCitySeatRequested?.Invoke(cityName, PartyRegistry.GetPartyByName(party), count);
    }
    public void CitySeatAdded(string cityName, string party, int count)
        => OnCitySeatAdded?.Invoke(cityName, PartyRegistry.GetPartyByName(party), count);

    public void RemoveCitySeatRequested(string cityName, string party, int count)
        => OnRemoveCitySeatRequested?.Invoke(cityName, PartyRegistry.GetPartyByName(party), count);
    public void CitySeatRemoved(string cityName, string party, int count)
        => OnCitySeatRemoved?.Invoke(cityName, PartyRegistry.GetPartyByName(party), count);


    private void RegisterDebugCommands()
    {
        // AddCommandInstance( string command, string description, string methodName, object instance )
        DebugLogConsole.AddCommandInstance("event.addCity", "Add City. Usage: event.addCity [Name] [X] [Y] [Seats]", "AddCityRequested", this);
        DebugLogConsole.AddCommandInstance("event.removeCity", "Remove City. Usage: event.removeCity [Name]", "RemoveCityRequested", this);
        DebugLogConsole.AddCommandInstance("event.addCitySeat", "Add City Seat. Usage: event.addCitySeat [CityName] [Count]", "AddCitySeatRequested", this);
        DebugLogConsole.AddCommandInstance("event.removeCitySeat", "Remove City Seat. Usage: event.removeCitySeat [CityName] [Count]", "RemoveCitySeatRequested", this);

        Debug.Log("EventBus commands have been registered to the debug console.");
    }
}

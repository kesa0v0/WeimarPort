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


    public event Action<string, int> OnAddCitySeatRequested;
    public event Action<string, int> OnCitySeatAdded;

    public event Action<string, int> OnRemoveCitySeatRequested;
    public event Action<string, int> OnCitySeatRemoved;

    public event Action<string, Vector2, int> OnAddCityRequested;
    public event Action<string, Vector2, int> OnCityAdded;

    public event Action<string> OnRemoveCityRequested;
    public event Action<string> OnCityRemoved;


    public void AddCitySeatRequested(string cityName, int count) => OnAddCitySeatRequested?.Invoke(cityName, count);
    public void CitySeatAdded(string cityName, int count) => OnCitySeatAdded?.Invoke(cityName, count);

    public void RemoveCitySeatRequested(string cityName, int count) => OnRemoveCitySeatRequested?.Invoke(cityName, count);
    public void CitySeatRemoved(string cityName, int count) => OnCitySeatRemoved?.Invoke(cityName, count);

    public void AddCityRequested(string cityName, float x, float y, int seatCount) => OnAddCityRequested?.Invoke(cityName, new Vector2(x, y), seatCount);
    public void CityAdded(string cityName, float x, float y, int seatCount) => OnCityAdded?.Invoke(cityName, new Vector2(x, y), seatCount);

    public void RemoveCityRequested(string cityName) => OnRemoveCityRequested?.Invoke(cityName);
    public void CityRemoved(string cityName) => OnCityRemoved?.Invoke(cityName);
    

    private void RegisterDebugCommands()
    {
        // AddCommandInstance( string command, string description, string methodName, object instance )
        DebugLogConsole.AddCommandInstance("event.addCitySeat", "Add City Seat. Usage: event.addCitySeat [CityName] [Count]", "AddCitySeatRequested", this);
        DebugLogConsole.AddCommandInstance("event.removeCitySeat", "Remove City Seat. Usage: event.removeCitySeat [CityName] [Count]", "RemoveCitySeatRequested", this);
        DebugLogConsole.AddCommandInstance("event.addCity", "Add City. Usage: event.addCity [Name] [X] [Y] [Seats]", "AddCityRequested", this);
        DebugLogConsole.AddCommandInstance("event.removeCity", "Remove City. Usage: event.removeCity [Name]", "RemoveCityRequested", this);

        Debug.Log("EventBus commands have been registered to the debug console.");
    }
}

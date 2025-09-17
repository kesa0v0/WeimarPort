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


    public event Action<string, int> OnAddCitySeat;
    public event Action<string, int> OnRemoveCitySeat;


    public void AddCitySeat(string cityName, int count) => OnAddCitySeat?.Invoke(cityName, count);
    public void RemoveCitySeat(string cityName, int count) => OnRemoveCitySeat?.Invoke(cityName, count);
    

    private void RegisterDebugCommands()
    {
        // AddCommandInstance( string command, string description, string methodName, object instance )
        DebugLogConsole.AddCommandInstance("event.addseat", "Add City Seat", "AddCitySeat", this);
        DebugLogConsole.AddCommandInstance("event.removeseat", "Remove City Seat", "RemoveCitySeat", this);

        Debug.Log("EventBus commands have been registered to the debug console.");
    }
}

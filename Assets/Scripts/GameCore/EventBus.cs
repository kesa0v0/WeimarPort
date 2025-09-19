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

    #region GameStateEvents

    public event Action<MainParty> OnTurnStarted;
    public event Action<MainParty> OnTurnEnded;
    public event Action<int> OnRoundStarted;
    public event Action<int> OnRoundEnded;

    public void TurnStarted(MainParty party)
    {
        Debug.Log($"TurnStarted: {party.partyName}");
        OnTurnStarted?.Invoke(party);
    }
    public void TurnEnded(MainParty party)
    {
        Debug.Log($"TurnEnded: {party.partyName}");
        OnTurnEnded?.Invoke(party);
    }
    public void RoundStarted(int roundNumber)
    {
        Debug.Log($"RoundStarted: {roundNumber}");
        OnRoundStarted?.Invoke(roundNumber);
    }
    public void RoundEnded(int roundNumber)
    {
        Debug.Log($"RoundEnded: {roundNumber}");
        OnRoundEnded?.Invoke(roundNumber);
    }

    #endregion


    #region StreetEvents

    public event Action<string, Vector2, int> OnAddCityRequested;
    public event Action<string, Vector2, int> OnCityAdded;

    public event Action<string> OnRemoveCityRequested;
    public event Action<string> OnCityRemoved;

    public event Action<string, Party, int> OnAddCitySeatRequested;
    public event Action<string, Party, int> OnCitySeatAdded;

    public event Action<string, Party, int> OnRemoveCitySeatRequested;
    public event Action<string, Party, int> OnCitySeatRemoved;

    public void AddCityRequested(string cityName, float x, float y, int seatCount)
    {
        Debug.Log($"AddCityRequested: {cityName} at ({x}, {y}) with {seatCount} seats.");
        OnAddCityRequested?.Invoke(cityName, new Vector2(x, y), seatCount);
    }
    public void CityAdded(string cityName, float x, float y, int seatCount)
    {
        Debug.Log($"CityAdded: {cityName} at ({x}, {y}) with {seatCount} seats.");
        OnCityAdded?.Invoke(cityName, new Vector2(x, y), seatCount);
    }

    public void RemoveCityRequested(string cityName)
    {
        Debug.Log($"RemoveCityRequested: {cityName}");
        OnRemoveCityRequested?.Invoke(cityName);
    }
    public void CityRemoved(string cityName) {
        Debug.Log($"CityRemoved: {cityName}");
        OnCityRemoved?.Invoke(cityName);
    }
    
    public void AddCitySeatRequested(string cityName, string partyName, int count)
    {
        var party = PartyRegistry.GetPartyByName(partyName);
        if (party == null)
        {
            Debug.LogError($"Party '{partyName}' not found.");
            return;
        }
        OnAddCitySeatRequested?.Invoke(cityName, party, count);
    }
    public void CitySeatAdded(string cityName, string partyName, int count) {
        var party  = PartyRegistry.GetPartyByName(partyName);
        if (party == null)
        {
            Debug.LogError($"Party '{partyName}' not found.");
            return;
        }
        OnCitySeatAdded?.Invoke(cityName, party, count);
    }

    public void RemoveCitySeatRequested(string cityName, string partyName, int count)
    {
        var party = PartyRegistry.GetPartyByName(partyName);
        if (party == null)
        {
            Debug.LogError($"Party '{partyName}' not found.");
            return;
        }
        OnRemoveCitySeatRequested?.Invoke(cityName, party, count);
    }
    public void CitySeatRemoved(string cityName, string partyName, int count)
    {
        var party = PartyRegistry.GetPartyByName(partyName);
        if (party == null)
        {
            Debug.LogError($"Party '{partyName}' not found.");
            return;
        }
        OnCitySeatRemoved?.Invoke(cityName, party, count);
    }

    #endregion


    private void RegisterDebugCommands()
    {
        // AddCommandInstance( string command, string description, string methodName, object instance )
        DebugLogConsole.AddCommandInstance("event.addCity", "Add City. Usage: event.addCity [Name] [X] [Y] [Seats]", "AddCityRequested", this);
        DebugLogConsole.AddCommandInstance("event.removeCity", "Remove City. Usage: event.removeCity [Name]", "RemoveCityRequested", this);
        DebugLogConsole.AddCommandInstance("event.addCitySeat", "Add City Seat. Usage: event.addCitySeat [CityName] [Count]", "AddCitySeatRequested", this);
        DebugLogConsole.AddCommandInstance("event.removeCitySeat", "Remove City Seat. Usage: event.removeCitySeat [CityName] [Count]", "RemoveCitySeatRequested", this);


        DebugLogConsole.AddCommandInstance("debug.newRound", "Start New Round. Usage: debug.newRound [RoundNumber]", "RoundStarted", this);
        DebugLogConsole.AddCommandInstance("debug.endTurn", "End Turn for Party. Usage: debug.endTurn [PartyName]", "TurnEnded", this);
    }
}

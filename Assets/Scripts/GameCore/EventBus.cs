using System;
using UnityEngine;
using IngameDebugConsole;
using System.Collections.Generic;


public class EventBus : MonoBehaviour
{
    public static EventBus Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region GameStateEvents

    public event Action<Party> OnTurnStarted;
    public event Action<Party> OnTurnEnded;
    public event Action<int> OnRoundStarted;
    public event Action<int> OnRoundEnded;

    public void TurnStarted(Party party)
    {
        Debug.Log($"TurnStarted: {party.Data.factionName}");
        OnTurnStarted?.Invoke(party);
    }
    public void TurnEnded(Party party)
    {
        Debug.Log($"TurnEnded: {party.Data.factionName}");
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

    // 상태 변화 알림만 남김
    public event Action<string, Vector2, int> OnCityAdded;
    public event Action<string> OnCityRemoved;
    public event Action<string, Party, int> OnCitySeatAdded;
    public event Action<string, Party, int> OnCitySeatRemoved;

    // UI-related requests
    public event Action<List<Party>, int, Action<List<Party>>> OnPartySelectionRequested;
    public event Action OnGovernmentChanged;

    public void RequestPartySelection(List<Party> candidates, int count, Action<List<Party>> onChosen)
    {
        OnPartySelectionRequested?.Invoke(candidates, count, onChosen);
    }

    public void NotifyGovernmentChanged()
    {
        OnGovernmentChanged?.Invoke();
    }

    public void CityAdded(string cityName, float x, float y, int seatCount)
    {
        Debug.Log($"CityAdded: {cityName} at ({x}, {y}) with {seatCount} seats.");
        OnCityAdded?.Invoke(cityName, new Vector2(x, y), seatCount);
    }

    public void CityRemoved(string cityName) {
        Debug.Log($"CityRemoved: {cityName}");
        OnCityRemoved?.Invoke(cityName);
    }

    public void CitySeatAdded(string cityName, Party party, int count) {
        Debug.Log($"CitySeatAdded: {cityName}, {party.Data.factionName}, {count}");
        OnCitySeatAdded?.Invoke(cityName, party, count);
    }

    public void CitySeatRemoved(string cityName, Party party, int count)
    {
        Debug.Log($"CitySeatRemoved: {cityName}, {party.Data.factionName}, {count}");
        OnCitySeatRemoved?.Invoke(cityName, party, count);
    }

    #endregion
}

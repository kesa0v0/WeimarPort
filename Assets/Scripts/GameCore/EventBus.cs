using System;
using UnityEngine;
using IngameDebugConsole;
using System.Collections.Generic;
using Unity.VisualScripting;


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

    public event Action<PartyModel> OnTurnStarted;
    public event Action<PartyModel> OnTurnEnded;
    public event Action<int> OnRoundStarted;
    public event Action<int> OnRoundEnded;

    public void TurnStarted(PartyModel party)
    {
        Debug.Log($"TurnStarted: {party.Data.factionName}");
        OnTurnStarted?.Invoke(party);
    }
    public void TurnEnded(PartyModel party)
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
    public event Action<string, FactionType, int> OnCitySeatAdded;
    public event Action<string, FactionType, int> OnCitySeatRemoved;

    // UI-related requests
    public event Action<List<FactionType>, int, Action<List<FactionType>>> OnPartySelectionRequested;
    public event Action OnGovernmentChanged;

    public void RequestPartySelection(List<FactionType> candidates, int count, Action<List<FactionType>> onChosen)
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

    public void CitySeatAdded(string cityName, FactionType party, int count) {
        Debug.Log($"CitySeatAdded: {cityName}, {party.HumanName()}, {count}");
        OnCitySeatAdded?.Invoke(cityName, party, count);
    }

    public void CitySeatRemoved(string cityName, FactionType party, int count)
    {
        Debug.Log($"CitySeatRemoved: {cityName}, {party.HumanName()}, {count}");
        OnCitySeatRemoved?.Invoke(cityName, party, count);
    }

    #endregion
}

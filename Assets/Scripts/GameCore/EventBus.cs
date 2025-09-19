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

    // 상태 변화 알림만 남김
    public event Action<string, Vector2, int> OnCityAdded;
    public event Action<string> OnCityRemoved;
    public event Action<string, Party, int> OnCitySeatAdded;
    public event Action<string, Party, int> OnCitySeatRemoved;

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
        Debug.Log($"CitySeatAdded: {cityName}, {party.partyName}, {count}");
        OnCitySeatAdded?.Invoke(cityName, party, count);
    }

    public void CitySeatRemoved(string cityName, Party party, int count)
    {
        Debug.Log($"CitySeatRemoved: {cityName}, {party.partyName}, {count}");
        OnCitySeatRemoved?.Invoke(cityName, party, count);
    }

    #endregion
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class CityModel
{
    public string cityName;
    public Vector2 position;
    public int seatMaxCount;

    public int currentSeats => seats.Values.Sum();
    public Dictionary<Party, int> seats = new()
    {
        { PartyRegistry.SPD, 0 },
        { PartyRegistry.KPD, 0 },
        { PartyRegistry.Zentrum, 0 },
        { PartyRegistry.DNVP, 0 },
    };

    public CityModel(string name, Vector2 pos, int seatMaxCount)
    {
        cityName = name;
        position = pos;
        this.seatMaxCount = seatMaxCount;
    }

    #region Seat Management

    public void AddSeat(Party party)
    {
        if (currentSeats >= seatMaxCount)
        {
            Debug.LogWarning("No more seats available in the city.");
            return;
        }

        if (seats.ContainsKey(party))
        {
            seats[party]++;
        }
        else
        {
            seats[party] = 1;
        }
    }

    public void RemoveSeat(Party party)
    {
        if (seats.ContainsKey(party) && seats[party] > 0)
        {
            seats[party]--;
        }
        else
        {
            Debug.LogWarning($"No seats to remove for party {party.partyName}.");
        }
    }

    #endregion

}

public struct CityParameters
{
    public string cityName;
    public Vector2 position;
    public int seatMaxCount;
    public GameObject cityPrefab;
    public Transform parent;

    public CityParameters(string cityName, Vector2 position, int seatMaxCount, GameObject cityPrefab, Transform parent = null)
    {
        this.cityName = cityName;
        this.position = position;
        this.seatMaxCount = seatMaxCount;
        this.cityPrefab = cityPrefab;
        this.parent = parent;
    }
}
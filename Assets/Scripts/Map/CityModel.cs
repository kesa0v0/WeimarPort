using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CityModel
{
    public string cityName;
    public Vector2 position;
    public int seatCount;

    public int currentSeats => seats.Values.Sum();
    public Dictionary<Party, int> seats = new()
    {
        { PartyRegistry.SPD, 0 },
        { PartyRegistry.KPD, 0 },
        { PartyRegistry.Zentrum, 0 },
        { PartyRegistry.DNVP, 0 },
    };

    public CityModel(string name, Vector2 pos, int seatCount)
    {
        cityName = name;
        position = pos;
        this.seatCount = seatCount;
    }

    #region Seat Management

    public void AddSeat(Party party)
    {
        if (currentSeats >= seatCount)
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
    public int seatCount;
    public GameObject cityPrefab;
    public Transform parent;

    public CityParameters(string cityName, Vector2 position, int seatCount, GameObject cityPrefab, Transform parent = null)
    {
        this.cityName = cityName;
        this.position = position;
        this.seatCount = seatCount;
        this.cityPrefab = cityPrefab;
        this.parent = parent;
    }
}
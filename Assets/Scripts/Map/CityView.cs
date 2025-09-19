
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class CityView : MonoBehaviour, ICityView
{
    [SerializeField] private TextMeshProUGUI cityName;
    [SerializeField] private GameObject seatGameObject;
    private readonly List<SeatView> seats = new();

    public void Initialize(CityModel model)
    {
        CityManager.Instance.RegisterCity(model.cityName, model, this);
    }

    private void OnDestroy()
    {
        CityManager.Instance.UnregisterCity(cityName.text);
    }

    public void SetCityName(string cityName)
    {
        this.cityName.text = cityName;
    }

    public void SetPosition(Vector2 position)
    {
        transform.position = new Vector3(position.x, 0, position.y);
    }

    public void SetSeatsByCount(int seatCount)
    {
        // If exists, destroy old seats
        foreach (SeatView child in seats)
        {
            Destroy(child.gameObject);
        }
        seats.Clear();

        // Create new seats
        for (int i = 0; i < seatCount; i++)
        {
            SeatView seat = Instantiate(seatGameObject, transform).GetComponent<SeatView>();
            seat.transform.localPosition = new Vector2((i - (seatCount - 1) / 2.0f) * 0.5f, -1);
            seats.Add(seat);
        }
    }

    public void UpdateSeatOccupancy(Dictionary<Party, int> occupiedBy)
    {
        int seatIndex = 0;
        foreach (var party in PartyRegistry.AllMainParties)
        {
            int count = occupiedBy.TryGetValue(party, out int c) ? c : 0;
            for (int i = 0; i < count; i++)
            {
                if (seatIndex < seats.Count)
                {
                    seats[seatIndex].SetColor(party.partyColor);
                    seatIndex++;
                }
                else
                {
                    Debug.LogError("Not enough seats to display all parties.");
                    return;
                }
            }
        }

        // Clear remaining seats
        for (int i = seatIndex; i < seats.Count; i++)
        {
            seats[i].SetColor(Color.gray); // Default color for unoccupied seats
        }
    }

    public void RequestSeatRemovalChoice(List<Party> removableParties, int count, Action<List<Party>> onChosen)
    {
        UIPartyStatusManager.instance.RequestPartySelection(removableParties, count, onChosen);
    }
}


public interface ICityView
{
    void SetCityName(string cityName);
    void SetPosition(Vector2 position);
    void SetSeatsByCount(int seatCount);
    void UpdateSeatOccupancy(Dictionary<Party, int> occupiedBy);
    void RequestSeatRemovalChoice(List<Party> removableParties, int count, Action<List<Party>> onChosen);
}
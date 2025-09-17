using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CityView : MonoBehaviour, ICityView
{
    [SerializeField] private TextMeshProUGUI cityName;
    [SerializeField] private GameObject seatGameObject;
    private readonly List<SeatView> seats = new();

    private CityPresenter presenter;

    public void Initialize(CityModel model)
    {
        presenter = new CityPresenter(model, this);
        AddEventSubscriptions();
    }
    
    private void AddEventSubscriptions()
    {
        // 이 오브젝트가 활성화될 때 Presenter의 핸들러를 이벤트 버스에 구독
        if (presenter != null)
        {
            EventBus.Instance.OnAddCitySeatRequested += presenter.HandleAddCitySeatRequested;
            EventBus.Instance.OnRemoveCitySeatRequested += presenter.HandleRemoveCitySeatRequested;
        }
    }

    private void RemoveEventSubscriptions()
    {
        // 이 오브젝트가 비활성화될 때 이벤트 버스에서 구독 해제
        if (presenter != null)
        {
            EventBus.Instance.OnAddCitySeatRequested -= presenter.HandleAddCitySeatRequested;
            EventBus.Instance.OnRemoveCitySeatRequested -= presenter.HandleRemoveCitySeatRequested;
        }
    }


    public void SetCityName(string cityName)
    {
        this.cityName.text = cityName;
    }

    public void SetPosition(Vector2 position)
    {
        transform.position = position;
    }

    public void SetSeatCount(int seatCount)
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
            // 자리 중간 기준으로 배치 (가로 0.5 간격)
            seat.transform.localPosition = new Vector2((i - (seatCount - 1) / 2.0f) * 0.5f, -1);
            seats.Add(seat);
        }
    }


    public void UpdateSeatOccupancy(Dictionary<Party, int> occupiedBy)
    {
        int seatIndex = 0;
        foreach (var entry in occupiedBy)
        {
            Party party = entry.Key;
            int count = entry.Value;

            for (int i = 0; i < count; i++)
            {
                if (seatIndex < seats.Count)
                {
                    seats[seatIndex].SetColor(party.partyColor);
                    seatIndex++;
                }
                else
                {
                    Debug.LogWarning("Not enough seats to display all parties.");
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
}

public interface ICityView
{
    void SetCityName(string cityName);
    void SetPosition(Vector2 position);
    void SetSeatCount(int seatCount);

    void UpdateSeatOccupancy(Dictionary<Party, int> occupiedBy);
}
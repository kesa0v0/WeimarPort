using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CityView : MonoBehaviour, ICityView
{
    [SerializeField] private TextMeshProUGUI cityName;
    [SerializeField] private GameObject seatGameObject;
    private readonly List<SeatView> seats = new();

    private CityPresenter presenter;

    private void Awake()
    {
        // Example initialization, in real case, model and presenter should be set from outside
        CityModel model = new CityModel("Sample City", new Vector2(0, 0), 5);
        presenter = new CityPresenter(model, this);
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

    
    public void UpdateSeatOccupancy(List<Party> occupiedBy)
    {
        for (int i = 0; i < seats.Count; i++)
        {
            SeatView seat = seats[i];
            if (i < occupiedBy.Count)
            {
                // 좌석이 점유된 경우, 색상을 변경하거나 아이콘 표시 등
                // 예: seat.SetOccupied(occupiedBy[i]);
            }
            else
            {
                // 좌석이 비어있는 경우, 기본 상태로 설정
                // 예: seat.SetOccupied(null);
            }
        }
    }
}

public interface ICityView
{
    void SetCityName(string cityName);
    void SetPosition(Vector2 position);
    void SetSeatCount(int seatCount);

    void UpdateSeatOccupancy(List<Party> occupiedBy);
}
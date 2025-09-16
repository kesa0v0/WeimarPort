using UnityEngine;

public class CityPresenter
{
    private CityModel model;
    private ICityView view;


    public CityPresenter(CityModel model, ICityView view)
    {
        this.model = model;
        this.view = view;

        // Initialize the view with model data
        InitView();
    }

    private void InitView()
    {
        view.SetCityName(model.cityName);
        view.SetPosition(model.position);
        view.SetSeatCount(model.seatCount);
        view.UpdateSeatOccupancy(model.seats);
    }

    public void AddSeatToParty(Party party, int count = 1)
    {
        int available = model.seatCount - model.currentSeats;
        int toAdd = Mathf.Min(count, available);

        // 1. 먼저 가능한 만큼 추가
        for (int i = 0; i < toAdd; i++)
            model.AddSeat(party);

        // 2. 남은 만큼은 다른 당에서 제거 후 추가
        int remaining = count - toAdd;
        for (int i = 0; i < remaining; i++)
        {
            // 다른 당에서 하나 제거 (예시: 첫 번째로 seats가 있는 당)
            Party toRemove = null;
            foreach (var kv in model.seats)
            {
                if (kv.Key != party && kv.Value > 0)
                {
                    toRemove = kv.Key;
                    break;
                }
            }
            if (toRemove != null)
            {
                model.RemoveSeat(toRemove);
                model.AddSeat(party);
            }
            else
            {
                // 더 이상 뺄 수 있는 좌석이 없음
                break;
            }
        }

        view.UpdateSeatOccupancy(model.seats);
    }

    public void RemoveSeatFromParty(Party party, int count = 1)
    {
        count = Mathf.Min(count, model.currentSeats);
        for (int i = 0; i < count; i++)
            model.RemoveSeat(party);

        view.UpdateSeatOccupancy(model.seats);
    }

}

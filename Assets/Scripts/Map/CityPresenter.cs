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

    public void AddSeatToParty(Party party)
    {
        model.AddSeat(party);
        view.UpdateSeatOccupancy(model.seats);
    }
    
    public void RemoveSeatFromParty(Party party)
    {
        model.RemoveSeat(party);
        view.UpdateSeatOccupancy(model.seats);
    }
}

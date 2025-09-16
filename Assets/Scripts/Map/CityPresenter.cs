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
    }


    public void OnCityClicked()
    {
        // Handle city click event
        Debug.Log($"City {model.cityName} clicked!");
        // Possibly open city details UI or perform other actions
    }
}

using UnityEngine;

public class CityFactory
{
    public static CityPresenter SpawnCity(string cityName, Vector2 position, int seatCount, GameObject cityPrefab, Transform parent = null)
    {
        // CityModel 생성
        var model = new CityModel(cityName, position, seatCount);

        // CityView 생성
        var viewObj = Object.Instantiate(cityPrefab, position, Quaternion.identity, parent);
        var view = viewObj.GetComponent<CityView>();

        // Presenter 연결
        var presenter = new CityPresenter(model, view);

        return presenter;
    }
}

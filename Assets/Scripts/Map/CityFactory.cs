using UnityEngine;

public class CityFactory
{

    public static CityPresenter SpawnCity(CityParameters paramenters)
    {
        // CityModel 생성
        var model = new CityModel(paramenters.cityName, paramenters.position, paramenters.seatCount);

        // CityView 생성
        var viewObj = Object.Instantiate(paramenters.cityPrefab, paramenters.position, Quaternion.identity, paramenters.parent);
        viewObj.transform.localRotation = Quaternion.identity; // 로컬 회전값 초기화
        var view = viewObj.GetComponent<CityView>();

        // Presenter 연결
        var presenter = new CityPresenter(model, view);

        return presenter;
    }
}

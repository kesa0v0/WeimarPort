using UnityEngine;

public class CityFactory
{

    public static CityView SpawnCity(CityParameters paramenters)
    {
        // CityModel 생성
        var model = new CityModel(paramenters.cityName, paramenters.position, paramenters.seatMaxCount);

        // CityView 생성
        var viewObj = Object.Instantiate(paramenters.cityPrefab, paramenters.position, Quaternion.identity, paramenters.parent);
        viewObj.transform.localRotation = Quaternion.identity; // 로컬 회전값 초기화
        var view = viewObj.GetComponent<CityView>();

        // 생성된 도시를 딕셔너리에 저장하여 관리합니다.
        CityManager.Instance.RegisterCity(model.cityName, model, view);
    
        return view;
    }
}

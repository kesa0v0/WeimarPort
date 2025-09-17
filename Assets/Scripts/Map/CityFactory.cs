using UnityEngine;

public class CityFactory
{
    public static CityView CreateCity(string name, Vector3 position)
    {
        GameObject cityObject = new GameObject(name);
        cityObject.transform.position = position;

        CityView city = cityObject.AddComponent<CityView>();
        return city;
    }
}

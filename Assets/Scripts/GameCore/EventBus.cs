using System;


public static class EventBus
{
    public static event Action OnGameStart;
    public static void gameStart() => OnGameStart?.Invoke();


    public static event Action<string, int> OnAddCitySeat;
    public static void addCitySeat(string cityName, int count) => OnAddCitySeat?.Invoke(cityName, count);

    public static event Action<string, int> OnRemoveCitySeat;
    public static void removeCitySeat(string cityName, int count) => OnRemoveCitySeat?.Invoke(cityName, count);
}

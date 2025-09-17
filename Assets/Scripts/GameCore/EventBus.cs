public static class EventBus
{
    public static event System.Action OnGameStart;
    public static void gameStart() => OnGameStart?.Invoke();


    public static event System.Action OnAddCitySeat;
    public static void addCitySeat() => OnAddCitySeat?.Invoke();

    public static event System.Action OnRemoveCitySeat;
    public static void removeCitySeat() => OnRemoveCitySeat?.Invoke();
}

using System;
using UnityEngine;
using IngameDebugConsole; 


public class EventBus : MonoBehaviour
{
    public static EventBus Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);

            RegisterDebugCommands();
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public event Action<string, int> OnAddCitySeat;
    public event Action<string, int> OnRemoveCitySeat;


    public void AddCitySeat(string cityName, int count) => OnAddCitySeat?.Invoke(cityName, count);
    public void RemoveCitySeat(string cityName, int count) => OnRemoveCitySeat?.Invoke(cityName, count);
    

    // --- 콘솔 명령어 등록을 위한 헬퍼 메소드 ---
    private void RegisterDebugCommands()
    {
        // AddCommandInstance( string command, string description, string methodName, object instance )
        DebugLogConsole.AddCommandInstance("event.addseat", "도시 의석 추가", "AddCitySeat", this);
        DebugLogConsole.AddCommandInstance("event.removeseat", "도시 의석 제거", "RemoveCitySeat", this);

        Debug.Log("EventBus commands have been registered to the debug console.");
    }
}

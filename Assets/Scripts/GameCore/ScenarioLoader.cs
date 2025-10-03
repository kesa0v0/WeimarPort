using UnityEngine;
using Newtonsoft.Json;
using IngameDebugConsole;
using System;

public class ScenarioLoader : MonoBehaviour
{
    [Header("Manager 참조")]
    public GameManager gameManager;
    public CityManager cityManager;
    

    private ScenarioExecutor executor;

    void Awake()
    {
        // 실행기(Executor)를 생성하고 필요한 Manager들을 주입
        executor = new ScenarioExecutor(gameManager, cityManager /*, ... */);

        // Debug Console에 명령어 등록
        DebugLogConsole.AddCommand<string>("loadScenario", "Load a scenario from json file", LoadScenarioFromResources);
    }

    // 외부에서 호출할 공개 메서드 (UI 버튼 등에 연결)
    public void LoadScenarioFromResources(string scenarioFileName)
    {
        // 1. Resources 폴더에서 JSON 파일 불러오기
        TextAsset jsonFile = Resources.Load<TextAsset>($"Scenarios/{scenarioFileName}");
        if (jsonFile == null)
        {
            Debug.LogError($"시나리오 파일을 찾을 수 없습니다: {scenarioFileName}");
            return;
        }

        // 2. 텍스트를 ScenarioData 객체로 파싱
        ScenarioData scenarioData = JsonConvert.DeserializeObject<ScenarioData>(jsonFile.text);
        if (scenarioData == null || scenarioData.setupScript == null)
        {
            Debug.LogError("시나리오 파일 파싱에 실패했습니다.");
            return;
        }

        // 3. 실행기에게 스크립트 실행을 위임
        StartCoroutine(executor.ExecuteScript(scenarioData.setupScript));
    }
}
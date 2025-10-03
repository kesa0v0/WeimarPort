using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Event.UI;
using UnityEngine;

public class ScenarioExecutor
{
    // 게임의 다른 Manager들에 대한 참조
    private readonly GameManager gameManager;
    private readonly CityManager cityManager;
    // ... ThreatManager, UnitManager 등 필요한 다른 Manager들 ...

    private CityModel selectedCity = null; // 플레이어가 선택한 도시를 저장할 변수

    // 생성자를 통해 필요한 Manager들을 주입받음
    public ScenarioExecutor(GameManager gm, CityManager cm /*, ... */)
    {
        this.gameManager = gm;
        this.cityManager = cm;
    }

    // 스크립트를 실행하는 메인 메서드
    public IEnumerator ExecuteScript(List<SetupInstruction> script)
    {
        Debug.Log("시나리오 스크립트 실행을 시작합니다...");
        List<CityPresenter> usedCitiesForUniqueness = new List<CityPresenter>();

        foreach (var instruction in script)
            {
                usedCitiesForUniqueness.Clear(); // 매 명령어마다 초기화
                // Zentrum을 Z로 자동 변환
                if (instruction.args.partyId == "Zentrum") instruction.args.partyId = "Z";
                switch (instruction.command)
                {
                    // 특정 위협 마커를 지정된 위치에 배치합니다.
                    // dataId, count, location
                    case "PlaceThreatMarker":
                        ExecutePlaceThreatMarker(instruction.args, usedCitiesForUniqueness);
                        break;

                    // 특정 정당의 CitySeat를 도시에 배치합니다.
                    // partyId, count, location
                    case "PlacePartyBases":
                        yield return ExecutePlacePartyBases(instruction.args, usedCitiesForUniqueness);
                        break;

                    // 특정 정당의 의석을 의회에 추가합니다.
                    // partyId, count
                    case "PlaceParliamentSeats":
                        // TODO: ExecutePlaceParliamentSeats 메서드 구현
                        Debug.Log($"명령어 '{instruction.command}' 실행 (구현 필요)");
                        break;

                    // 특정 종류의 유닛을 해당 정당의 공급처에 추가합니다.
                    // dataId, partyId, count
                    case "ReinforceUnit":
                        // TODO: ExecuteReinforceUnit 메서드 구현
                        Debug.Log($"명령어 '{instruction.command}' 실행 (구현 필요)");
                        break;

                    // 특정 유닛 인스턴스를 지정된 위치(도시/공급처 등)에 배치합니다.
                    // instanceId, location
                    case "PlaceUnit":
                        ExecutePlaceUnit(instruction.args);
                        break;

                    // 특정 정당의 승점(VP)을 value만큼 변경합니다. (음수 가능)
                    // partyId, value
                    case "ModifyVp":
                        // TODO: ExecuteModifyVp 메서드 구현
                        Debug.Log($"명령어 '{instruction.command}' 실행 (구현 필요)");
                        break;

                    // 특정 정당의 예비 점수를 value만큼 변경합니다. (음수 가능)
                    // partyId, value
                    case "ModifyReserve":
                        // TODO: ExecuteModifyReserve 메서드 구현
                        Debug.Log($"명령어 '{instruction.command}' 실행 (구현 필요)");
                        break;

                    // 특정 트랙(Economy, NSDAP 등)을 value만큼 이동합니다.	
                    // dataId, value
                    case "MoveTrack":
                        // TODO: ExecuteMoveTrack 메서드 구현
                        Debug.Log($"명령어 '{instruction.command}' 실행 (구현 필요)");
                        break;

                    // 특정 이슈 마커를 사회 트랙에 배치합니다.
                    // dataId, location
                    case "PlaceIssueMarker":
                        // TODO: ExecutePlaceIssueMarker 메서드 구현
                        Debug.Log($"명령어 '{instruction.command}' 실행 (구현 필요)");
                        break;

                    // 특정 군소 정당(DDP 등)의 제어권을 partyId에게 넘깁니다.
                    // dataId, partyId
                    case "ChangeMinorPartyControl":
                        // TODO: ExecuteChangeMinorPartyControl 메서드 구현
                        Debug.Log($"명령어 '{instruction.command}' 실행 (구현 필요)");
                        break;

                    // 특정 유닛을 해산된 유닛 공간으로 보냅니다.
                    // instanceId
                    case "DissolveUnit":
                        // TODO: ExecuteDissolveUnit 메서드 구현
                        Debug.Log($"명령어 '{instruction.command}' 실행 (구현 필요)");
                        break;

                    // 특정 외교 관계 깃발을 보드에 배치합니다.	
                    // flagType, count
                    case "PlaceFlag":
                        // TODO: ExecutePlaceFlag 메서드 구현
                        Debug.Log($"명령어 '{instruction.command}' 실행 (구현 필요)");
                        break;

                    default:
                        Debug.LogWarning($"알 수 없는 명령어: '{instruction.command}'");
                        break;
                }
            }
        Debug.Log("시나리오 스크립트 실행 완료.");
    }

    // 'PlaceThreatMarker' 명령어를 처리하는 메서드
    private void ExecutePlaceThreatMarker(Arguments args, List<CityPresenter> usedCities)
    {
        int count = args.count > 0 ? args.count : 1;
        for (int i = 0; i < count; i++)
        {
            // location.type 문자열을 직접 사용하여 분기 처리합니다.
            switch (args.location.type)
            {
                case "DR_Box":
                    // DR_Box에 배치하는 로직 (ThreatManager에 구현 필요)
                    ThreatManager.Instance.CreateAndPlaceInDRBox(args.dataId);
                    break;

                case "SpecificCity":
                    var targetCity = CityManager.Instance.GetPresenter(args.location.name);
                    if (targetCity != null)
                    {
                        ThreatManager.Instance.CreateAndPlaceInCity(args.dataId, targetCity);
                    }
                    break;

                case "RandomCity":
                    bool unique = args.location.@params?.unique ?? false;
                    var randomCity = CityManager.Instance.GetRandomCity(exclude: unique ? usedCities : null);
                    if (randomCity != null)
                    {
                        ThreatManager.Instance.CreateAndPlaceInCity(args.dataId, randomCity);
                        if (unique) usedCities.Add(randomCity);
                    }
                    break;

                default:
                    Debug.LogWarning($"알 수 없는 위치 타입: '{args.location.type}'");
                    break;
            }
        }
    }

    private IEnumerator ExecutePlacePartyBases(Arguments args, List<CityPresenter> usedCities)
    {
        int count = args.count > 0 ? args.count : 1;
        var party = gameManager.GetParty(Enum.TryParse<FactionType>(args.partyId, out var faction)
            ? faction : throw new ArgumentException($"Invalid faction type: {args.partyId}"));
        if (party == null)
        {
            Debug.LogWarning($"정당 '{args.partyId}'를 찾을 수 없습니다.");
            yield break;
        }

        for (int i = 0; i < count; i++)
        {
            switch (args.location.type)
            {
                case "SpecificCity":
                    var targetCity = CityManager.Instance.GetPresenter(args.location.name);
                    if (targetCity != null)
                    {
                        // cityManager.PlacePartyBase(party, targetCity);
                        Debug.Log($"{targetCity.Model.cityName}에 '{party.Data.factionType}' CitySeat 배치 (실제 로직 호출 필요)");
                    }
                    break;

                case "RandomCity":
                    bool unique = args.location.@params?.unique ?? false;
                    var randomCity = CityManager.Instance.GetRandomCity(exclude: unique ? usedCities : null);
                    if (randomCity != null)
                    {
                        // cityManager.PlacePartyBase(party, randomCity);
                        Debug.Log($"{randomCity.Model.cityName}에 '{party.Data.factionType}' CitySeat 배치 (실제 로직 호출 필요)");
                        if (unique) usedCities.Add(randomCity);
                    }
                    break;
                
                case "PlayerChoice_City":
                    Debug.Log($"플레이어에게 '{party.Data.factionType}' CitySeat 배치할 도시 선택 요청");
                    bool uniqueForPlayerChoice = args.location.@params?.unique ?? false;
                    List<CityModel> availableCities = CityManager.Instance.GetAllCityModels();
                    if (uniqueForPlayerChoice)
                    {
                        // usedCities에 이미 선택된 도시가 있다면 제외
                        var usedCityModels = usedCities.Select(p => p.Model).ToHashSet();
                        availableCities = availableCities.Where(city => !usedCityModels.Contains(city)).ToList();
                    }
                    EventBus.Subscribe<SelectionMadeEvent<CityModel>>(OnCitySelected);
                    EventBus.Publish(new RequestSelectionEvent<CityModel>(PlayerSelectionType.Setup_PlaceInitialPartyBase, availableCities));

                    // selectedCity 변수가 채워질 때까지 대기
                    while (selectedCity == null)
                    {
                        yield return null; // 다음 프레임까지 대기
                    }

                    // 선택 완료 후 로직 처리
                    Debug.Log($"{selectedCity.cityName}에 '{party.Data.factionType}' CitySeat 배치 (실제 로직 호출 필요)");
                    yield return cityManager.AddSeatToCity(party, cityManager.GetPresenter(selectedCity.cityName));

                    // unique 옵션이 true라면, 사용한 도시를 usedCities에 추가
                    if (uniqueForPlayerChoice)
                    {
                        var selectedPresenter = cityManager.GetPresenter(selectedCity.cityName);
                        if (selectedPresenter != null && !usedCities.Contains(selectedPresenter))
                            usedCities.Add(selectedPresenter);
                    }

                    // 다음 선택을 위해 초기화 및 구독 해제
                    selectedCity = null;
                    EventBus.Unsubscribe<SelectionMadeEvent<CityModel>>(OnCitySelected);
                    break;

                default:
                    Debug.LogWarning($"알 수 없는 위치 타입: '{args.location.type}'");
                    break;
            }
        }
    }

    // 선택 완료 이벤트가 발생했을 때 호출될 콜백 메서드
    private void OnCitySelected(SelectionMadeEvent<CityModel> e)
    {
        selectedCity = e.SelectedItem;
    }

    private void ExecutePlaceUnit(Arguments args)
    {   
        Debug.Log($"'{args.partyId}' 유닛을 배치합니다.");
        int count = args.count > 0 ? args.count : 1; // count가 없으면 1로 간주
        var party = gameManager.GetParty(Enum.TryParse<FactionType>(args.partyId, out var faction)
            ? faction : throw new ArgumentException($"Invalid faction type: {args.partyId}"));
        if (party == null)
        {
            Debug.LogWarning($"정당 '{args.partyId}'를 찾을 수 없습니다.");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            if (args.location.type == "RandomCity")
            {
                CityPresenter targetCity = cityManager.GetRandomCity(exclude: null);
                // unitManager.PlaceUnit(party, targetCity);
                Debug.Log($"{targetCity.Model.cityName}에 '{args.instanceId}({args.dataId})' 유닛 배치 (실제 로직 호출 필요)");
            }
            else if (args.location.type == "SpecificCity")
            {
                CityPresenter targetCity = cityManager.GetPresenter(args.location.name);
                if (targetCity != null)
                {
                    // unitManager.PlaceUnit(party, targetCity);
                    Debug.Log($"{targetCity.Model.cityName}에 '{args.instanceId}({args.dataId})' 유닛 배치 (실제 로직 호출 필요)");
                }
            }
            else
            {
                Debug.LogWarning($"알 수 없는 위치 타입: '{args.location.type}'");
            }
        }
    }

}
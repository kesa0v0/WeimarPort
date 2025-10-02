using Event.UI;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public UIPartyStatusManager partyStatusManager;
    public PlayerHandPanel playerHandPanel;
    public DisposedPanel disposedPanel;
    public GovernmentPanel governmentPanel;


    [Header("Tooltip Panels")]
    public GameObject quickViewPanel; // Inspector에서 Quick-View 패널 연결
    public GameObject unitIconPrefab; // 유닛 아이콘 프리팹

    private QuickViewPanel _currentQuickView; // 현재 씬에 생성된 퀵뷰 인스턴스를 저장할 변수


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        EventBus.Subscribe<RequestSelectionEvent<CityModel>>(OnRequestCitySelection);
    }



    public void OnRequestCitySelection(RequestSelectionEvent<CityModel> evt)
    {
        Debug.Log($"UIManager: 도시 선택 요청 이벤트 수신 - {evt.Instruction}, 아이템 수: {evt.Items.Count}");
        // 여기서 evt.Items를 사용하여 UI에 도시 목록을 표시하고 선택을 처리합니다.
        foreach (var city in evt.Items)
        {
            Debug.Log($"도시: {city.cityName}");
        }   
    }

    public void ShowQuickView(CityModel cityModel, Vector3 worldPosition)
    {
        // 퀵뷰가 없다면 프리팹으로부터 새로 생성
        if (_currentQuickView == null)
        {
            GameObject instance = Instantiate(quickViewPanel, transform); // UIManager의 자식으로 생성
            _currentQuickView = instance.GetComponent<QuickViewPanel>();
        }

        // 도시 이름 설정
        _currentQuickView.cityNameText.text = cityModel.cityName;

        // 생성된 인스턴스의 스크립트를 통해 UnitContainer에 접근합니다.
        RectTransform container = _currentQuickView.unitContainer;

        // 1. 기존 유닛 아이콘들 삭제
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        // 2. CityModel 데이터로 새로운 유닛 아이콘 생성 및 부모 지정
        foreach (UnitModel unit in cityModel.GetUnitsInCity())
        {
            GameObject iconObj = Instantiate(unitIconPrefab, container);
            // 강도 표시용 텍스트
            var txt = iconObj.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (txt != null)
            {
                txt.text = unit.Data != null ? unit.Data.Strength.ToString() : "?";
            }
            // 아이콘 이미지 및 소속 정당별 색상
            var img = iconObj.GetComponent<UnityEngine.UI.Image>();
            if (img != null)
            {
                // 1. 아이콘 스프라이트
                if (unit.Data != null && unit.Data.Icon != null)
                {
                    img.sprite = unit.Data.Icon;
                }
                else
                {
                    Debug.LogWarning($"Unit Data or Icon is null for unit in city {cityModel.cityName}");
                }
            }
        }

        // 3. 패널 위치 설정 및 활성화 (월드좌표 → 캔버스 로컬좌표 변환)
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPosition);
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main, out localPoint))
            {
                _currentQuickView.GetComponent<RectTransform>().anchoredPosition = localPoint;
            }
        }
        else
        {
            // fallback: 그냥 월드좌표
            _currentQuickView.transform.position = worldPosition;
        }
        _currentQuickView.gameObject.SetActive(true);
    }

    public void HideQuickView()
    {
        if (_currentQuickView != null)
        {
            _currentQuickView.gameObject.SetActive(false);
        }
    }
}

/// <summary>
/// '바이마르' 게임에서 플레이어가 마주하는 모든 선택 상황을 정의하는 열거형입니다.
/// EventBus를 통해 선택을 요청할 때 이 타입을 사용하여 UI와 로직을 분기합니다.
/// </summary>
public enum PlayerSelectionType
{
    #region // ========== 게임 준비 (Game Setup) ========== //
    /// <summary>
    /// 게임 시작 시 초기 정당 기반(피규어)을 배치할 도시를 선택합니다. [cite: 303]
    /// </summary>
    Setup_PlaceInitialPartyBase,
    #endregion


    #region // ========== 아젠다 페이즈 (Agenda Phase) ========== //
    /// <summary>
    /// 매 라운드 시작 시, 4개의 아젠다 카드 중 하나를 선택합니다. [cite: 605, 667]
    /// </summary>
    Agenda_ChooseAgendaCard,
    /// <summary>
    /// 특정 아젠다 카드 효과로 인해, 아직 공급처에 있는 백색 이슈 마커 중 하나를 선택하여 의견 트랙에 놓습니다. [cite: 681]
    /// </summary>
    Agenda_ChooseWhiteIssueMarkerToPlace,
    #endregion


    #region // ========== 임펄스 페이즈: 카드 사용 (Impulse Phase: Card Play) ========== //
    /// <summary>
    /// 손에 있는 카드를 어떤 용도(이벤트, 토론, 행동)로 사용할지 선택합니다. [cite: 608]
    /// </summary>
    Impulse_ChooseCardPlayOption,
    /// <summary>
    /// '토론(Debate)' 액션 시, 의견 트랙에서 전진시킬 첫 번째 이슈 마커를 선택합니다. [cite: 731]
    /// </summary>
    Impulse_Debate_ChooseFirstIssue,
    /// <summary>
    /// '토론(Debate)' 액션 시, 의견 트랙에서 전진시킬 두 번째 이슈 마커를 선택합니다. [cite: 731]
    /// </summary>
    Impulse_Debate_ChooseSecondIssue,
    /// <summary>
    /// '행동(Actions)'을 수행할 도시 하나를 선택합니다. [cite: 730, 817]
    /// </summary>
    Impulse_Actions_ChooseCity,
    /// <summary>
    /// 도시에서 수행할 구체적인 행동(시위, 동원 등)을 선택합니다.
    /// </summary>
    Impulse_Actions_ChooseActionType,
    #endregion


    #region // ========== 임펄스 페이즈: 도시 행동 (Impulse Phase: City Actions) ========== //
    /// <summary>
    /// '동원(Mobilize)' 시, 이동시킬 자신의 유닛을 선택합니다. [cite: 1283]
    /// </summary>
    Action_Mobilize_ChooseUnit,
    /// <summary>
    /// 도시가 가득 찼을 때 '시위(Demonstration)'를 통해 제거할 상대방의 정당 기반을 선택합니다. [cite: 1170]
    /// </summary>
    Action_Demonstration_ChooseOpponentBaseToRemove,
    /// <summary>
    /// '장악(Take Control)' 시, 통제권을 뺏거나 되찾을 국가방위군(Reichswehr) 유닛을 선택합니다. [cite: 1270]
    /// </summary>
    Action_TakeControl_ChooseReichswehrUnit,
    /// <summary>
    /// '전투(Fight)' 시, 공격할 상대 야당을 선택합니다. [cite: 1300]
    /// </summary>
    Action_Fight_ChooseOpposingParty,
    /// <summary>
    /// (DNVP 전용) 정부의 '전투(Fight)' 액션을 도울지 여부를 결정합니다. [cite: 1304]
    /// </summary>
    Action_Fight_DNVP_ConfirmSupport,
    /// <summary>
    /// (급진 정당 외) 상대의 쿠데타(Coup) 시도에 자신의 유닛으로 저항할지 여부를 선택합니다. [cite: 1406]
    /// </summary>
    Action_Coup_ConfirmOpposition,
    #endregion


    #region // ========== 임펄스 페이즈: 대응 (Impulse Phase: Reaction) ========== //
    /// <summary>
    /// 상대방의 행동(시위, 쿠데타 등)에 '대응(Reaction)'할지 여부를 선택합니다. [cite: 850]
    /// </summary>
    Reaction_ConfirmReaction,
    #endregion


    #region // ========== 정치 페이즈 (Politics Phase) ========== //
    /// <summary>
    /// 분할된 공간의 이슈를 획득했을 때, 두 정당이 합의하여 효과를 결정해야 합니다. (옵션 선택) [cite: 941]
    /// </summary>
    Politics_Issue_ChooseEffectOnDividedSpace,
    /// <summary>
    /// 황색 이슈 마커 획득 시, 해당 라운드 카드의 녹색 또는 황색 효과 중 하나를 선택합니다. [cite: 627, 943]
    /// </summary>
    Politics_Issue_ChooseYellowIssueEffect,
    /// <summary>
    /// 특정 사회 마커("기갑순양함", "노동복지") 효과로, 제거할 상대방의 의석을 선택합니다. [cite: 1653]
    /// </summary>
    Politics_Society_ChooseSeatToRemove,
    /// <summary>
    /// 소수 정당(DDP, DVP, USPD) 이슈 획득 시, 해당 소수 정당의 통제권을 넘겨줄 정당을 선택합니다. [cite: 1448, 1661]
    /// </summary>
    Politics_MinorParty_ChooseControllingPlayer,
    /// <summary>
    /// 정부 구성 시, 연정 제안을 수락할지 거절할지 선택합니다. [cite: 1036]
    /// </summary>
    Politics_Government_ConfirmCoalition,
    #endregion


    #region // ========== 카드 효과 (Card Effects) ========== //
    /// <summary>
    /// 카드 효과로 인해 특정 도시, 유닛, 마커, 플레이어 등을 선택해야 하는 일반적인 경우입니다.
    /// </summary>
    CardEffect_ChooseCity,
    CardEffect_ChooseUnit,
    CardEffect_ChooseMarker,
    CardEffect_ChoosePlayer,
    CardEffect_ChoosePartyBase,
    /// <summary>
    /// 'OR' 키워드가 있는 카드 효과에서, 두 가지 옵션 중 하나를 선택합니다. [cite: 758]
    /// </summary>
    CardEffect_ChooseBetweenTwoOptions,
    /// <summary>
    /// (Friedrich Ebert Dies 카드) 1차 투표 후, 결선에 진출한 두 후보 중 누구에게 투표할지 선택합니다. [cite: 1521]
    /// </summary>
    CardEffect_Election_ChooseCandidate,
    /// <summary>
    /// (The Strike on Prussia 카드) NSDAP 트랙에 피규어를 놓고 효과를 받을지, 아니면 포기할지 선택합니다. [cite: 1503, 1504]
    /// </summary>
    CardEffect_StrikeOnPrussia_ConfirmNSDAPPlacement,
    /// <summary>
    /// (Petition against Young Plan 카드) 영 플랜 반대 청원에 찬성(YES)할지 반대(NO)할지 투표합니다. [cite: 1538]
    /// </summary>
    CardEffect_YoungPlan_VoteYesOrNo,
    #endregion


    #region // ========== 특수 카드 (Special Cards) ========== //
    /// <summary>
    /// (Gustav Stresemann 카드) 자신 또는 타인의 주사위 굴림을 다시 굴리게 할지 여부를 선택합니다. [cite: 1453]
    /// </summary>
    Special_Stresemann_ConfirmReroll,
    /// <summary>
    /// (Reichspräsident 카드) 정당 카드 사용 후, 추가 '토론' 액션을 수행할지 여부를 선택합니다. [cite: 1472]
    /// </summary>
    Special_Reichsprasident_ConfirmExtraDebate,
    #endregion
}
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
    public Transform quickViewUnitContainer; // 퀵뷰에 유닛 아이콘이 생성될 부모 Transform
    public GameObject unitIconPrefab; // 유닛 아이콘 프리팹

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }



    public void ShowQuickView(CityModel cityModel, Vector3 position)
    {
        // 1. 기존 유닛 아이콘들 삭제
        foreach (Transform child in quickViewUnitContainer)
        {
            Destroy(child.gameObject);
        }

        // 2. CityModel 데이터로 새로운 유닛 아이콘 생성
        foreach (UnitModel unit in cityModel.GetUnitsInCity())
        {
            GameObject iconObj = Instantiate(unitIconPrefab, quickViewUnitContainer);
            // iconObj의 Image와 Text를 unit 데이터로 설정 (유닛 아이콘, 전투력 등)
        }
        // (기반(Base)도 필요하면 여기에 추가)

        // 3. 패널 위치 설정 및 활성화
        quickViewPanel.transform.position = position;
        quickViewPanel.SetActive(true);
    }

    public void HideQuickView()
    {
        quickViewPanel.SetActive(false);
    }
}

using UnityEngine;

/// <summary>
/// 게임의 주 화면 모드(지도 뷰, 의회 뷰)를 전환하는 것을 관리합니다.
/// </summary>
public class ViewModeManager : MonoBehaviour
{
    [Header("뷰 루트 오브젝트")]
    [SerializeField] private GameObject mapViewRoot; // 도시, 유닛, 마커 등을 담고 있는 부모 오브젝트
    [SerializeField] private GameObject politicsViewRoot; // 의회, 이슈 트랙 등을 담고 있는 부모 오브젝트

    void Start()
    {
        // 게임 시작 시 기본 뷰를 지도 뷰로 설정
        ShowMapView();
    }

    /// <summary>
    /// 지도 뷰를 보여줍니다.
    /// </summary>
    public void ShowMapView()
    {
        VisibilityManager.SetVisible(mapViewRoot, true);
        VisibilityManager.SetVisible(politicsViewRoot, false);
    }

    /// <summary>
    /// 의회 뷰를 보여줍니다.
    /// </summary>
    public void ShowPoliticsView()
    {
        VisibilityManager.SetVisible(mapViewRoot, false);
        VisibilityManager.SetVisible(politicsViewRoot, true);
    }
}

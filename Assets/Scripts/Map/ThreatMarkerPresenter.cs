using UnityEngine;

/// <summary>
/// 개별 위협 마커의 로직과 상호작용을 담당하는 Presenter입니다.
/// </summary>
public class ThreatMarkerPresenter
{
    public ThreatMarkerModel Model { get; private set; }
    public ThreatMarkerView View { get; private set; }

    public ThreatMarkerPresenter(ThreatMarkerModel model, ThreatMarkerView view)
    {
        Model = model;
        View = view;
        View.Initialize(this); // View에게 자신(Presenter)을 알려줌
    }

    /// <summary>
    /// Model의 현재 상태를 기반으로 View의 외형을 업데이트합니다.
    /// </summary>
    public void UpdateView()
    {
        // 1. Model로부터 현재 상태와 데이터를 읽어옵니다.
        var data = Model.Data;
        bool isFlipped = Model.IsFlipped;

        // 2. 어떤 Material을 사용할지 'Presenter'가 결정합니다.
        Material materialToUse;
        if (data.Category == ThreatMarkerData.MarkerCategory.TwoSidedPoverty)
        {
            materialToUse = isFlipped ? data.ProsperityMaterial : data.ThreatMaterial;
        }
        else if (data.Category == ThreatMarkerData.MarkerCategory.PartySpecificThreat)
        {
            materialToUse = isFlipped ? data.InactiveMaterial : data.ThreatMaterial;
        }
        else // OneSidedThreat
        {
            materialToUse = data.ThreatMaterial;
        }

        // 3. View에게는 최종 결정된 Material을 적용하라고 '지시'만 합니다.
        View.SetMaterial(materialToUse);
    }

    /// <summary>
    /// View로부터 클릭 이벤트를 받았을 때 호출될 메서드입니다.
    /// </summary>
    public void OnClicked()
    {
        Debug.Log($"{Model.InstanceId} 마커가 클릭되었습니다.");
        // TODO: 마커 선택과 관련된 게임 로직 실행
    }
}
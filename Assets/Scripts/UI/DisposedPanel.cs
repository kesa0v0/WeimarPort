using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DisposedPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Summary UI")]
    [SerializeField] private TextMeshProUGUI totalCountText;

    [Header("Popup UI")]
    [SerializeField] private GameObject popupRoot; // enable/disable on hover
    [SerializeField] private Transform popupUnitContainer; // where unit icons are instantiated
    [SerializeField] private GameObject unitIconPrefab; // should contain UnitIconView

    public void Redraw()
    {
        var bin = DisposedBin.Instance;
        if (bin == null) return;

        // aggregate by UnitData
        var byData = new Dictionary<UnitData, int>();
        int total = 0;
        foreach (var kv in bin.ContainedUnits)
        {
            var presenter = kv.Key;
            var count = kv.Value;
            if (presenter == null || presenter.Model == null || presenter.Model.Data == null) continue;
            total += count;
            var data = presenter.Model.Data;
            if (!byData.ContainsKey(data)) byData[data] = 0;
            byData[data] += count;
        }

        if (totalCountText != null) totalCountText.text = total.ToString();

        // rebuild popup content
        if (popupUnitContainer != null)
        {
            foreach (Transform child in popupUnitContainer)
                Destroy(child.gameObject);

            if (unitIconPrefab != null)
            {
                foreach (var entry in byData)
                {
                    var item = Instantiate(unitIconPrefab, popupUnitContainer);
                    var iconView = item.GetComponent<UnitIconView>();
                    if (iconView != null)
                    {
                        iconView.Setup(entry.Key, UnitIconView.HorizontalPortion.LeftHalf, entry.Value);
                    }
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (popupRoot != null) popupRoot.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (popupRoot != null) popupRoot.SetActive(false);
    }
}

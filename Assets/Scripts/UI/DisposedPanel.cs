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
        var byData = new Dictionary<UnitModel, int>();
        int total = 0;
        foreach (UnitModel kv in bin.GetUnits())
        {
            if (byData.ContainsKey(kv))
                byData[kv]++;
            else
                byData[kv] = 1;
            total++;
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
                        iconView.Setup(entry.Key.Data, UnitIconView.HorizontalPortion.LeftHalf, entry.Value);
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

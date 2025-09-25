using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPartyStatusView : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI partyNameText;
    [SerializeField] private TextMeshProUGUI partyStatusText;
    [SerializeField] private TextMeshProUGUI partyAgendaText;
    [SerializeField] private TextMeshProUGUI partySubPartiesText;
    [SerializeField] private TextMeshProUGUI partyHeldCardsCountText;
    [SerializeField] private ScrollRect partyInSupplyUnitsScrollView;
    [SerializeField] private Transform partyInSupplyUnitsContent;
    [SerializeField] private GameObject inSupplyUnitItemPrefab;


    public System.Action OnClicked;


    public void OnPointerClick(PointerEventData eventData)
    {
        OnClicked?.Invoke();
    }

    public void SetPartyName(string name, Color color)
    {
        partyNameText.text = name;
        partyNameText.color = color;
    }

    public void SetPartyAgenda(string agenda, Color color)
    {
        partyAgendaText.text = agenda;
        partyAgendaText.color = color;
    }

    public void SetPartySubParties(string subParties)
    {
        partySubPartiesText.text = subParties;
    }

    public void SetPartyStatus(string status)
    {
        partyStatusText.text = status;
    }

    public void SetPartyHeldCardsCount(int timelineCards, int policyCards)
    {
        partyHeldCardsCountText.text = $"T {timelineCards} : {policyCards} P";
    }

    public void SetInSupplyUnits(List<UnitModel> units)
    {
        // Clear existing items
        foreach (Transform child in partyInSupplyUnitsContent)
        {
            Destroy(child.gameObject);
        }

        // Populate with new items
        foreach (var unit in units)
        {
            var item = Instantiate(inSupplyUnitItemPrefab, partyInSupplyUnitsContent);
            var unitImage = item.transform.Find("UnitIcon").GetComponent<Image>();
            var unitInfoText = item.transform.Find("UnitCountText").GetComponent<TextMeshProUGUI>();

            unitImage.sprite = unit.Data.unitSprite;
            unitInfoText.text = $"";
        }
    }
    
    public void SetHighlight(bool highlight)
    {
        // Implement highlight logic, e.g., change background color or add border
        var image = GetComponent<Image>();
        if (image != null)
        {
            image.color = highlight ? Color.yellow : Color.white; // Example: yellow highlight
        }
    }
}

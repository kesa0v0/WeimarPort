using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPartyStatusView : MonoBehaviour, IUIOtherPartyStatusView, IPointerClickHandler
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

    public void SetInSupplyUnits(Dictionary<UnitPresenter, int> units)
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
            var unitImage = item.transform.Find("UnitImage").GetComponent<Image>();
            var unitInfoText = item.transform.Find("UnitText").GetComponent<TextMeshProUGUI>();

            unitInfoText.text = $"{unit.Key}: {unit.Value}";
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


public interface IUIOtherPartyStatusView
{
    void SetPartyName(string name, Color color);
    void SetPartyStatus(string status);
    void SetPartyAgenda(string agenda, Color color);
    void SetInSupplyUnits(Dictionary<UnitPresenter, int> units);
}
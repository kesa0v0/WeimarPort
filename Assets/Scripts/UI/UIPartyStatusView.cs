using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPartyStatusView : MonoBehaviour, IUIOtherPartyStatusView
{
    [SerializeField] private TextMeshProUGUI partyNameText;
    [SerializeField] private TextMeshProUGUI partyStatusText;
    [SerializeField] private TextMeshProUGUI partyAgendaText;
    [SerializeField] private TextMeshProUGUI partySubPartiesText;
    [SerializeField] private ScrollRect partyPreservedUnitsScrollView;
    [SerializeField] private Transform partyPreservedUnitsContent;
    [SerializeField] private GameObject preservedUnitItemPrefab;

    
    public System.Action OnClicked;

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClicked?.Invoke();
    }

    public void SetPartyName(string name)
    {
        partyNameText.text = name;
    }

    public void SetPartyAgenda(string agenda)
    {
        partyAgendaText.text = agenda;
    }

    public void SetPartySubParties(string subParties)
    {
        partySubPartiesText.text = subParties;
    }

    public void SetPartyStatus(string status)
    {
        partyStatusText.text = status;
    }

    public void SetPreservedUnits(Dictionary<string, int> units)
    {
        // Clear existing items
        foreach (Transform child in partyPreservedUnitsContent)
        {
            Destroy(child.gameObject);
        }

        // Populate with new items
        foreach (var unit in units)
        {
            var item = Instantiate(preservedUnitItemPrefab, partyPreservedUnitsContent);
            var unitImage = item.transform.Find("ArmyImage").GetComponent<Image>();
            var unitInfoText = item.transform.Find("PreservedArmyInfoText").GetComponent<TextMeshProUGUI>();

            unitInfoText.text = $"{unit.Key}: {unit.Value}";
        }
    }
}


public interface IUIOtherPartyStatusView
{
    void SetPartyName(string name);
    void SetPartyStatus(string status);
    void SetPartyAgenda(string agenda);
    void SetPreservedUnits(System.Collections.Generic.Dictionary<string, int> units);
}
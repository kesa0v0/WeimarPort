using UnityEngine;

public class UIPartyStatusPresenter
{
    public UIPartyStatusModel model;
    public UIPartyStatusView view;

    public UIPartyStatusPresenter(UIPartyStatusModel model, UIPartyStatusView view)
    {
        this.model = model;
        this.view = view;
        UpdateView();
    }

    public void UpdateModel(string status, string agenda)
    {
        model.UpdateStatus(status);
        model.UpdateAgenda(agenda);
        UpdateView();
    }

    private void UpdateView()
    {
        view.SetPartyName(model.PartyName);
        view.SetPartyStatus(model.PartyStatus);
        view.SetPartyAgenda(model.PartyAgenda);
        view.SetPreservedUnits(model.PartyUnits);
    }
}

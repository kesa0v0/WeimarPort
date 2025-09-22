using System;
using System.Collections.Generic;

public enum SelectionType
{
    None,
    City,
    Unit
}

public class SelectionManager
{
    private static SelectionManager _instance;
    public static SelectionManager Instance => _instance ??= new SelectionManager();

    private SelectionType _type = SelectionType.None;

    // City selection state
    private Predicate<CityPresenter> _cityFilter;
    private int _cityTargetCount;
    private readonly List<CityPresenter> _cityChosen = new();
    private Action<List<CityPresenter>> _onCitiesSelected;
    private Action _onCanceled;

    // Unit selection state
    private Predicate<UnitPresenter> _unitFilter;
    private int _unitTargetCount;
    private readonly List<UnitPresenter> _unitChosen = new();
    private Action<List<UnitPresenter>> _onUnitsSelected;

    public bool IsActive => _type != SelectionType.None;
    public SelectionType CurrentType => _type;

    public void Cancel()
    {
        _type = SelectionType.None;
        _cityFilter = null; _unitFilter = null;
        _cityTargetCount = 0; _unitTargetCount = 0;
        _cityChosen.Clear(); _unitChosen.Clear();
        var cb = _onCanceled; _onCanceled = null; _onCitiesSelected = null; _onUnitsSelected = null;
        cb?.Invoke();
    }

    // City selection API
    public void RequestCitySelection(int count, Action<List<CityPresenter>> onSelected, Predicate<CityPresenter> filter = null, Action onCanceled = null)
    {
        _type = SelectionType.City;
        _cityTargetCount = Math.Max(1, count);
        _cityFilter = filter;
        _onCitiesSelected = onSelected;
        _onCanceled = onCanceled;
        _cityChosen.Clear();
        UIManager.Instance?.ShowActionPrompt("도시를 선택하세요...");
    }

    public void RequestSingleCity(Action<CityPresenter> onSelected, Predicate<CityPresenter> filter = null, Action onCanceled = null)
    {
        RequestCitySelection(1, list => onSelected?.Invoke(list[0]), filter, onCanceled);
    }

    public bool HandleCityClicked(CityPresenter city)
    {
        if (_type != SelectionType.City) return false;
        if (city == null) return true; // consume click
        if (_cityFilter != null && !_cityFilter(city)) return true; // invalid but consumed

        _cityChosen.Add(city);
        if (_cityChosen.Count >= _cityTargetCount)
        {
            var result = new List<CityPresenter>(_cityChosen);
            UIManager.Instance?.HideActionPrompt();
            Cancel(); // will clear and call _onCanceled, so capture first
            _onCitiesSelected?.Invoke(result);
        }
        return true; // consumed
    }

    // Unit selection API
    public void RequestUnitSelection(int count, Action<List<UnitPresenter>> onSelected, Predicate<UnitPresenter> filter = null, Action onCanceled = null)
    {
        _type = SelectionType.Unit;
        _unitTargetCount = Math.Max(1, count);
        _unitFilter = filter;
        _onUnitsSelected = onSelected;
        _onCanceled = onCanceled;
        _unitChosen.Clear();
        UIManager.Instance?.ShowActionPrompt("유닛을 선택하세요...");
    }

    public void RequestSingleUnit(Action<UnitPresenter> onSelected, Predicate<UnitPresenter> filter = null, Action onCanceled = null)
    {
        RequestUnitSelection(1, list => onSelected?.Invoke(list[0]), filter, onCanceled);
    }

    public bool HandleUnitClicked(UnitPresenter unit)
    {
        if (_type != SelectionType.Unit) return false;
        if (unit == null) return true;
        if (_unitFilter != null && !_unitFilter(unit)) return true;

        _unitChosen.Add(unit);
        if (_unitChosen.Count >= _unitTargetCount)
        {
            var result = new List<UnitPresenter>(_unitChosen);
            UIManager.Instance?.HideActionPrompt();
            Cancel();
            _onUnitsSelected?.Invoke(result);
        }
        return true;
    }
}

// 브리지: Esc로 Selection 취소
public class SelectionInputBridge : UnityEngine.MonoBehaviour
{
    private void Update()
    {
        if (SelectionManager.Instance.IsActive && UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Escape))
        {
            UIManager.Instance?.HideActionPrompt();
            SelectionManager.Instance.Cancel();
        }
    }
}

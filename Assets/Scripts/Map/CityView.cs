
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class CityView : MonoBehaviour, ICityView
{
    [SerializeField] private TextMeshProUGUI cityName;
    [SerializeField] private GameObject cityIndicator;
    [SerializeField] private GameObject seatGameObject;
    [SerializeField] private Transform seatParent;
    private readonly List<SeatView> seats = new();

    public void SetCityName(string cityName)
    {
        this.name = cityName;
        this.cityName.text = cityName;
    }

    public void SetPosition(Vector2 position)
    {
        transform.position = new Vector3(position.x, 0, position.y);
    }

    public void SetSeatsByCount(int seatCount)
    {
        // If exists, destroy old seats
        foreach (SeatView child in seats)
        {
            Destroy(child.gameObject);
        }
        seats.Clear();

        // Create new seats
        for (int i = 0; i < seatCount; i++)
        {
            SeatView seat = Instantiate(seatGameObject, seatParent).GetComponent<SeatView>();
            seat.transform.localPosition = new Vector2((i - (seatCount - 1) / 2.0f) * 0.5f, 0);
            seats.Add(seat);
        }

        // 좌석 개수에 따라 인디케이터 크기 자동 조절 (예: 최소 1, seatCount 1당 0.3씩 증가)
        float indicatorWidth = 2f + seatCount * 0.3f;
        SetCityIndicatorSize(indicatorWidth);
    }

    public void SetCityIndicatorSize(float size)
    {
        var currentScale = cityIndicator.transform.localScale;
        cityIndicator.transform.localScale = new Vector3(size, currentScale.y, currentScale.z);
    }

    public void UpdateSeatOccupancy(Dictionary<Party, int> occupiedBy)
    {
        int seatIndex = 0;
        foreach (var party in GameManager.Instance.gameState.allParties)
        {
            int count = occupiedBy.TryGetValue(party, out int c) ? c : 0;
            for (int i = 0; i < count; i++)
            {
                if (seatIndex < seats.Count)
                {
                    seats[seatIndex].SetColor(party.Data.factionColor);
                    seatIndex++;
                }
                else
                {
                    Debug.LogError("Not enough seats to display all parties.");
                    return;
                }
            }
        }

        // Clear remaining seats
        for (int i = seatIndex; i < seats.Count; i++)
        {
            seats[i].SetColor(Color.gray); // Default color for unoccupied seats
        }
    }

    public GameObject GetCityIndicator()
    {
        return cityIndicator;
    }

    public void ShowAsCandidate(bool isCandidate)
    {
        // 간단 구현: 인디케이터 활성/비활성 토글 또는 색 변경 가능
        if (cityIndicator != null)
            cityIndicator.SetActive(true); // ensure visible
        // 추가로 머터리얼 색을 바꾸는 등의 연출을 여기에 추가 가능
    }

    /*
    // 간단한 클릭 전달: collider + raycast 또는 EventSystem 환경에서 동작
    private void OnMouseDown()
    {
        var presenter = CityManager.Instance.GetCity(this.name);
        if (presenter != null)
        {
            GameManager.Instance?.OnCityClicked(presenter);
        }
    }
    */}


public interface ICityView
{
    void SetCityName(string cityName);
    void SetPosition(Vector2 position);
    void SetSeatsByCount(int seatCount);
    void UpdateSeatOccupancy(Dictionary<Party, int> occupiedBy);
    GameObject GetCityIndicator();
}
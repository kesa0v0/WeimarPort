
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class CityView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cityName;
    [SerializeField] private List<Transform> placementSlots;
    [SerializeField] private GameObject cityIndicator;
    [SerializeField] private GameObject baseGameObject;
    [SerializeField] private Transform baseParent;
    private readonly List<PartyBaseView> partyBases = new();

    private int nextSlotIndex = 0;

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
        foreach (PartyBaseView child in partyBases)
        {
            Destroy(child.gameObject);
        }
        partyBases.Clear();

        // Create new seats
        for (int i = 0; i < seatCount; i++)
        {
            PartyBaseView seat = Instantiate(baseGameObject, baseParent).GetComponent<PartyBaseView>();
            seat.transform.localPosition = new Vector2((i - (seatCount - 1) / 2.0f) * 0.5f, 0);
            partyBases.Add(seat);
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

    public void UpdateSeatOccupancy(Dictionary<FactionType, int> occupiedBy)
    {
        int seatIndex = 0;
        foreach (var party in GameManager.Instance.gameState.Parties)
        {
            int count = occupiedBy.TryGetValue(party.Data.factionType, out int c) ? c : 0;
            for (int i = 0; i < count; i++)
            {
                if (seatIndex < partyBases.Count)
                {
                    partyBases[seatIndex].SetColor(party.Data.factionColor);
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
        for (int i = seatIndex; i < partyBases.Count; i++)
        {
            partyBases[i].SetColor(Color.gray); // Default color for unoccupied seats
        }
    }
    

    #region Object Placement


    /// <summary>
    /// 특정 게임 오브젝트(View)를 이 도시의 자식으로 만들고 위치를 지정합니다.
    /// </summary>
    public void AddObjectToCity(Transform objectTransform)
    {
        objectTransform.SetParent(this.transform, true);

        if (placementSlots != null && placementSlots.Count > 0)
        {
            objectTransform.position = placementSlots[nextSlotIndex].position;
            nextSlotIndex = (nextSlotIndex + 1) % placementSlots.Count;
        }
        else
        {
            objectTransform.position = this.transform.position + (Vector3)(UnityEngine.Random.insideUnitCircle * 0.5f);
        }
    }
    
    
    /// <summary>
    /// 도시에서 객체를 제거하고 부모를 null로 설정합니다.
    /// </summary>
    public void RemoveObjectFromCity(Transform objectTransform)
    {
        objectTransform.SetParent(null);
    }

    #endregion

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
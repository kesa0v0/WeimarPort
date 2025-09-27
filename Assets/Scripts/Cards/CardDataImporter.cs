using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Weimar.CardEffects;

[InitializeOnLoad]
public class CardDataImporter
{
    private static readonly string JsonFilePath = Path.Combine(Application.dataPath, "Resources/Raw/cards.json");
    private static readonly string AssetPath = "Assets/Resources/Data/Cards";

    static CardDataImporter()
    {
        if (!SessionState.GetBool("CardDataImported", false))
        {
            ImportCardData();
            SessionState.SetBool("CardDataImported", true);
        }
    }

    [MenuItem("Weimar/Force Re-import Card Data")]
    public static void ImportCardData()
    {
        Debug.Log("Card Data 임포트를 시작합니다...");
        ClearExistingCardData();
        
        if (!File.Exists(JsonFilePath))
        {
            Debug.LogError($"JSON 파일을 찾을 수 없습니다: {JsonFilePath}");
            return;
        }
        string jsonText = File.ReadAllText(JsonFilePath);
        CardCollectionJson cardCollection = JsonConvert.DeserializeObject<CardCollectionJson>(jsonText);

        if (!Directory.Exists(AssetPath))
        {
            Directory.CreateDirectory(AssetPath);
        }

        // 이제 단일 리스트를 처리하는 하나의 메서드만 호출하면 됩니다.
        ProcessCardList(cardCollection.cards);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Card Data 임포트 완료!");
    }

    private static void ClearExistingCardData()
    {
        if (!Directory.Exists(AssetPath)) return;
        Debug.Log("기존 CardData 에셋을 삭제합니다...");
        string[] guids = AssetDatabase.FindAssets($"t:{nameof(CardData)}", new[] { AssetPath });
        foreach (string guid in guids)
        {
            AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(guid));
        }
    }

    // 이제 affiliation 파라미터가 필요 없어졌습니다.
    private static void ProcessCardList(List<CardJson> cardList)
    {
        if (cardList == null) return;

        foreach (var cardJson in cardList)
        {
            string assetFilePath = $"{AssetPath}/Card_{cardJson.cardId}.asset";
            CardData cardData = ScriptableObject.CreateInstance<CardData>();
            AssetDatabase.CreateAsset(cardData, assetFilePath);

            cardData.CardId = cardJson.cardId;
            
            // JSON의 문자열을 FactionType enum으로 파싱합니다.
            if (Enum.TryParse<FactionType>(cardJson.affiliation, true, out FactionType affiliation))
            {
                cardData.Affiliation = affiliation;
            }
            else
            {
                Debug.LogWarning($"알 수 없는 affiliation 값: '{cardJson.affiliation}' for card '{cardJson.cardId}'");
                cardData.Affiliation = FactionType.None;
            }
            
            cardData.CardName = cardJson.cardName;
            cardData.LargeCircleValue = cardJson.largeValue;
            cardData.SmallCircleValue = cardJson.smallValue;
            cardData.RemoveFromGameOnPlay = cardJson.removeFromGame;

            if (cardJson.eventScript != null && cardJson.eventScript.Count > 0)
            {
                cardData.EventScriptJson = JsonConvert.SerializeObject(cardJson.eventScript, Formatting.Indented);
            }
            else
            {
                cardData.EventScriptJson = "";
            }

            if (!string.IsNullOrEmpty(cardJson.imagePath))
            {
                cardData.CardImage = Resources.Load<Sprite>(cardJson.imagePath);
            }

            EditorUtility.SetDirty(cardData);
        }
    }

    // JSON 파싱을 위한 보조 클래스들
    private class CardCollectionJson
    {
        public List<CardJson> cards;
    }

    private class CardJson
    {
        public string cardId;
        public string affiliation; // affiliation 필드 추가
        public string cardName;
        public string imagePath;
        public int largeValue;
        public int smallValue;
        public bool removeFromGame;
        public List<CardEffect> eventScript;
    }
}

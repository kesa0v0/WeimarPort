
using System.Collections.Generic;

[System.Serializable]
    public class ParliamentModel
    {
        // 의회 의석 (Key: FactionType, Value: 의석 수)
        public Dictionary<FactionType, int> Seats = new Dictionary<FactionType, int>();
    }
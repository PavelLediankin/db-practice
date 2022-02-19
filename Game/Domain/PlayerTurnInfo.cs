using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Game.Domain
{
    public class PlayerTurnInfo
    {
        public Guid PlayerId
        {
            get;
            // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local For MongoDB
            private set;
        }

        [BsonElement]
        public PlayerDecision PlayerDecision { get; }

        [BsonElement]
        public readonly int Score;

        [BsonConstructor]
        public PlayerTurnInfo(Guid playerId, PlayerDecision playerDecision, int score)
        {
            PlayerId = playerId;
            PlayerDecision = playerDecision;
            Score = score;
        }
    }
}
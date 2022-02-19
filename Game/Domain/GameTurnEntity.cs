using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace Game.Domain
{
    public class GameTurnEntity
    {
        [BsonElement]
        public Guid Id
        {
            get;
            // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local For MongoDB
            private set;
        }
        [BsonElement]
        public Guid GameId
        {
            get;
            // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local For MongoDB
            private set;
        }
        [BsonIgnore]
        public IReadOnlyList<Player> Players { get; }
        [BsonElement]
        public Guid WinnerId
        {
            get;
            // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local For MongoDB
            private set;
        }

        [BsonElement]
        public readonly int Turn;

        public GameTurnEntity(Guid id,
            Guid gameId,
            Guid winnerId,
            int turnIndex,
            IReadOnlyList<Player> players
            )
            : this(Guid.Empty, gameId, winnerId,turnIndex)
        { }

        [BsonConstructor]
        public GameTurnEntity(Guid id,
            Guid gameId,
            Guid winnerId,
            int currentTurnIndex)
        {
            Id = id;
            GameId = gameId;
            WinnerId = winnerId;
           //Players = players;
            Turn = currentTurnIndex;
        }
    }
}
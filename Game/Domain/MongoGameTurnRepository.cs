using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoGameTurnRepository : IGameTurnRepository
    {
        private readonly IMongoCollection<GameTurnEntity> gameturnCollection;
        public const string CollectionName = "gameturns";

        public MongoGameTurnRepository(IMongoDatabase db)
        {
            gameturnCollection = db.GetCollection<GameTurnEntity>(CollectionName);
        }

        public GameTurnEntity Insert(GameTurnEntity turn)
        {
            gameturnCollection.InsertOne(turn);
            return turn;
        }

        public GameTurnEntity FindById(Guid turnId)
        {
            var filter = new BsonDocument("_id", turnId);
            return gameturnCollection.Find(filter).FirstOrDefault();
        }

        public List<GameTurnEntity> FindLastTurns(int count)
        {
            var filter = new BsonDocument();
            var all = gameturnCollection.CountDocuments(filter);
            return gameturnCollection.Find(filter)
                .Skip((int?)(all - count))
                .Limit(count)
                .ToList();
        }
    }
}
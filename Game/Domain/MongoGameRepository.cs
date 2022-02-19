using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoGameRepository : IGameRepository
    {
        private readonly IMongoCollection<GameEntity> gamesCollection;
        public const string CollectionName = "games";

        public MongoGameRepository(IMongoDatabase db)
        {
            gamesCollection = db.GetCollection<GameEntity>(CollectionName);

            //var options = new CreateIndexOptions { Unique = true };
            //gamesCollection.Indexes.CreateOne("{ Login : 1 }", options);
        }

        public GameEntity Insert(GameEntity game)
        {
            gamesCollection.InsertOne(game);
            return game;
        }

        public GameEntity FindById(Guid gameId)
        {
            var filter = new BsonDocument("_id", gameId);
            return gamesCollection.Find(filter).FirstOrDefault();
        }

        public void Update(GameEntity game)
        {
            var filter = new BsonDocument("_id", game.Id);
            gamesCollection.ReplaceOne(filter, game);
        }

        // Возвращает не более чем limit игр со статусом GameStatus.WaitingToStart
        public IList<GameEntity> FindWaitingToStart(int limit)
        {
            var filter = new BsonDocument("Status", GameStatus.WaitingToStart);
            return gamesCollection.Find(filter)
                .Limit(limit)
                .ToList();
        }

        // Обновляет игру, если она находится в статусе GameStatus.WaitingToStart
        public bool TryUpdateWaitingToStart(GameEntity game)
        {
            //TODO: Для проверки успешности используй IsAcknowledged и ModifiedCount из результата

            if (game.Status != GameStatus.Playing)
                return false;
            var filter = new BsonDocument("_id", game.Id);
            var actionResult = gamesCollection.ReplaceOne(filter, game);

            return actionResult.IsAcknowledged
               && actionResult.ModifiedCount > 0;
        }
    }
}
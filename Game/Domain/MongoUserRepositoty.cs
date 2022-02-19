using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoUserRepository : IUserRepository
    {
        private readonly IMongoCollection<UserEntity> userCollection;
        public const string CollectionName = "users";

        public MongoUserRepository(IMongoDatabase database)
        {
            userCollection = database.GetCollection<UserEntity>(CollectionName);

            var options = new CreateIndexOptions { Unique = true };
            userCollection.Indexes.CreateOne("{ Login : 1 }", options);


            //var indexKeysDefinition = Builders<UserEntity>.IndexKeys
            //    .Ascending(user => user.Login)
            //    ;
            //userCollection.Indexes.CreateOne
            //    (new CreateIndexModel<UserEntity>(indexKeysDefinition),
            //    new CreateOneIndexOptions {  });
        }

        public UserEntity Insert(UserEntity user)
        {
            userCollection.InsertOne(user);
            return user;
        }

        public UserEntity FindById(Guid id)
        {
            var filter = new BsonDocument("_id", id);
            return userCollection.Find(filter).FirstOrDefault();
        }

        public UserEntity GetOrCreateByLogin(string login)
        {
            try 
            { 
                return userCollection.FindOneAndUpdate<UserEntity>(u => 
                    u.Login == login, 
                    Builders<UserEntity>.Update
                        .SetOnInsert(u => u.Id, Guid.NewGuid())
                        .Set("Login", login), 
                    new FindOneAndUpdateOptions<UserEntity, UserEntity> 
                    { 
                        IsUpsert = true, ReturnDocument = ReturnDocument.After 
                    }); 
            }
            catch (MongoCommandException e) when (e.Code == 11000) 
            { 
                return userCollection
                    .FindSync(u => u.Login == login)
                    .First();
            }
        }

        public void Update(UserEntity user)
        {
            var filter = new BsonDocument("_id", user.Id);
            userCollection.ReplaceOne(filter, user);
        }

        public void Delete(Guid id)
        {
            var filter = new BsonDocument("_id", id);
            userCollection.DeleteOne(filter);
        }

        // Для вывода списка всех пользователей (упорядоченных по логину)
        // страницы нумеруются с единицы
        public PageList<UserEntity> GetPage(int pageNumber, int pageSize)
        {
            //TODO: Тебе понадобятся SortBy, Skip и Limit
            var filter = new BsonDocument();
            var count = userCollection.CountDocuments(filter);
            List<UserEntity> pageList;

            pageList = userCollection.Find(filter)
                .SortBy(user => user.Login)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToList();
            
            return new PageList<UserEntity>(pageList, count, pageNumber, pageSize);
        }

        // Не нужно реализовывать этот метод
        public void UpdateOrInsert(UserEntity user, out bool isInserted)
        {
            throw new NotImplementedException();
        }
    }
}
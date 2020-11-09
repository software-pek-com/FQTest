using System;
using System.Collections.Generic;
using System.Configuration;
using MongoDB.Driver;
using MoWebApp.Core;
using MoWebApp.Data;
using MoWebApp.Documents;

namespace MoWebApp.Services
{
    /// <summary>
    /// Represents data actions on the <see cref="User"/> collection in a MongoDB.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IMongoClient client;

        /// <summary>
        /// Creates an instance of this class.
        /// </summary>
        public UserService(IMongoClient client)
        {
            Guard.ArgumentNotNull(client, nameof(client));

            this.client = client;
        }

        /// <summary>
        /// Returns all known <see cref="IEnumerable<UserDocument>">users</see>.
        /// </summary>
        public IEnumerable<User> GetAll()
        {
            var databaseName = (string)ConfigurationManager.GetSection("Database:Name");
            var database = client.GetDatabase(databaseName);

            var collection = database.GetCollection<User>(nameof(User));
            var users = collection.Find(_ => true);

            return users.ToEnumerable();
        }

        /// <summary>
        /// Returns all <see cref="IEnumerable<UserDocument>">users</see>.
        /// </summary>
        public IEnumerable<UserDetails> GetById(string id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns all <see cref="IEnumerable<UserDocument>">users</see> matching <paramref name="filter"/>.
        /// </summary>
        public IEnumerable<UserSummary> Find(UserSearchFilter filter, UserSearchOrderBy orderBy)
        {
            throw new NotImplementedException();
        }
    }
}

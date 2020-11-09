using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
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
        private readonly IMapper mapper;

        /// <summary>
        /// Creates an instance of this class.
        /// </summary>
        public UserService(IMongoClient client, IMapper mapper)
        {
            Guard.ArgumentNotNull(client, nameof(client));
            Guard.ArgumentNotNull(mapper, nameof(mapper));

            this.client = client;
            this.mapper = mapper;
        }

        /// <summary>
        /// Returns all known <see cref="IEnumerable<UserDocument>">users</see>.
        /// </summary>
        public IEnumerable<User> GetAll()
        {
            var queryable = GetUserQueryable();
            return queryable.Where(_ => true).ToList();
        }

        /// <summary>
        /// Returns the <see cref="UserDetails">user</see> with the <paramref name="id"/>.
        /// </summary>
        public UserDetails GetById(string id)
        {
            var queryable = GetUserQueryable();
            var user = queryable.Where(u => u.Id == id).ToList().FirstOrDefault();

            if (user == null)
            {
                return null;
            }

            return mapper.Map<UserDetails>(user);
        }

        /// <summary>
        /// Returns all <see cref="IEnumerable<UserDocument>">users</see> matching <paramref name="filter"/>.
        /// </summary>
        public IEnumerable<UserSummary> Find(UserSearchFilter filter, UserSearchOrderBy orderBy)
        {
            var queryable = GetUserQueryable();

            var results = queryable.Where(u =>
                !string.IsNullOrEmpty(filter.FirstName) && u.FirstName.StartsWith(filter.FirstName)
                || !string.IsNullOrEmpty(filter.LastName) && u.LastName.StartsWith(filter.LastName)
                || filter.HasUserEverConnected && u.LastConnectionDate == null
                );

            if (orderBy.LastConnectionDate)
            {
                results.OrderBy(u => u.LastConnectionDate);
            }
            if (orderBy.CreationDate)
            {
                results.OrderBy(u => u.Audit.CreationDate);
            }
            if (orderBy.LastName)
            {
                results.OrderBy(u => u.LastName);
            }

            var summary = results.Select(r => mapper.Map<UserSummary>(r));
            return summary;
        }

        #region Private

        /// <remarks>
        /// Protected virtual for unit tests.
        /// </remarks>
        //protected virtual IMongoQueryable<User> GetUserQueryable()
        protected virtual IQueryable<User> GetUserQueryable()
        {
            var databaseName = (string)ConfigurationManager.GetSection("Database:Name");
            var database = client.GetDatabase(databaseName);

            return database.GetCollection<User>(nameof(User)).AsQueryable();
        }

        #endregion
    }
}

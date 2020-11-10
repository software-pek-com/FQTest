using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using AutoMapper;
using Microsoft.Extensions.Options;
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
        private readonly AppSettings settings;
        private readonly IMongoClient client;
        private readonly IMapper mapper;

        /// <summary>
        /// Creates an instance of this class.
        /// </summary>
        public UserService(IOptions<AppSettings> settings, IMongoClient client, IMapper mapper)
        {
            Guard.ArgumentNotNull(settings, nameof(settings));
            Guard.ArgumentNotNull(client, nameof(client));
            Guard.ArgumentNotNull(mapper, nameof(mapper));

            this.settings = settings.Value;
            this.client = client;
            this.mapper = mapper;
        }

        #region IUserService

        /// <summary>
        /// Returns all known <see cref="IEnumerable<UserDocument>">users</see>.
        /// </summary>
        public IEnumerable<User> GetAll()
        {
            var collection = AsCollection();
            return collection.Find(_ => true).ToList();
        }

        /// <summary>
        /// Returns the <see cref="UserDetails">user</see> with the <paramref name="id"/>.
        /// </summary>
        public UserDetails GetById(string id)
        {
            Guard.ArgumentNotNullOrEmpty(id, nameof(id));

            var user = AsQueryable().Where(u => u.Id == id).ToList().FirstOrDefault();

            if (user == null)
            {
                return null;
            }

            return mapper.Map<UserDetails>(user);
        }

        /// <summary>
        /// Returns all <see cref="IEnumerable<UserDocument>">users</see> matching <paramref name="filter"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="string.StartsWith"/> is used for matching.
        /// </remarks>
        public IEnumerable<UserSummary> Find(UserSearchFilter filter, UserSearchOrderBy orderBy)
        {
            Guard.ArgumentNotNull(filter, nameof(filter));
            Guard.ArgumentNotNull(orderBy, nameof(orderBy));

            var queryable = AsQueryable();

            // Assuming filter conditions are or'd as most reasonable.
            // Any other filtering would have to be defined at the requirements level.
            var results = queryable.Where(u =>
                (!string.IsNullOrEmpty(filter.FirstName) && u.FirstName.StartsWith(filter.FirstName))
                || (!string.IsNullOrEmpty(filter.LastName) && u.LastName.StartsWith(filter.LastName))
                || (filter.HasUserEverConnected && u.LastConnectionDate != null)
                || (!filter.HasUserEverConnected && u.LastConnectionDate == null)
                );

            // Assuming the orderings are mutually exclusive.
            // Any other logic would have to be defined at the requirements level.
            if (orderBy.LastConnectionDate)
            {
                results.OrderBy(u => u.LastConnectionDate);
            }
            else if (orderBy.CreationDate)
            {
                results.OrderBy(u => u.Audit.CreationDate);
            }
            else if (orderBy.LastName)
            {
                results.OrderBy(u => u.LastName);
            }

            var summary = results.ToList().Select(r => mapper.Map<UserSummary>(r));
            return summary;
        }

        /// <summary>
        /// Deletes user with <paramref name="id"/>. Returns true if user deleted and false otherwise.
        /// </summary>
        public bool Delete(string id)
        {
            Guard.ArgumentNotNullOrEmpty(id, nameof(id));

            var collection = AsCollection();

            Expression<Func<User, bool>> filterExpression = u => u.Id == id;

            var result = collection.DeleteOne(filterExpression, CancellationToken.None);

            return result.DeletedCount == 1;
        }

        #endregion

        #region Private

        /// <remarks>
        /// Protected virtual for unit tests.
        /// </remarks>
        protected virtual IMongoCollection<User> AsCollection()
        {
            var database = client.GetDatabase(settings.DbName);

            return database.GetCollection<User>(nameof(User));
        }

        /// <remarks>
        /// Protected virtual for unit tests.
        /// </remarks>
        protected virtual IQueryable<User> AsQueryable()
        {
            return AsCollection().AsQueryable();
        }

        #endregion
    }
}

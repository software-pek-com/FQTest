using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading;
using AutoMapper;
using Microsoft.AspNetCore.Http;
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
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly AppSettings settings;
        private readonly IMongoClient client;
        private readonly IMapper mapper;

        /// <summary>
        /// Creates an instance of this class.
        /// </summary>
        public UserService(IHttpContextAccessor httpContextAccessor, IOptions<AppSettings> settings, IMongoClient client, IMapper mapper)
        {
            Guard.ArgumentNotNull(httpContextAccessor, nameof(httpContextAccessor));
            Guard.ArgumentNotNull(settings, nameof(settings));
            Guard.ArgumentNotNull(client, nameof(client));
            Guard.ArgumentNotNull(mapper, nameof(mapper));

            this.httpContextAccessor = httpContextAccessor;
            this.settings = settings.Value;
            this.client = client;
            this.mapper = mapper;
        }

        #region IUserService

        /// <summary>
        /// Returns all known <see cref="IEnumerable<UserDocument>">users</see>.
        /// </summary>
        /// <remarks>
        /// Leaving this method in for development and debugging.
        /// </remarks>
        public IEnumerable<User> GetAll()
        {
            var collection = AsCollection();
            return collection.Find(_ => true).ToList();
        }

        /// <summary>
        /// Creates a new <paramref name="user"/>.
        /// </summary>
        public void Create(UserDetails user)
        {
            Guard.ArgumentNotNull(user, nameof(user));

            var collection = AsCollection();

            var userData = mapper.Map<User>(user);

            userData.Audit.CreationDate = DateTime.UtcNow;
            userData.Audit.CreationUser = GetCurrentUserName();

            collection.InsertOne(userData);
        }

        /// <summary>
        /// Updates the <paramref name="user"/>.
        /// </summary>
        /// <remarks>
        /// This is not entirely correct implementation because we always replace
        /// user data with what we get in <paramref name="user"/>. A correct implementation
        /// (not done because of lack of time) would need to detect what in <paramref name="user"/>
        /// has changed to ensure that it is correctly 'blended' with existing data (which is not being
        /// updated).
        /// </remarks>
        public void Update(UserDetails user)
        {
            Guard.ArgumentNotNull(user, nameof(user));

            var id = user.Id;
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("User Id cannot be null or empty.");
            }

            var collection = AsCollection();
            var currentData = collection.Find(u => u.Id == id).First();

            var updateData = mapper.Map<User>(user);
            updateData.Audit.CreationDate = currentData.Audit.CreationDate;
            updateData.Audit.CreationUser = currentData.Audit.CreationUser;
            updateData.Audit.LastUpdateDate = DateTime.UtcNow;
            updateData.Audit.LastUpdateUser = GetCurrentUserName();

            var result = collection.ReplaceOne(u => u.Id == id, updateData);
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

        /// <summary>
        /// Authentication is not enabled so we do not have the current user's Identity/Name.
        /// Normally, current user's Identity is part of the context.
        /// </summary>
        private string GetCurrentUserName()
        {
            return "admin";
            //return httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

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

using System.Collections.Generic;
using System.Linq;
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
        /// <remarks>
        /// <see cref="string.StartsWith"/> is used for matching.
        /// </remarks>
        public IEnumerable<UserSummary> Find(UserSearchFilter filter, UserSearchOrderBy orderBy)
        {
            var queryable = GetUserQueryable();

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

        #region Private

        /// <remarks>
        /// Protected virtual for unit tests.
        /// </remarks>
        protected virtual IQueryable<User> GetUserQueryable()
        {
            var database = client.GetDatabase(settings.DbName);

            return database.GetCollection<User>(nameof(User)).AsQueryable();
        }

        #endregion
    }
}

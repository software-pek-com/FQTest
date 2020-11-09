using System.Collections.Generic;
using MoWebApp.Data;
using MoWebApp.Documents;

namespace MoWebApp.Services
{
    /// <summary>
    /// Represents data actions on the <see cref="User"/> collection.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Returns all <see cref="IEnumerable<UserDocument>">users</see>.
        /// </summary>
        IEnumerable<User> GetAll();

        /// <summary>
        /// Returns all <see cref="IEnumerable<UserDocument>">users</see>.
        /// </summary>
        UserDetails GetById(string id);

        /// <summary>
        /// Returns all <see cref="IEnumerable<UserDocument>">users</see> matching <paramref name="filter"/>.
        /// </summary>
        IEnumerable<UserSummary> Find(UserSearchFilter filter, UserSearchOrderBy orderBy);
    }
}
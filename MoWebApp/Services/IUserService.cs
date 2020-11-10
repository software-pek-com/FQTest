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
        /// <remarks>
        /// Leaving this method in for development and debugging.
        /// </remarks>
        IEnumerable<User> GetAll();

        /// <summary>
        /// Creates a new <paramref name="user"/>.
        /// </summary>
        void Create(UserDetails user);

        /// <summary>
        /// Updates the <paramref name="user"/>.
        /// </summary>
        void Update(UserDetails user);

        /// <summary>
        /// Returns all <see cref="IEnumerable<UserDocument>">users</see>.
        /// </summary>
        UserDetails GetById(string id);

        /// <summary>
        /// Returns all <see cref="IEnumerable<UserDocument>">users</see> matching <paramref name="filter"/>.
        /// </summary>
        IEnumerable<UserSummary> Find(UserSearchFilter filter, UserSearchOrderBy orderBy);

        /// <summary>
        /// Deletes user with <paramref name="id"/>. Returns true if user deleted and false otherwise.
        /// </summary>
        bool Delete(string id);
    }
}
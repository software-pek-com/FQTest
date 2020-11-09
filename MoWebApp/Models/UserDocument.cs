using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MoWebApp.Models
{
    public class UserDocument
    {
        public UserDocument()
        {
            Audit = new AuditElement();
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRequired]
        public string FirstName { get; set; }
        
        [BsonRequired]
        public string LastName { get; set; }

        /// <summary>
        /// Must be unique in the database.
        /// </summary>
        [BsonRequired]
        public string UserName { get; set; }

        /// <remarks>
        /// Details of how to use the preferable <see cref="DateTimeOffset"/> in MongoDB queries
        /// are <see cref="https://stackoverflow.com/questions/10480127/mongodb-and-datetimeoffset-type">here</see>.
        /// </remarks>
        public DateTime Birthdate { get; set; }

        public string Password { get; set; }

        [BsonRequired]
        public AuditElement Audit { get; set; }

        public DateTime LastConnectionDate { get; set; }
    }
}

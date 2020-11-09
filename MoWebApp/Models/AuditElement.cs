using System;
using MongoDB.Bson.Serialization.Attributes;

namespace MoWebApp.Models
{
    public class AuditElement
    {
        [BsonRequired]
        public DateTime CreationDate { get; set; }

        [BsonRequired]
        public string CreationUser { get; set; }

        public DateTime LastUpdateDate { get; set; }

        public string LastUpdateUser { get; set; }
    }
}

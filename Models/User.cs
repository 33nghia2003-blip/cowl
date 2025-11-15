using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace cowl.Models;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("username")]
    public string Username { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("passwordHash")]
    public string PasswordHash { get; set; } = string.Empty;

    [BsonElement("fullName")]
    public string FullName { get; set; } = string.Empty;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("lastLogin")]
    public DateTime? LastLogin { get; set; }

    [BsonElement("isActive")]
    public bool IsActive { get; set; } = true;
}

public class Company
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("userId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("companyName")]
    public string CompanyName { get; set; } = string.Empty;

    [BsonElement("representativeName")]
    public string RepresentativeName { get; set; } = string.Empty;

    [BsonElement("phoneNumber")]
    public string PhoneNumber { get; set; } = string.Empty;

    [BsonElement("address")]
    public string Address { get; set; } = string.Empty;

    [BsonElement("status")]
    public string Status { get; set; } = string.Empty;

    [BsonElement("businessSector")]
    public string BusinessSector { get; set; } = string.Empty;

    [BsonElement("activeDate")]
    public string ActiveDate { get; set; } = string.Empty;

    [BsonElement("hasAppointment")]
    public bool HasAppointment { get; set; } = false;

    [BsonElement("isConsidering")]
    public bool IsConsidering { get; set; } = false;

    [BsonElement("noNeed")]
    public bool NoNeed { get; set; } = false;

    [BsonElement("note")]
    public string Note { get; set; } = string.Empty;

    [BsonElement("appointmentDate")]
    public DateTime? AppointmentDate { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

using NHibernate.Engine;
using System.ComponentModel.DataAnnotations;

namespace luanvanthacsi.Api.Entities
{
    public class Scientist
    {
        [Key]
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SqlBuilder.Sample.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("username")]
        [StringLength(50)]
        public string Username { get; set; }

        [Column("password")]
        [StringLength(50)]
        public string Password { get; set; }
    }
}
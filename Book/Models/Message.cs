using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Book.Models
{
    public class Message
    {
        public int Id { get; set; }

        [Required]
        [Column("UserId")]     
        public int Id_User { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public DateTime MessageDate { get; set; }

        [ForeignKey("Id_User")]
        public User User { get; set; }
    }
}

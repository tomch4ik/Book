using System.ComponentModel.DataAnnotations;

namespace Book.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; }

        [Required]
        public string Pwd { get; set; }

        [Required]
        public string Salt { get; set; }
    }
}


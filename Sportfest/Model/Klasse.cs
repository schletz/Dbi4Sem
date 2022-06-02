using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sportfest.Model
{
    [Table("Klasse")]
    public class Klasse
    {
        [Key]
        [MaxLength(16)]
        public string Name { get; set; }
        public int Jahrgang { get; set; }
        [Required]
        [MaxLength(255)]
        public string Abteilung { get; set; }
    }
}

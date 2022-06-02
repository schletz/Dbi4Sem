using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sportfest.Model
{
    [Table("Ergebnis")]
    public class Ergebnis
    {
        [Required]
        public Schueler Schueler { get; set; }
        [Required]
        public Bewerb Bewerb { get; set; }
        public int Durchgang { get; set; }
        [Column(TypeName = "DECIMAL(6,2)")]
        public decimal Zeit { get; set; }
    }
}

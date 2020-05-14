using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Sportfest.Model
{
    [Table("Schueler")]
    public class Schueler
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SchuelerId { get; set; }
        [Required]
        [MaxLength(255)]
        public string Vorname { get; set; }
        [Required]
        [MaxLength(255)]
        public string Zuname { get; set; }
        [Column(TypeName = "CHAR(1)")]
        public string Geschlecht { get; set; }
        [Required]
        public Klasse Klasse { get; set; }
    }
}

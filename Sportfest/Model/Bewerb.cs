using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sportfest.Model
{
    [Table("Bewerb")]
    public class Bewerb
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int BewerbId { get; set; }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
    }
}

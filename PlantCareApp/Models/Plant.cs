using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlantCareApp.Models
{
    public class Plant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PlantId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;  // Initialize Name to an empty string

        public string Description { get; set; } = string.Empty;  // Initialize Description to an empty string

        public ICollection<MaintenanceTask> MaintenanceTasks { get; set; } = new List<MaintenanceTask>(); // Initialize the collection
    }
}

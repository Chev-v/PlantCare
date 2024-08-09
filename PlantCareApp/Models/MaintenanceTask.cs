using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlantCareApp.Models
{
    public class MaintenanceTask
    {
        [Key]
        public int TaskId { get; set; }

        [Required]
        public string TaskType { get; set; } = string.Empty;  // Initialize TaskType to an empty string

        [Required]
        public DateTime Date { get; set; }

        [ForeignKey("Plant")]
        [Required]
        public int PlantId { get; set; }

        public Plant? Plant { get; set; }  // Make Plant nullable
    }
}

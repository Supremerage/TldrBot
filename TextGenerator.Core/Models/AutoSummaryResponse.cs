using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TextGenerator.Core.Models
{
    public class AutoSummaryResponse
    {
        [Required]
        [JsonPropertyName("summary_text")]
        public string SummaryText { get; set; }
    }
}

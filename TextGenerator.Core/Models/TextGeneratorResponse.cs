using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TextGenerator.Core.Models
{
    public class TextGeneratorResponse
    {
        [Required]
        [JsonPropertyName("generated_text")]
        public string GeneratedText { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MealManagement.Core.DTOs
{
    public class SendMessageDto
    {
        [Required(ErrorMessage = "לא ניתן לשלוח הודעה ריקה")]
        [StringLength(1000)]
        public string Content { get; set; } = string.Empty;
    }
}
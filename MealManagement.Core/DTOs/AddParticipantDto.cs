using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealManagement.Core.DTOs
{
    public class AddParticipantDto
    {
        [Required(ErrorMessage = "יש להזין אימייל")]
        [EmailAddress(ErrorMessage = "כתובת אימייל לא תקינה")]
        public string Email { get; set; } = string.Empty;
    }
}

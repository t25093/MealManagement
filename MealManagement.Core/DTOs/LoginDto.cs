using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MealManagement.Core.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "יש להזין אימייל")]
        [EmailAddress(ErrorMessage = "כתובת אימייל לא תקינה")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "יש להזין סיסמה")]
        public string Password { get; set; } = string.Empty;
    }
}
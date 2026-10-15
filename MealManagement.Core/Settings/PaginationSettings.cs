using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace MealManagement.Core.Settings
{
    public class PaginationSettings
    {
        public const string SectionName = "Pagination";
        
        [Range(1, 100)]
        public int DefaultPageSize { get; set; }
        
        [Range(1, 100)]
        public int MaxPageSize { get; set; }
    }
}

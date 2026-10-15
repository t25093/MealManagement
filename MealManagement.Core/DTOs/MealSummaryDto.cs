using MealManagement.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MealManagement.Core.Enums;

namespace MealManagement.Core.DTOs
{
    public class MealSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public MealType MealType { get; set; }
        public DateTime EventDate { get; set; }
        public int GuestsCount { get; set; }
        public string OwnerName { get; set; } = string.Empty;

        // אחוז המנות שכבר נלקחו (0 עד 100)
        public double CoveragePercent { get; set; }
    }
}
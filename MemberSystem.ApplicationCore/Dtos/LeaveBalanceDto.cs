using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemberSystem.ApplicationCore.Dtos
{
    public class LeaveBalanceDto
    {
        public int MemberId { get; set; }

        public int LeaveTypeId { get; set; }

        public int Year { get; set; }

        public decimal RemainingDays { get; set; }
    }
}

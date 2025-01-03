using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemberSystem.ApplicationCore.Dtos
{
    public class EditLeaveRequestStatusDto
    {
        public int ApproverId { get; set; }

        public int LeaveRequestId { get; set; }

        public int MemberId { get; set; }

        public string Status { get; set; }

    }
}

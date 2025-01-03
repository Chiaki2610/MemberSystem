using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemberSystem.ApplicationCore.Dtos
{
    public class ApprovalFlowDto
    {
        public int ApprovalOrder { get; set; }
        public string FlowDescription { get; set; }
        public string ApprovalStatus { get; set; }
        public DateTime? ApprovalTime { get; set; }
        public string ApproverName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubAdminPanel.Core.DTOs
{
    public class TokenRequestDto
    {
        public string Name { get; set; } 
        public int ExpireDays { get; set; } 
    }
}

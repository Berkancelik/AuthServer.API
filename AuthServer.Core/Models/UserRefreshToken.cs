﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Models
{
    class UserRefreshToken
    {
        public string RefreshToken { get; set; }
        public string Code { get; set; }
        public DateTime Expiration { get; set; }
    }
}

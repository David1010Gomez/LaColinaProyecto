﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ColinaApplication.Dian.Entity
{
    public class Token
    {
        public string access_token { get; set; }

        public int expires_in { get; set; }

        public string token_type { get; set; }

        public string scope { get; set; }
        
    }
}
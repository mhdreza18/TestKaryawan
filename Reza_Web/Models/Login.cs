using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Reza_Web.Models
{
    public class Login
    {
        public string CardCode { get; set; }
        public string DAPCardCode { get; set; }
        public string CardName { get; set; }
        public string Password { get; set; }
        public string Privilege { get; set; }
        public string Address { get; set; }
    }

    
}
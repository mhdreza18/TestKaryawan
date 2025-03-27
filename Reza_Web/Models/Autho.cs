using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Reza_Web.Models
{
    public class Autho
    {
        public int Id { get; set; }
        public string DAPCardCode { get; set; }
        public string Alias { get; set; }
        public List<Children> ListChildren { get; set; }
        public int Status { get; set; }
        public string Module { get; set; }

        public class Children
        {
            public int Id { get; set; }
            public string SubAlias { get; set; }
            public int Status { get; set; }
            public string Module { get; set; }
        }

    }

    public class AuthoUpdate
    {
        public string DAPCardCode { get; set; }
        public List<ChildrenUpdate> ListChildren { get; set; }
        public class ChildrenUpdate
        {
            public int Id { get; set; }
            public bool Status { get; set; }
        }

    }
}
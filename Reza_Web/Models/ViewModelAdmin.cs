using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Reza_Web.Models
{
    public class ViewModelAdmin
    {
       
        public List<Login> Admin { get; set; }
      
        public List<Autho> Autho { get; set; }
      
        public List<UserMaster> UserMaster { get; set; }

        public List<KaryawanMaster> KaryawanMaster { get; set; }


    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiEsca.Models
{
    public class ClsModResponse
    {
        public string MESSAGE { get; set; }
        public object ITEMS { get; set; }
        public bool SUCCESS { get; set; }
    }
}
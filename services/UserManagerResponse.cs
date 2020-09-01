using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mmt.Api.services
{
    public class UserManagerResponse
    {
        public string Result { get; set; }
        public bool IsSucces { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Functie { get; set; }
        public List<string> Roles { get; set; }


        public IEnumerable<string> Errors { get; set; }


        public DateTime? ExpireDate { get; set; }
    }
}

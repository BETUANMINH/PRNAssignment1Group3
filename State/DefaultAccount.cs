using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;

namespace WPFAssignment1Group3.State
{
    public class DefaultAccount
    {
        public int Id { get; set; } = 1;
        public string Username { get; set; }
        public string Password { get; set; }
        public int Role {  get; set; } = 0;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities
{
    public class User : IEntity
    {
        public Guid Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }   
        public string? Fullname { get; set; }  
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public UserType UserType { get; set; }
        public string? UserPictures { get; set; }
        public UserState UserState { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.DTOs
{
    public class AuthTokensDto : ICloneable
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpireOn { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public override bool Equals(object obj)
        {
            return AccessToken == ((AuthTokensDto)obj).AccessToken;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(AccessToken);
        }
    }
}

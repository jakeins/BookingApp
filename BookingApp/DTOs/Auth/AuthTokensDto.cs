using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.DTOs
{
    public class AuthTokensDto : ICloneable
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        public string RefreshToken { get; set; }
        public DateTime ExpireOn { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public override bool Equals(object obj)
        {
            var dto = obj as AuthTokensDto;
            return dto != null &&
                   AccessToken == dto.AccessToken &&
                   RefreshToken == dto.RefreshToken &&
                   ExpireOn.ToLongDateString() == dto.ExpireOn.ToLongDateString();
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(AccessToken, RefreshToken, ExpireOn);
        }
    }
}

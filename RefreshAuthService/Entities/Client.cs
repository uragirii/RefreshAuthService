using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RefreshAuthService.Entities
{
    public class Client
    {
        [Key]
        [Required]
        public string ClientId { get; set; }
        [Required]
        [MaxLength(100)]
        public string ClientSecret { get; set; }
    }
}
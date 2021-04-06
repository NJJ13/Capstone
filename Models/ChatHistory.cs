using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FreshAir.Models
{
    public class ChatHistory
    {
        [Key]
        public int ChatHistoryId { get; set; }
        public int EventId { get; set; }
        public string Message { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utb_sc_Domain.Entities
{
    public class Message
    {
        public int Id { get; set; } // Primární klíč

        [Required]
        public string Content { get; set; } // Obsah zprávy

        [Required]
        public DateTime SentAt { get; set; } // Datum a čas odeslání

        // Vztahy
        [Required]
        public int SenderId { get; set; } // ID odesílatele
        public User Sender { get; set; } // Navigační vlastnost k uživateli

        [Required]
        public int ReceiverId { get; set; } // ID příjemce
        public User Receiver { get; set; } // Navigační vlastnost k příjemci

        public int? ChatId { get; set; } // Volitelný vztah ke chatu
        public Chat Chat { get; set; }
    }
}

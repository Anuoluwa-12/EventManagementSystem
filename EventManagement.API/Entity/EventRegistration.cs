namespace EventManagement.API.Entity
{
        public class EventRegistration
        {
            public int Id { get; set; }

            public int UserId { get; set; }

            public User User { get; set; }

            public int EventId { get; set; }
            public int? TicketId { get; set; }
            public Ticket? Ticket { get; set; }
            public decimal AmountPaid { get; set; }
            public string PaymentStatus { get; set; }
            public string? PaymentReference { get; set; }

            public Event Event { get; set; }

            public DateTime RegistrationDate { get; set; }
        }
    }

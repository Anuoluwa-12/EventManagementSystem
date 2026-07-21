namespace EventManagement.API.Entity
{
        public class EventRegistration
        {
            public int Id { get; set; }

            public int UserId { get; set; }

            public User User { get; set; }

            public int EventId { get; set; }
            public int? TicketId { get; set; }
            public decimal UnitPrice { get; set; }
            public Ticket? Ticket { get; set; }
            public int Quantity { get; set; } = 1;
            public decimal AmountPaid { get; set; }
            public decimal TotalAmount { get; set; }
            public DateTime? PaidAt { get; set; }
            public DateTime? CancelledAt { get; set; }
            public DateTime? CreatedAt { get; set; }
            public string BookingStatus { get; set; } = "Pending";
            public string PaymentStatus { get; set; } = "Pending";
            public string? PaymentReference { get; set; }
            public Event Event { get; set; }
            public DateTime RegistrationDate { get; set; }
            public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
    }

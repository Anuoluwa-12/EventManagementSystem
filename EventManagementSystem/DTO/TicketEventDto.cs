namespace EventManagementSystem.DTO
{
    public class TicketEventDto
        {
            public int TicketId { get; set; }

            public int EventId { get; set; }

            public string EventTitle { get; set; }

            public DateTime EventDate { get; set; }

            public string Venue { get; set; }

            public string? EventImageUrl { get; set; }

            public string TicketNumber { get; set; }

            public DateTime PurchaseDate { get; set; }

            public string QRCodeUrl { get; set; }
        }
    }


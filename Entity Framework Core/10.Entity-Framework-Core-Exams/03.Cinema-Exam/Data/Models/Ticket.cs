using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Cinema.Data.Models
{
    public class Ticket
    {
        //•	Id – integer, Primary Key
        //•	Price – decimal (non-negative, minimum value: 0.01) (required)
        //•	CustomerId – integer, foreign key(required)
        //•	Customer – the customer’s ticket 
        //•	ProjectionId – integer, foreign key(required)
        //•	Projection – the projection’s ticket

        [Key]
        public int Id { get; set; }

        [Range(0.01, double.MaxValue), Required]
        public decimal Price { get; set; }

        [ForeignKey(nameof(Customer)), Required]
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        [ForeignKey(nameof(Projection)), Required]
        public int ProjectionId { get; set; }
        public Projection Projection { get; set; }
    }
}

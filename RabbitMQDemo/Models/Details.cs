namespace RabbitMQDemo.Models
{
    public class Details
    {
        public int messages_unacknowledged { get; set; }
        public int consumers { get; set; }

        public int TotalConsumers
        {
            get
            {
                return consumers - messages_unacknowledged;
            }
        }
    }
}

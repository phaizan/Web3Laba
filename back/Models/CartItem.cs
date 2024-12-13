namespace back.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        // Ссылка на корзину
        public int CartId { get; set; }
        public Cart Cart { get; set; }

        // Ссылка на тур
        public int TourId { get; set; }
        public Tour Tour { get; set; }

        // Количество туров в корзине
        public int Quantity { get; set; }

        // Итоговая стоимость для конкретного тура
        public decimal TotalPrice => Tour.Price * Quantity;
    }
}

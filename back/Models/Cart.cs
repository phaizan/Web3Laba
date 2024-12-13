namespace back.Models
{
    public class Cart
    {
        public int Id { get; set; }

        // Ссылка на пользователя
        public int UserId { get; set; }
        public User User { get; set; }

        // Список товаров в корзине
        public ICollection<CartItem> CartItems { get; set; }
    }
}

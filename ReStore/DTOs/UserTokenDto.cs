namespace ReStore.DTOs
{
    public class UserTokenDto
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public BasketDto Basket { get; set; }
    }
}

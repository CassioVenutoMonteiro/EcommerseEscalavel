namespace EcommerseEscalavel.DTOs
{
    public class SalesReportDto
    {
        public int TotalOrders { get; set; }
        public int TotalItemsSold { get; set; }
        public decimal TotalRevenue { get; set; }
        public TopProductDto TopProduct { get; set; }
    }
}

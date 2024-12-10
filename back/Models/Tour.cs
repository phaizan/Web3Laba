namespace back.Models
{
    public class Tour
    {
            public int Id { get; set; }
            public string LinkToCard { get; set; }
            public string TourName { get; set; }
            public string ShortDescription { get; set; }
            public string AmountofPlaces { get; set; }
            public decimal Price { get; set; }  
            public string Picture { get; set; }
            public string TourCategory { get; set; }    

            public string DepartureCity { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime FinishDate { get; set; }
    }
}

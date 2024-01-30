using Microsoft.EntityFrameworkCore;

namespace AlHersey.Models
{
    public class Search
    {
        public int KATEGORI { get; set; }
        public int URUN { get; set; }
        public int MARKA { get; set; }
        public string? ARAMAISMI { get; set; }
       
    }
}

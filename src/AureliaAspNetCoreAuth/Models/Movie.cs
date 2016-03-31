using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AureliaAspNetCoreAuth.Models
{
    public class Movie
    {
        public int id { get; set; }
        public string title { get; set; }
        public int releaseYear { get; set; }

        public Movie(int Id, string Title, int ReleaseYear)
        {
            id = Id;
            title = Title;
            releaseYear = ReleaseYear;
        }
    }
}

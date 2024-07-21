using System;

namespace AMEKSA.Entities
{
    public class ImcasComment
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string Comment { get; set; }

        public DateTime CommentDateTime { get; set; }
    }
}

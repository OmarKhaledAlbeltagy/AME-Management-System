using AMEKSA.Context;
using AMEKSA.Entities;
using AMEKSA.Models;
using System;

namespace AMEKSA.Repo
{
    public class ImcasCommentRep: IImcasCommentRep
    {
        private readonly DbContainer db;
        private readonly ITimeRep ti;

        public ImcasCommentRep(DbContainer db, ITimeRep ti)
        {
            this.db = db;
            this.ti = ti;
        }

        public bool AddComment(AddImcasComment obj)
        {
            DateTime now = ti.GetCurrentTime();

            ImcasComment res = new ImcasComment();
            res.FullName = obj.FullName;
            res.Comment = obj.Comment;
            res.CommentDateTime = now; 

            db.imcasComment.Add(res);
            db.SaveChanges();
            return true;
        }
    }
}

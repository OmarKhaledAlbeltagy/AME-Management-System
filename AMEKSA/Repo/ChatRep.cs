using AMEKSA.ChatCustomEntities;
using AMEKSA.Context;
using AMEKSA.Entities;
using AMEKSA.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
    public class ChatRep : IChatRep
    {
        private readonly DbContainer db;
        private readonly ITimeRep ti;

        public ChatRep(DbContainer db,ITimeRep ti)
        {
            this.db = db;
            this.ti = ti;
        }
        public bool AccountMedicalManagerComment(AccountMedicalVisitChat obj)
        {
            DateTime d = ti.GetCurrentTime();
            string repid = db.accountMedicalVisit.Find(obj.AccountMedicalVisitId).extendidentityuserid;
            AccountMedicalVisitChat x = new AccountMedicalVisitChat();
            x.ManagerId = obj.ManagerId;
            x.RepId = repid;
            x.AccountMedicalVisitId = obj.AccountMedicalVisitId;
            x.ManagerComment = obj.ManagerComment;
            x.ManagerCommentDateTime = d;
            db.accountMedicalVisitChat.Add(x);
            db.SaveChanges();
            return true;

        }

        public bool AccountMedicalRepReply(RepReplyModel obj)
        {
            DateTime d = ti.GetCurrentTime();
            AccountMedicalVisitChat ch = db.accountMedicalVisitChat.Find(obj.Id);
            ch.RepReply = obj.RepReply;
            ch.RepReplyDateTime = d;
            db.SaveChanges();
            return true;
        }

        public bool AccountSalesManagerComment(AccountSalesVisitChat obj)
        {
            DateTime d = ti.GetCurrentTime();
            AccountSalesVisit visit = db.accountSalesVisit.Find(obj.AccountSalesVisitId);
            string repid = visit.extendidentityuserid;
            AccountSalesVisitChat x = new AccountSalesVisitChat();
            x.ManagerId = obj.ManagerId;
            x.RepId = repid;
            x.AccountSalesVisitId = obj.AccountSalesVisitId;
            x.ManagerComment = obj.ManagerComment;
            x.ManagerCommentDateTime = d;
            db.accountSalesVisitChat.Add(x);
            db.SaveChanges();
            return true;
        }


        public bool AccountSupportiveManagerComment(AccountSupportiveVisitChat obj)
        {
            DateTime d = ti.GetCurrentTime();
            AccountSupportiveVisit visit = db.AccountSupportiveVisit.Find(obj.AccountSupportiveVisitId);
            string repid = visit.extendidentityuserid;
            AccountSupportiveVisitChat x = new AccountSupportiveVisitChat();
            x.ManagerId = obj.ManagerId;
            x.RepId = repid;
            x.AccountSupportiveVisitId = obj.AccountSupportiveVisitId;
            x.ManagerComment = obj.ManagerComment;
            x.ManagerCommentDateTime = d;
            db.AccountSupportiveVisitChat.Add(x);
            db.SaveChanges();
            return true;
        }

        public bool AccountSalesRepReply(RepReplyModel obj)
        {
            DateTime d = ti.GetCurrentTime();
            AccountSalesVisitChat ch = db.accountSalesVisitChat.Find(obj.Id);
            ch.RepReply = obj.RepReply;
            ch.RepReplyDateTime = d;
            db.SaveChanges();
            return true;
        }

        public bool ContactMedicalManagerComment(ContactMedicalVisitChat obj)
        {
            DateTime d = ti.GetCurrentTime();
            string repid = db.contactMedicalVisit.Find(obj.ContactMedicalVisitId).extendidentityuserid;
            ContactMedicalVisitChat x = new ContactMedicalVisitChat();
            x.ManagerId = obj.ManagerId;
            x.RepId = repid;
            x.ContactMedicalVisitId = obj.ContactMedicalVisitId;
            x.ManagerComment = obj.ManagerComment;
            x.ManagerCommentDateTime = d;
            db.contactMedicalVisitChat.Add(x);
            db.SaveChanges();
            return true;
        }

        public bool ContactMedicalRepReply(RepReplyModel obj)
        {
            DateTime d = ti.GetCurrentTime();
            ContactMedicalVisitChat ch = db.contactMedicalVisitChat.Find(obj.Id);
            ch.RepReply = obj.RepReply;
            ch.RepReplyDateTime = d;
            db.SaveChanges();
            return true;
        }

 

        public IEnumerable<CustomComments> GetCommentsByDateRep(SearchByDate obj)
        {
            List<CustomComments> res = new List<CustomComments>();

            IEnumerable<CustomComments> AccountMedical = db.accountMedicalVisitChat.Join(db.accountMedicalVisit, a => a.AccountMedicalVisitId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                ManagerId = a.ManagerId,
                RepId = a.RepId,
                ManagerComment = a.ManagerComment,
                RepReply = a.RepReply,
                ManagerCommentDateTime = a.ManagerCommentDateTime,
                RepReplyDateTime = a.RepReplyDateTime,
                VisitId = b.Id,
                OrgId = b.AccountId,
                VisitDate = b.VisitDate,
                VisitTime = b.VisitTime
            }).Join(db.account, a => a.OrgId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                ManagerId = a.ManagerId,
                RepId = a.RepId,
                ManagerComment = a.ManagerComment,
                RepReply = a.RepReply,
                ManagerCommentDateTime = a.ManagerCommentDateTime,
                RepReplyDateTime = a.RepReplyDateTime,
                VisitId = a.VisitId,
                OrgId = a.OrgId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                OrgName = b.AccountName
            }).Join(db.Users, a => a.ManagerId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                ManagerId = a.ManagerId,
                RepId = a.RepId,
                ManagerComment = a.ManagerComment,
                RepReply = a.RepReply,
                ManagerCommentDateTime = a.ManagerCommentDateTime,
                RepReplyDateTime = a.RepReplyDateTime,
                VisitId = a.VisitId,
                OrgId = a.OrgId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                OrgName = a.OrgName,
                ManagerName = b.FullName
            }).Join(db.Users, a => a.RepId, b => b.Id, (a, b) => new CustomComments
            {
                Id = a.Id,
                ManagerId = a.ManagerId,
                RepId = a.RepId,
                ManagerComment = a.ManagerComment,
                RepReply = a.RepReply,
                ManagerCommentDateTime = a.ManagerCommentDateTime,
                RepReplyDateTime = a.RepReplyDateTime,
                VisitId = a.VisitId,
                OrgId = a.OrgId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                OrgName = a.OrgName,
                ManagerName = a.ManagerName,
                RepName = b.FullName,
                VisitType = 1
            }).Where(x => x.RepId == obj.UserId && x.ManagerCommentDateTime >= obj.Start && x.ManagerCommentDateTime <= obj.End);

            foreach (var item in AccountMedical)
            {
                res.Add(item);
            }

            IEnumerable<CustomComments> AccountSales = db.accountSalesVisitChat.Join(db.accountSalesVisit, a => a.AccountSalesVisitId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                ManagerId = a.ManagerId,
                RepId = a.RepId,
                ManagerComment = a.ManagerComment,
                RepReply = a.RepReply,
                ManagerCommentDateTime = a.ManagerCommentDateTime,
                RepReplyDateTime = a.RepReplyDateTime,
                VisitId = b.Id,
                OrgId = b.AccountId,
                VisitDate = b.VisitDate,
                VisitTime = b.VisitTime
            }).Join(db.account, a => a.OrgId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                ManagerId = a.ManagerId,
                RepId = a.RepId,
                ManagerComment = a.ManagerComment,
                RepReply = a.RepReply,
                ManagerCommentDateTime = a.ManagerCommentDateTime,
                RepReplyDateTime = a.RepReplyDateTime,
                VisitId = a.VisitId,
                OrgId = a.OrgId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                OrgName = b.AccountName
            }).Join(db.Users, a => a.ManagerId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                ManagerId = a.ManagerId,
                RepId = a.RepId,
                ManagerComment = a.ManagerComment,
                RepReply = a.RepReply,
                ManagerCommentDateTime = a.ManagerCommentDateTime,
                RepReplyDateTime = a.RepReplyDateTime,
                VisitId = a.VisitId,
                OrgId = a.OrgId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                OrgName = a.OrgName,
                ManagerName = b.FullName
            }).Join(db.Users, a => a.RepId, b => b.Id, (a, b) => new CustomComments
            {
                Id = a.Id,
                ManagerId = a.ManagerId,
                RepId = a.RepId,
                ManagerComment = a.ManagerComment,
                RepReply = a.RepReply,
                ManagerCommentDateTime = a.ManagerCommentDateTime,
                RepReplyDateTime = a.RepReplyDateTime,
                VisitId = a.VisitId,
                OrgId = a.OrgId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                OrgName = a.OrgName,
                ManagerName = a.ManagerName,
                RepName = b.FullName,
                VisitType = 2
            }).Where(x => x.RepId == obj.UserId && x.ManagerCommentDateTime >= obj.Start && x.ManagerCommentDateTime <= obj.End);

            foreach (var item in AccountSales)
            {
                res.Add(item);
            }

            IEnumerable<CustomComments> ContactMedical = db.contactMedicalVisitChat.Join(db.contactMedicalVisit, a => a.ContactMedicalVisitId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                ManagerId = a.ManagerId,
                RepId = a.RepId,
                ManagerComment = a.ManagerComment,
                RepReply = a.RepReply,
                ManagerCommentDateTime = a.ManagerCommentDateTime,
                RepReplyDateTime = a.RepReplyDateTime,
                VisitId = b.Id,
                OrgId = b.ContactId,
                VisitDate = b.VisitDate,
                VisitTime = b.VisitTime
            }).Join(db.contact, a => a.OrgId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                ManagerId = a.ManagerId,
                RepId = a.RepId,
                ManagerComment = a.ManagerComment,
                RepReply = a.RepReply,
                ManagerCommentDateTime = a.ManagerCommentDateTime,
                RepReplyDateTime = a.RepReplyDateTime,
                VisitId = a.VisitId,
                OrgId = a.OrgId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                OrgName = b.ContactName
            }).Join(db.Users, a => a.ManagerId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                ManagerId = a.ManagerId,
                RepId = a.RepId,
                ManagerComment = a.ManagerComment,
                RepReply = a.RepReply,
                ManagerCommentDateTime = a.ManagerCommentDateTime,
                RepReplyDateTime = a.RepReplyDateTime,
                VisitId = a.VisitId,
                OrgId = a.OrgId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                OrgName = a.OrgName,
                ManagerName = b.FullName
            }).Join(db.Users, a => a.RepId, b => b.Id, (a, b) => new CustomComments
            {
                Id = a.Id,
                ManagerId = a.ManagerId,
                RepId = a.RepId,
                ManagerComment = a.ManagerComment,
                RepReply = a.RepReply,
                ManagerCommentDateTime = a.ManagerCommentDateTime,
                RepReplyDateTime = a.RepReplyDateTime,
                VisitId = a.VisitId,
                OrgId = a.OrgId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                OrgName = a.OrgName,
                ManagerName = a.ManagerName,
                RepName = b.FullName,
                VisitType = 3
            }).Where(x => x.RepId == obj.UserId && x.ManagerCommentDateTime >= obj.Start && x.ManagerCommentDateTime <= obj.End);

            foreach (var item in ContactMedical)
            {
                res.Add(item);
            }

            IEnumerable<CustomComments> AccountSupportive = db.AccountSupportiveVisitChat.Join(db.AccountSupportiveVisit, a => a.AccountSupportiveVisitId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                ManagerId = a.ManagerId,
                RepId = a.RepId,
                ManagerComment = a.ManagerComment,
                RepReply = a.RepReply,
                ManagerCommentDateTime = a.ManagerCommentDateTime,
                RepReplyDateTime = a.RepReplyDateTime,
                VisitId = b.Id,
                OrgId = b.AccountId,
                VisitDate = b.VisitDate,
                VisitTime = b.VisitTime
            }).Join(db.account, a => a.OrgId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                ManagerId = a.ManagerId,
                RepId = a.RepId,
                ManagerComment = a.ManagerComment,
                RepReply = a.RepReply,
                ManagerCommentDateTime = a.ManagerCommentDateTime,
                RepReplyDateTime = a.RepReplyDateTime,
                VisitId = a.VisitId,
                OrgId = a.OrgId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                OrgName = b.AccountName
            }).Join(db.Users, a => a.ManagerId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                ManagerId = a.ManagerId,
                RepId = a.RepId,
                ManagerComment = a.ManagerComment,
                RepReply = a.RepReply,
                ManagerCommentDateTime = a.ManagerCommentDateTime,
                RepReplyDateTime = a.RepReplyDateTime,
                VisitId = a.VisitId,
                OrgId = a.OrgId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                OrgName = a.OrgName,
                ManagerName = b.FullName
            }).Join(db.Users, a => a.RepId, b => b.Id, (a, b) => new CustomComments
            {
                Id = a.Id,
                ManagerId = a.ManagerId,
                RepId = a.RepId,
                ManagerComment = a.ManagerComment,
                RepReply = a.RepReply,
                ManagerCommentDateTime = a.ManagerCommentDateTime,
                RepReplyDateTime = a.RepReplyDateTime,
                VisitId = a.VisitId,
                OrgId = a.OrgId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                OrgName = a.OrgName,
                ManagerName = a.ManagerName,
                RepName = b.FullName,
                VisitType = 4
            }).Where(x => x.RepId == obj.UserId && x.ManagerCommentDateTime >= obj.Start && x.ManagerCommentDateTime <= obj.End);

            foreach (var item in AccountSupportive)
            {
                res.Add(item);
            }


            return res.DistinctBy(a => a.Id).OrderBy(x=>x.ManagerCommentDateTime);
        }

        public bool AccountSupportiveRepReply(RepReplyModel obj)
        {
            DateTime d = ti.GetCurrentTime();
            AccountSupportiveVisitChat ch = db.AccountSupportiveVisitChat.Find(obj.Id);
            ch.RepReply = obj.RepReply;
            ch.RepReplyDateTime = d;
            db.SaveChanges();
            return true;
        }
    }
}

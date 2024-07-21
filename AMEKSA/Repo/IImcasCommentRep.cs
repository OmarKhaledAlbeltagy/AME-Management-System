using AMEKSA.Entities;
using AMEKSA.Models;

namespace AMEKSA.Repo
{
    public interface IImcasCommentRep
    {
        bool AddComment(AddImcasComment obj);
    }
}

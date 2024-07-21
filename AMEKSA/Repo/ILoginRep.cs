using AMEKSA.Models;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
    public interface ILoginRep
    {
        Task<dynamic> Login(LoginModel obj);
    }
}

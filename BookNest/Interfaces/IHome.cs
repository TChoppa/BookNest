using BookNest.DTO;
using BookNest.Models;

namespace BookNest.Interfaces
{
    public interface IHome
    {
        public Task<bool> Register(RegisterDTO dto);
        public Task<bool> Login(LoginDTO dto);
        public Task<int> EditPasssword(ForgetPassword dto);

        //public Task<bool> Login();
        //public Task<bool> ForgetPassword();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SdWP.DTO.Requests;
using SdWP.DTO.Responses;
namespace SdWP.Service.IServices
{
    public interface IUserRegisterService
    {
        Task<UserRegisterResponseDTO> RegisterAsync(UserRegisterRequestDTO dto);
    }
}

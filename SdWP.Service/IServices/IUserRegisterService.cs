using SdWP.DTO.Requests;
using SdWP.DTO.Responses;
using SdWP.Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SdWP.Service.IServices
{
    public interface IUserRegisterService
    {
        Task<ResultService<UserRegisterResponseDTO>> RegisterAsync(UserRegisterRequestDTO dto);
    }
}

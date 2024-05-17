using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFAssignment1Group3.Models;
using WPFAssignment1Group3.Services;

namespace WPFAssignment1Group3.State
{
    public interface IAuthenticator { 
        Task<bool> Login(string username, string password);
        Task<RegistrationResult> Registration(string username, string password, string confirmPassword, int role);
    }
    public class Authenticator : IAuthenticator
    {
        private readonly IStaffServices _staffServices;

        public Authenticator(IStaffServices staffServices)
        {
            _staffServices = staffServices;
        }

        public async Task<bool> Login(string username, string password)
        {
            var result = await _staffServices.GetStaffByName(username);
            if (result != null)
            {
                if (result.Password.Equals(password))
                {
                    App.AccountStore = new AccountStore
                    {
                        Id = result.StaffId,
                        Username = result.Name,
                        role = result.Role,
                    };
                    return true;
                }
            }
            return false;
        }

        public async Task<RegistrationResult> Registration(string username, string password, string confirmPassword, int role)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(confirmPassword))
            {
                return RegistrationResult.NotNullable;
            }
            if (password != confirmPassword)
            {
                return RegistrationResult.PasswordDoNotMatch;
            }
            Staff st = new Staff()
            {
                Name = username,
                Password = confirmPassword,
                Role = role
            };
            var registerStatus = await _staffServices.AddOrEditStaff(st);
            if (!registerStatus)
            {
                return RegistrationResult.CanNotRegistration;
            }
            return RegistrationResult.Success;
        }
       
    }
    public enum RegistrationResult
    {
        Success,
        PasswordDoNotMatch,
        NotNullable,
        UsernameAlreadyExists,
        CanNotRegistration
    }
}

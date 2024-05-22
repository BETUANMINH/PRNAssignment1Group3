using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFAssignment1Group3.Models;

namespace WPFAssignment1Group3.Services
{
    public interface IStaffServices
    {
        Task<List<Staff>> GetAlls();
        Task<List<Staff>> GetStaffsByName(string name);
        Task<Staff> GetStaffById(int id);
        Task<Staff> GetStaffByName(string username);
        Task<bool> AddOrEditStaff(Staff staff);
        Task<bool> DeleteStaff(int id);
    }
    public class StaffServices : IStaffServices
    {
        private readonly MyStoreContext _context;

        public StaffServices(MyStoreContext context)
        {
            _context = context;
        }

        public async Task<bool> AddOrEditStaff(Staff staff)
        {
            var entry = await _context.Staffs.FirstOrDefaultAsync(e => e.StaffId == staff.StaffId);
            if (entry != null) 
            {
                entry.Name = staff.Name;
                entry.Password = staff.Password;
                entry.Role = staff.Role;
                _context.Update(entry);
            }else
            {
                _context.Add(staff);
            }
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<Staff> GetStaffByName(string username)
        {
            var result =  await _context.Staffs.FirstOrDefaultAsync(e => e.Name == username);
            return result;
        }

        public async Task<bool> DeleteStaff(int id)
        {
            var entry = await _context.Staffs.FirstOrDefaultAsync(e => e.StaffId == id);
            if(entry != null)
            {
                _context.Remove(entry);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<Staff>> GetAlls()
        {
            var result = await _context.Staffs.AsQueryable().ToListAsync();
            return result;
        }

        public async Task<Staff> GetStaffById(int id)
        {
            var result = await _context.Staffs.FirstOrDefaultAsync(e => e.StaffId == id);
            return result;
        }

        public async Task<List<Staff>> GetStaffsByName(string name)
        {
            var result = await _context.Staffs.Where(e => e.Name.Contains(name)).ToListAsync();
            return result;
        }
    }
}

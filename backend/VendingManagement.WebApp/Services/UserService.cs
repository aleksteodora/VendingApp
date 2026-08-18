using Microsoft.EntityFrameworkCore;
using VendingManagement.WebApp.Data;
using VendingManagement.WebApp.DTOs;
using VendingManagement.WebApp.Entities;

namespace VendingManagement.WebApp.Services
{
    public class UserService : IUserService
    {
        private readonly VendingDbContext _context;

        public UserService(VendingDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserDataOut>> GetAllAsync()
        {
            return await _context.Users
                .Include(u => u.Meter)
                .Select(u => MapToDataOut(u))
                .ToListAsync();
        }

        public async Task<UserDataOut> GetByIdAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.Meter)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            return MapToDataOut(user);
        }

        public async Task<UserDataOut> CreateAsync(UserDataIn dataIn)
        {
            var user = new User
            {
                FullName = dataIn.FullName,
                Address = dataIn.Address,
                PhoneNumber = dataIn.PhoneNumber,
                ApiKey = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var meter = new Meter
            {
                MeterSerialNumber = dataIn.MeterSerialNumber,
                UserId = user.Id
            };

            _context.Meters.Add(meter);
            await _context.SaveChangesAsync();

            user.Meter = meter;
            return MapToDataOut(user);
        }

        public async Task<UserDataOut> UpdateAsync(int id, UserDataIn dataIn)
        {
            var user = await _context.Users
                .Include(u => u.Meter)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            user.FullName = dataIn.FullName;
            user.Address = dataIn.Address;
            user.PhoneNumber = dataIn.PhoneNumber;

            if (user.Meter != null)
            {
                user.Meter.MeterSerialNumber = dataIn.MeterSerialNumber;
            }

            await _context.SaveChangesAsync();

            return MapToDataOut(user);
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.Meter)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        private static UserDataOut MapToDataOut(User user)
        {
            return new UserDataOut
            {
                Id = user.Id,
                FullName = user.FullName,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                MeterSerialNumber = user.Meter?.MeterSerialNumber
            };
        }
    }
}
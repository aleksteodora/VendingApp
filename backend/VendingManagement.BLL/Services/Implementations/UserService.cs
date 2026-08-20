using VendingManagement.DAL.Entities;
using VendingManagement.DAL.UOW.Interfaces;
using VendingManagement.Shared.DTOs;
using VendingManagement.BLL.Services.Interfaces;

namespace VendingManagement.BLL.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<UserDataOut>> GetAllAsync()
        {
            var users = await _unitOfWork.UserRepository.GetAllAsync();
            var meters = await _unitOfWork.MeterRepository.GetAllAsync();

            return users.Select(u => MapToDataOut(u, meters.FirstOrDefault(m => m.UserId == u.Id))).ToList();
        }

        public async Task<UserDataOut> GetByIdAsync(int id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            var meter = await _unitOfWork.MeterRepository.FirstOrDefaultAsync(m => m.UserId == id);

            return MapToDataOut(user, meter);
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

            var meter = new Meter
            {
                MeterSerialNumber = dataIn.MeterSerialNumber,
                User = user
            };

            await _unitOfWork.UserRepository.AddAsync(user);
            await _unitOfWork.MeterRepository.AddAsync(meter);

            await _unitOfWork.SaveChangesAsync();

            return MapToDataOut(user, meter);
        }

        public async Task<UserDataOut> UpdateAsync(int id, UserDataIn dataIn)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            user.FullName = dataIn.FullName;
            user.Address = dataIn.Address;
            user.PhoneNumber = dataIn.PhoneNumber;
            _unitOfWork.UserRepository.Update(user);

            var meter = await _unitOfWork.MeterRepository.FirstOrDefaultAsync(m => m.UserId == id);
            if (meter != null)
            {
                meter.MeterSerialNumber = dataIn.MeterSerialNumber;
                _unitOfWork.MeterRepository.Update(meter);
            }

            await _unitOfWork.SaveChangesAsync();

            return MapToDataOut(user, meter);
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            var meter = await _unitOfWork.MeterRepository.FirstOrDefaultAsync(m => m.UserId == id);
            if (meter != null)
            {
                _unitOfWork.MeterRepository.Remove(meter);
            }

            _unitOfWork.UserRepository.Remove(user);
            await _unitOfWork.SaveChangesAsync();
        }

        private static UserDataOut MapToDataOut(User user, Meter? meter)
        {
            return new UserDataOut
            {
                Id = user.Id,
                FullName = user.FullName,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                MeterSerialNumber = meter?.MeterSerialNumber
            };
        }
    }
}
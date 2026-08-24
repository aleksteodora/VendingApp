using VendingManagement.DAL.Entities;
using VendingManagement.DAL.UOW.Interfaces;
using VendingManagement.Shared.DTOs;
using VendingManagement.Shared.Common;
using VendingManagement.Shared.Constants;
using VendingManagement.BLL.Services.Interfaces;

namespace VendingManagement.BLL.Services.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponsePackage<PagedResult<UserDataOut>>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber < 1)
                pageNumber = 1;
            if (pageSize < 1 || pageSize > 100)
                pageSize = 20;

            var (users, totalCount) = await _unitOfWork.CustomerRepository.GetPagedWithMeterAsync(pageNumber, pageSize);

            var items = users
                .Select(u => MapToDataOut(u, u.Meter))
                .ToList();

            var pagedResult = new PagedResult<UserDataOut>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return new ResponsePackage<PagedResult<UserDataOut>>(
                pagedResult,
                ResponseStatus.OK,
                "Users retrieved successfully.");
        }

        public async Task<ResponsePackage<UserDataOut>> GetByIdAsync(int id)
        {
            var user = await _unitOfWork.CustomerRepository.GetByIdAsync(id);

            if (user == null || user.IsDeleted)
            {
                return new ResponsePackage<UserDataOut>(
                    ResponseStatus.NotFound,
                    "User not found.");
            }

            var meter = await _unitOfWork.MeterRepository.GetByUserIdAsync(id);

            return new ResponsePackage<UserDataOut>(
                MapToDataOut(user, meter),
                ResponseStatus.OK,
                "User retrieved successfully.");
        }

        public async Task<ResponsePackage<UserDataOut>> CreateAsync(UserDataIn dataIn)
        {
            var user = new Customer
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

            await _unitOfWork.CustomerRepository.AddAsync(user);
            await _unitOfWork.MeterRepository.AddAsync(meter);
            await _unitOfWork.SaveChangesAsync();

            return new ResponsePackage<UserDataOut>(
                MapToDataOut(user, meter),
                ResponseStatus.Created,
                "User created successfully.");
        }

        public async Task<ResponsePackage<UserDataOut>> UpdateAsync(int id, UserDataIn dataIn)
        {
            var user = await _unitOfWork.CustomerRepository.GetByIdAsync(id);

            if (user == null || user.IsDeleted)
            {
                return new ResponsePackage<UserDataOut>(
                    ResponseStatus.NotFound,
                    "User not found.");
            }

            user.FullName = dataIn.FullName;
            user.Address = dataIn.Address;
            user.PhoneNumber = dataIn.PhoneNumber;
            _unitOfWork.CustomerRepository.Update(user);

            var meter = await _unitOfWork.MeterRepository.GetByUserIdAsync(id);
            if (meter != null)
            {
                meter.MeterSerialNumber = dataIn.MeterSerialNumber;
                _unitOfWork.MeterRepository.Update(meter);
            }

            await _unitOfWork.SaveChangesAsync();

            return new ResponsePackage<UserDataOut>(
                MapToDataOut(user, meter),
                ResponseStatus.OK,
                "User updated successfully.");
        }

        public async Task<ResponsePackageNoData> DeleteAsync(int id)
        {
            var user = await _unitOfWork.CustomerRepository.GetByIdAsync(id);

            if (user == null || user.IsDeleted)
            {
                return new ResponsePackageNoData(
                    ResponseStatus.NotFound,
                    "User not found.");
            }

            var meter = await _unitOfWork.MeterRepository.GetByUserIdAsync(id);
            if (meter != null)
            {
                meter.IsDeleted = true;
                _unitOfWork.MeterRepository.Update(meter);
            }

            user.IsDeleted = true;
            _unitOfWork.CustomerRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return new ResponsePackageNoData(
                ResponseStatus.OK,
                "User deleted successfully.");
        }

        private static UserDataOut MapToDataOut(Customer user, Meter? meter)
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
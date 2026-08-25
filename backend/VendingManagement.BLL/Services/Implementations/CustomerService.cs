using VendingManagement.DAL.Entities;
using VendingManagement.DAL.UOW.Interfaces;
using VendingManagement.Shared.DTOs;
using VendingManagement.Shared.Common;
using VendingManagement.Shared.Constants;
using VendingManagement.BLL.Services.Interfaces;
using VendingManagement.DAL.Repositories;

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
                .Select(u => new UserDataOut
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Address = u.Address,
                    PhoneNumber = u.PhoneNumber,
                    MeterSerialNumber = u.MeterSerialNumber
                })
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
            var user = await _unitOfWork.CustomerRepository.GetByIdWithMeterAsync(id);

            if (user == null || user.IsDeleted)
            {
                return new ResponsePackage<UserDataOut>(
                    ResponseStatus.NotFound,
                    "User not found.");
            }

            return new ResponsePackage<UserDataOut>(
                MapToDataOut(user),
                ResponseStatus.OK,
                "User retrieved successfully.");
        }
        
        public async Task<ResponsePackage<UserDataOut>> CreateAsync(UserDataIn dataIn)
        {
            var existingMeter = await _unitOfWork.MeterRepository.GetBySerialNumberAsync(dataIn.MeterSerialNumber);
            if (existingMeter != null && !existingMeter.IsDeleted)
            {
                return new ResponsePackage<UserDataOut>(
                    ResponseStatus.BadRequest,
                    "A user with this meter serial number already exists.");
            }

            var user = new Customer
            {
                FullName = dataIn.FullName,
                Address = dataIn.Address,
                PhoneNumber = dataIn.PhoneNumber,
                ApiKey = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                Meter = new Meter
                {
                    MeterSerialNumber = dataIn.MeterSerialNumber,
                }
            };

            await _unitOfWork.CustomerRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return new ResponsePackage<UserDataOut>(
                MapToDataOut(user),
                ResponseStatus.Created,
                "User created successfully.");
        }

        public async Task<ResponsePackage<UserDataOut>> UpdateAsync(int id, UserDataIn dataIn)
        {
            var user = await _unitOfWork.CustomerRepository.GetByIdWithMeterAsync(id);

            if (user == null || user.IsDeleted)
            {
                return new ResponsePackage<UserDataOut>(
                    ResponseStatus.NotFound,
                    "User not found.");
            }

            var existingMeter = await _unitOfWork.MeterRepository.GetBySerialNumberAsync(dataIn.MeterSerialNumber);
            if (existingMeter != null && !existingMeter.IsDeleted && existingMeter.UserId != id)
            {
                return new ResponsePackage<UserDataOut>(
                    ResponseStatus.BadRequest,
                    "A user with this meter serial number already exists.");
            }

            user.FullName = dataIn.FullName;
            user.Address = dataIn.Address;
            user.PhoneNumber = dataIn.PhoneNumber;

            if (user.Meter != null)
            {
                user.Meter.MeterSerialNumber = dataIn.MeterSerialNumber;
            }

            await _unitOfWork.SaveChangesAsync();

            return new ResponsePackage<UserDataOut>(
                MapToDataOut(user),
                ResponseStatus.OK,
                "User updated successfully.");
        }

        public async Task<ResponsePackageNoData> DeleteAsync(int id)
        {
            var user = await _unitOfWork.CustomerRepository.GetByIdWithMeterAsync(id);

            if (user == null || user.IsDeleted)
            {
                return new ResponsePackageNoData(
                    ResponseStatus.NotFound,
                    "User not found.");
            }

            if (user.Meter != null)
            {
                user.Meter.IsDeleted = true;
            }

            user.IsDeleted = true;
            await _unitOfWork.SaveChangesAsync();

            return new ResponsePackageNoData(
                ResponseStatus.OK,
                "User deleted successfully.");
        }

        private static UserDataOut MapToDataOut(Customer user)
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
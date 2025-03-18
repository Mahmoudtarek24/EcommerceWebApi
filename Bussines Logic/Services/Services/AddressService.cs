
using Bussines_Logic.DTO.IDTO;
using Data_Access_Layer.Models;

namespace Bussines_Logic.Services.Services
{
	public class AddressService<D> : IGenericService<AddressResponseDTO, D> where D : class
	{
		public IUnitOfWork unitOfWork { get; }

		public AddressService(IUnitOfWork unitOfWork)
		{
			this.unitOfWork = unitOfWork;
		}

		public async Task<ApiResponse<AddressResponseDTO>> CreateAsync(D createDto)
		{
			if (createDto is AddressCreateDTO CreateDTO)
			{
				try
				{
					var customer = await unitOfWork.CustomerRepository.GetByIdAsync(CreateDTO.CustomerId);
					if (customer is null)
						return new ApiResponse<AddressResponseDTO>(404, "Customer not found");

					await unitOfWork.CreateTarsaction();

					var address = new Address()
					{
						AddressLine1 = CreateDTO.AddressLine1,
						AddressLine2 = CreateDTO.AddressLine2,
						State = CreateDTO.State,
						Area = CreateDTO.Area,
						Governorate = CreateDTO.Governorate,
						CustomerId = customer.Id,
						PostalCode = CreateDTO.PostalCode,
					};
					await unitOfWork.AddressRepository.Insert(address);
					await unitOfWork.Save();
					await unitOfWork.Commit();

					var CreateRespond = new AddressResponseDTO()
					{
						State = CreateDTO.State,
						AddressLine1 = CreateDTO.AddressLine1,
						AddressLine2 = CreateDTO.AddressLine2,
						Area = CreateDTO.Area,
						CustomerId = customer.Id,
						Governate = CreateDTO.Governorate,
						Id = address.AddressId,
						PostalCode = CreateDTO.PostalCode,
						Message = "Created Sussessfully"
					};
					return new ApiResponse<AddressResponseDTO>(200, CreateRespond);
				}
				catch (Exception ex)
				{
					await unitOfWork.RollBack();
					return new ApiResponse<AddressResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
				}
			}
			else
			{
				return new ApiResponse<AddressResponseDTO>(400, "Invalid DTO type for Address registration.");
			}

		}

		public async Task<ApiResponse<AddressResponseDTO>> GetByIdAsync(int id)
		{
			try
			{
				string[] Includes = { "Customer" };
				var address = await unitOfWork.AddressRepository.GetEntityAsync(e => e.AddressId == id, Includes);
				if (address is null)
					return new ApiResponse<AddressResponseDTO>(404, "Address not found.");

				var Respond = new AddressResponseDTO()
				{
					State = address.State,
					AddressLine1 = address.AddressLine1,
					AddressLine2 = address.AddressLine2,
					Area = address.Area,
					CustomerId = address.Customer.Id,
					Governate = address.Governorate,
					Id = address.AddressId,
					PostalCode = address.PostalCode,
					Message = "Address Data"
				};
				return new ApiResponse<AddressResponseDTO>(200, Respond);
			}
			catch (Exception ex)
			{
				// Log the exception
				return new ApiResponse<AddressResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
			}
		}
		public async Task<ApiResponse<AddressResponseDTO>> UpdateAsync(D upateDto)
		{
			if (upateDto is AddressUpdateDTO updateDTO)
			{
				try
				{
					string[] Includes = { "Customer" };
					var address = await unitOfWork.AddressRepository.GetEntityAsync(e => e.AddressId == updateDTO.AddressId && e.CustomerId == updateDTO.CustomerId, Includes);
					if (address is null)
						return new ApiResponse<AddressResponseDTO>(404, "Address not found.");

					await unitOfWork.CreateTarsaction();
					address.State = updateDTO.State;
					address.Area = updateDTO.Area;
					address.AddressLine1 = updateDTO.AddressLine1;
					address.AddressLine2 = updateDTO.AddressLine2;
					address.PostalCode = updateDTO.PostalCode;
					address.Governorate = updateDTO.Governorate;

					await unitOfWork.Save();
					await unitOfWork.Commit();

					var respond = new AddressResponseDTO
					{
						State = address.State,
						AddressLine1 = address.AddressLine1,
						AddressLine2 = address.AddressLine2,
						Area = address.Area,
						CustomerId = address.Customer.Id,
						Governate = address.Governorate,
						Id = address.AddressId,
						PostalCode = address.PostalCode,
						Message = "Updated Successfully"
					};
					return new ApiResponse<AddressResponseDTO>(200, respond);
				}
				catch (Exception ex)
				{
					unitOfWork.RollBack();
					return new ApiResponse<AddressResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
				}
			}
			else
			{
				return new ApiResponse<AddressResponseDTO>(400, "Invalid DTO type for Address registration.");
			}
		}

		public async Task<ApiResponse<AddressResponseDTO>> DeleteAsync(int id)
		{
			throw new NotImplementedException();
		}
		public async Task<ApiResponse<AddressResponseDTO>> DeleteAsync(AddressDeleteDTO dTO)
		{
			string[] Includes = { "Customer" };
			var address = await unitOfWork.AddressRepository.GetEntityAsync(e => e.AddressId == dTO.AddressId && e.CustomerId == dTO.CustomerId, Includes);
			if (address is null)
				return new ApiResponse<AddressResponseDTO>(404, "Address not found.");

			try
			{
				await unitOfWork.CreateTarsaction();
				await unitOfWork.AddressRepository.Delete(address);
				await unitOfWork.Save();
				await unitOfWork.Commit();

				var respond = new AddressResponseDTO()
				{
					Message = $"Customer with Id {dTO.CustomerId} and Address Id {dTO.AddressId}  Deleted successfully."
				};
				return new ApiResponse<AddressResponseDTO>(200, respond);
			}
			catch (Exception ex)
			{

				await unitOfWork.RollBack();
				return new ApiResponse<AddressResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
			}
		}

		public async Task<ApiResponse<List<AddressResponseDTO>>> GetAddressesByCustomerAsync(int customerId)
		{
			try
			{
				string[] Includes = { "Customer" };
				var address = await unitOfWork.AddressRepository.GetAddressesByCustomer(e => e.CustomerId == customerId, Includes);
				if (address.Count()==0)
					return new ApiResponse<List<AddressResponseDTO>>(404, "Customer not found.");

				List<AddressResponseDTO> addressResponseDTOs = new List<AddressResponseDTO>();
				foreach (var respond in address)
				{
					AddressResponseDTO responseDTO = new AddressResponseDTO()
					{
						AddressLine1 = respond.AddressLine1,
						State = respond.State,
						AddressLine2 = respond.AddressLine2,
						Area = respond.Area,
						Governate = respond.Governorate,
						PostalCode = respond.PostalCode,
						Id = respond.AddressId,
						CustomerId = respond.CustomerId,
					};
					addressResponseDTOs.Add(responseDTO);
				}
				return new ApiResponse<List<AddressResponseDTO>>(200, addressResponseDTOs);
			}
			catch (Exception ex)
			{
				return new ApiResponse<List<AddressResponseDTO>>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
			}
		}
	}
}
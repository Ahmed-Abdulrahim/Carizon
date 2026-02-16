namespace Carizon.Infrastructure.Services
{
    public class RoleService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ILogger<RoleService> logger) : IRoleService
    {
        //Assign Role To User
        public async Task<ResultResponse<AssignRoleResponse>> AssignRoleToUserAsync(string userId, string roleName)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return ResultResponse<AssignRoleResponse>.Failure("User not found.");
            }

            if (!await roleManager.RoleExistsAsync(roleName))
            {
                return ResultResponse<AssignRoleResponse>.Failure($"Role '{roleName}' does not exist.");
            }

            if (await userManager.IsInRoleAsync(user, roleName))
            {
                return ResultResponse<AssignRoleResponse>.Failure($"User is already in role '{roleName}'.");
            }

            var result = await userManager.AddToRoleAsync(user, roleName);
            if (!result.Succeeded)
            {
                return ResultResponse<AssignRoleResponse>.Failure(result.Errors.First().Description);
            }

            logger.LogInformation("Assigned role {RoleName} to user {UserId}", roleName, userId);
            var assignRole = new AssignRoleResponse
            {
                RoleName = roleName,
                UserId = userId,
            };
            return ResultResponse<AssignRoleResponse>.Success(assignRole, $"Role '{roleName}' assigned successfully.");
        }

        //Create Role
        public async Task<ResultResponse<RoleResponse>> CreateRoleAsync(RoleDto dto)
        {
            if (await roleManager.RoleExistsAsync(dto.Name))
            {
                return ResultResponse<RoleResponse>.Failure($"Role '{dto.Name}' already exists.");
            }
            var role = new ApplicationRole
            {
                Name = dto.Name,
                Description = dto.Description ?? string.Empty,
            };
            var result = await roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                return ResultResponse<RoleResponse>.Failure(result.Errors.Select(e => e.Description));
            }

            logger.LogInformation("Created role: {RoleName}", dto.Name);
            return ResultResponse<RoleResponse>.Success(MapToDto(role), $"Role '{dto.Name}' created successfully.");
        }

        //Delete Role
        public async Task<ResultResponse<RoleResponse>> DeleteRoleAsync(string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return ResultResponse<RoleResponse>.Failure("Role not found.");
            }

            // Check if any users are assigned to this role
            var usersInRole = await userManager.GetUsersInRoleAsync(role.Name!);
            if (usersInRole.Any())
            {
                return ResultResponse<RoleResponse>.Failure($"Cannot delete role '{role.Name}'. It is assigned to {usersInRole.Count} user(s).");
            }

            var result = await roleManager.DeleteAsync(role);
            if (!result.Succeeded)
            {
                return ResultResponse<RoleResponse>.Failure(result.Errors.Select(e => e.Description));
            }

            logger.LogInformation("Deleted role: {RoleName}", role.Name);
            return ResultResponse<RoleResponse>.Success(MapToDto(role), $"Role '{role.Name}' deleted successfully.");
        }

        //Get All Role
        public async Task<ResultResponse<RoleResponse>> GetAllRolesAsync()
        {
            var roles = await roleManager.Roles.OrderBy(r => r.Name).Select(r => new RoleResponse
            {
                Id = r.Id.ToString(),
                Name = r.Name!,
                NormalizedName = r.NormalizedName,
                Description = r.Description,
            }).ToListAsync();

            return ResultResponse<RoleResponse>.Success(roles);
        }

        //Get Role By Id
        public async Task<ResultResponse<RoleResponse>> GetRoleByIdAsync(string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return ResultResponse<RoleResponse>.Failure("Role not found.");
            }
            var roleDto = new RoleResponse
            {
                Id = role.Id.ToString(),
                Name = role.Name!,
                NormalizedName = role.NormalizedName,
                Description = role.Description,
            };
            return ResultResponse<RoleResponse>.Success(roleDto);
        }

        //Get Role By Nmae
        public async Task<ResultResponse<RoleResponse>> GetRoleByNameAsync(string roleName)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role is null) return ResultResponse<RoleResponse>.Failure("Role not found.");
            var roleDto = new RoleResponse
            {
                Id = role.Id.ToString(),
                Name = role.Name!,
                NormalizedName = role.NormalizedName,
                Description = role.Description,
            };
            return ResultResponse<RoleResponse>.Success(roleDto);
        }

        //GetRoleForSpecificUser
        public async Task<ResultResponse<UserRoleResponse>> GetUserRolesAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user is null) return ResultResponse<UserRoleResponse>.Failure("User not found.");
            var roles = await userManager.GetRolesAsync(user);
            var userRole = new UserRoleResponse
            {
                UserId = user.Id.ToString(),
                Email = user.Email!,
                FullName = user.FullName,
                Roles = roles.ToList()
            };
            return ResultResponse<UserRoleResponse>.Success(userRole);
        }

        //GetUsersInRole
        public async Task<ResultResponse<UserRoleResponse>> GetUsersInRoleAsync(string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                return ResultResponse<UserRoleResponse>.Failure($"Role '{roleName}' does not exist.");
            }

            var users = await userManager.GetUsersInRoleAsync(roleName);

            var userDtos = new List<UserRoleResponse>();
            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                userDtos.Add(new UserRoleResponse
                {
                    UserId = user.Id.ToString(),
                    Email = user.Email!,
                    FullName = user.FullName,
                    Roles = roles.ToList()
                });
            }

            return ResultResponse<UserRoleResponse>.Success(userDtos);
        }

        //Remove Role From User
        public async Task<ResultResponse<AssignRoleResponse>> RemoveRoleFromUserAsync(string userId, string roleName)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return ResultResponse<AssignRoleResponse>.Failure("User not found.");
            }

            if (!await userManager.IsInRoleAsync(user, roleName))
            {
                return ResultResponse<AssignRoleResponse>.Failure($"User is not in role '{roleName}'.");
            }

            var result = await userManager.RemoveFromRoleAsync(user, roleName);
            if (!result.Succeeded)
            {
                return ResultResponse<AssignRoleResponse>.Failure(result.Errors.First().Description);
            }

            logger.LogInformation("Removed role {RoleName} from user {UserId}", roleName, userId);
            var removeRole = new AssignRoleResponse
            {
                RoleName = roleName,
                UserId = userId,
            };
            return ResultResponse<AssignRoleResponse>.Success(removeRole);
        }

        // update Role
        public async Task<ResultResponse<RoleResponse>> UpdateRoleAsync(string roleId, RoleDto dto)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return ResultResponse<RoleResponse>.Failure("Role not found.");
            }

            // Check if new name conflicts with existing role
            if (!string.IsNullOrEmpty(dto.Name) && dto.Name != role.Name)
            {
                if (await roleManager.RoleExistsAsync(dto.Name))
                {
                    return ResultResponse<RoleResponse>.Failure($"Role '{dto.Name}' already exists.");
                }
                role.Name = dto.Name;
            }

            if (dto.Description != null)
            {
                role.Description = dto.Description;
            }


            var result = await roleManager.UpdateAsync(role);
            if (!result.Succeeded)
            {
                return ResultResponse<RoleResponse>.Failure(result.Errors.Select(e => e.Description));
            }

            logger.LogInformation("Updated role: {RoleId}", roleId);
            return ResultResponse<RoleResponse>.Success(MapToDto(role), "Role updated successfully.");
        }



        //Private
        private static RoleResponse MapToDto(ApplicationRole role)
        {
            return new RoleResponse
            {
                Id = role.Id.ToString(),
                Name = role.Name!,
                NormalizedName = role.NormalizedName,
                Description = role.Description,

            };
        }
    }
}

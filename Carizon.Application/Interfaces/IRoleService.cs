namespace Carizon.Application.Interfaces
{
    public interface IRoleService
    {
        Task<ResultResponse<RoleResponse>> GetAllRolesAsync();
        Task<ResultResponse<RoleResponse>> GetRoleByIdAsync(string roleId);
        Task<ResultResponse<RoleResponse>> GetRoleByNameAsync(string roleName);
        Task<ResultResponse<RoleResponse>> CreateRoleAsync(RoleDto dto);
        Task<ResultResponse<RoleResponse>> UpdateRoleAsync(string roleId, RoleDto dto);
        Task<ResultResponse<RoleResponse>> DeleteRoleAsync(string roleId);
        Task<ResultResponse<UserRoleResponse>> GetUserRolesAsync(string userId);
        Task<ResultResponse<AssignRoleResponse>> AssignRoleToUserAsync(string userId, string roleName);
        Task<ResultResponse<AssignRoleResponse>> RemoveRoleFromUserAsync(string userId, string roleName);
        Task<ResultResponse<UserRoleResponse>> GetUsersInRoleAsync(string roleName);
    }
}

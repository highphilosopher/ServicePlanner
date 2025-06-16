using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServicePlanner.Data;
using ServicePlanner.Models;
using BCrypt.Net;

namespace ServicePlanner.Services
{
    public class UserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ServicePlannerContext _context;

        public UserService(UserManager<User> userManager, SignInManager<User> signInManager, ServicePlannerContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<IdentityResult> CreateUserAsync(User user, string password)
        {
            user.UserName = user.Email;
            user.EmailConfirmed = true; // Auto-confirm emails for admin-created accounts
            user.IsFirstLogin = true;
            user.CreatedDate = DateTime.UtcNow;

            var result = await _userManager.CreateAsync(user, password);
            
            if (result.Succeeded)
            {
                // Add user to role
                await _userManager.AddToRoleAsync(user, user.Role.ToString());
            }

            return result;
        }

        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            var existingUser = await _userManager.FindByIdAsync(user.Id);
            if (existingUser == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            }

            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;
            existingUser.UserName = user.Email;
            existingUser.Role = user.Role;
            existingUser.IsActive = user.IsActive;

            var result = await _userManager.UpdateAsync(existingUser);
            
            if (result.Succeeded)
            {
                // Update user roles
                var currentRoles = await _userManager.GetRolesAsync(existingUser);
                await _userManager.RemoveFromRolesAsync(existingUser, currentRoles);
                await _userManager.AddToRoleAsync(existingUser, user.Role.ToString());
            }

            return result;
        }

        public async Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }

        public async Task<IdentityResult> ResetPasswordAsync(User user, string newPassword)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return await _userManager.ResetPasswordAsync(user, token, newPassword);
        }

        public async Task<SignInResult> SignInAsync(string email, string password, bool rememberMe = false)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !user.IsActive)
            {
                return SignInResult.Failed;
            }

            var result = await _signInManager.PasswordSignInAsync(user, password, rememberMe, lockoutOnFailure: true);
            
            if (result.Succeeded)
            {
                user.LastLoginDate = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
            }

            return result;
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> SetFirstLoginCompleteAsync(User user)
        {
            user.IsFirstLogin = false;
            return await _userManager.UpdateAsync(user);
        }

        public async Task<bool> IsInRoleAsync(User user, string role)
        {
            return await _userManager.IsInRoleAsync(user, role);
        }

        public async Task<IdentityResult> DeactivateUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            }

            user.IsActive = false;
            return await _userManager.UpdateAsync(user);
        }

        public async Task SeedDefaultAdminAsync()
        {
            const string adminEmail = "admin@serviceplanner.local";
            const string adminPassword = "ServicePlanner2025!";

            var adminUser = await _userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "System",
                    LastName = "Administrator",
                    Role = UserRole.Admin,
                    EmailConfirmed = true,
                    IsFirstLogin = true,
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                };

                var result = await _userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, UserRole.Admin.ToString());
                }
            }
        }
    }
}

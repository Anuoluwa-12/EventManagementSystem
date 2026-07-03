using EventManagement.API.Data;
using EventManagement.API.DTO;
using EventManagement.API.Entity;
using EventManagement.API.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _config;
    private readonly PasswordHasher<User> _passwordHasher;
    private readonly IWebHostEnvironment _environment;
    public UserService(
      ApplicationDbContext context,
      IConfiguration config,
      IWebHostEnvironment environment)
    {
        _context = context;
        _config = config;
        _environment = environment;
        _passwordHasher = new PasswordHasher<User>();
    }
    //REGISTER 
    public async Task<string> RegisterAsync(RegisterDto dto)
    {
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(x => x.Email == dto.Email);

        if (existingUser != null)
            return "User already exists";

        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Role = "Attendee"
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return "User registered successfully";
    }

    //  LOGIN 
    public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Email == dto.Email);

        if (user == null)
            return null;

        var result = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            dto.Password
        );

        if (result == PasswordVerificationResult.Failed)
            return null;

        var token = GenerateJwtToken(user);

        return new LoginResponseDto
        {
            Token = token,
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role
        };
    }

    //FORGOT PASSWORD
    public async Task<bool> ForgotPasswordAsync(ForgotPasswordDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Email == dto.Email);

        if (user == null)
            return false;

        user.ResetPasswordToken = Guid.NewGuid().ToString();
        user.ResetPasswordTokenExpiry = DateTime.UtcNow.AddHours(1);

        await _context.SaveChangesAsync();

        return true;
    }

    //RESET PASSWORD
    public async Task<bool> ResetPasswordAsync(ResetPasswordDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x =>
                x.ResetPasswordToken == dto.Token &&
                x.ResetPasswordTokenExpiry > DateTime.UtcNow);

        if (user == null)
            return false;

        var passwordHasher = new PasswordHasher<User>();

        user.PasswordHash =
            passwordHasher.HashPassword(user, dto.NewPassword);

        user.ResetPasswordToken = null;
        user.ResetPasswordTokenExpiry = null;

        await _context.SaveChangesAsync();

        return true;
    }

    //CORPORATE ONBOARDING
    public async Task<string> CorporateOnboardingAsync(
    CorporateOnboardingDto dto)
    {
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(x => x.Email == dto.Email);

        if (existingUser != null)
            return "User already exists";

        // Create company
        if (string.IsNullOrWhiteSpace(dto.CompanyName))
            throw new Exception("CompanyName is required");

        var company = new Company
        {
            CompanyName = dto.CompanyName,
            CompanyEmail = dto.CompanyEmail,
            PhoneNumber = dto.PhoneNumber,
            Address = dto.Address,
            Website = dto.Website
        };

        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        // Create company admin
        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Role = "CorporateAdmin",
            CompanyId = company.Id
        };

        user.PasswordHash =
            _passwordHasher.HashPassword(user, dto.Password);

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return "Corporate account created successfully";
    }

    //GET PROFILE
    public async Task<ProfileDto> GetProfileAsync(int userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            return null;

        return new ProfileDto
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            ProfilePictureUrl = user.ProfilePictureUrl
        };
    }

    //UPDATE PROFILE
    public async Task<bool> UpdateProfileAsync(
    int userId,
    UpdateProfileDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            return false;

        // Update profile information
        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.Email = dto.Email;

        // Check if a new profile picture was uploaded
        if (dto.ProfilePicture != null)
        {
            // Generate a unique filename
            var fileName = Guid.NewGuid().ToString() +
                           Path.GetExtension(dto.ProfilePicture.FileName);

            // Create the folder path
            var folderPath = Path.Combine(
                _environment.WebRootPath,
                "images",
                "profile");

            // Create the folder if it doesn't exist
            Directory.CreateDirectory(folderPath);

            // Create the full file path
            var filePath = Path.Combine(folderPath, fileName);

            // Save the uploaded image
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.ProfilePicture.CopyToAsync(stream);
            }

            // Save the image URL to the database
            user.ProfilePictureUrl = "/images/profile/" + fileName;
        }

        await _context.SaveChangesAsync();

        return true;
    }
    public async Task<bool> UpdateAccountTypeAsync(
    int userId,
    string accountType)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
            return false;

        user.AccountType = accountType;

        await _context.SaveChangesAsync();

        return true;
    }
    //  JWT 
    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"])
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(2),
            signingCredentials: creds
        );
        
  return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

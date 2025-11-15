using MongoDB.Driver;
using cowl.Models;
using BCrypt.Net;

namespace cowl.Services;

public class AuthService
{
    private readonly IMongoCollection<User> _usersCollection;
    private static AuthService? _instance;
    private static readonly object _lock = new object();
    
    public User? CurrentUser { get; private set; }

    private AuthService()
    {
        var connectionString = "mongodb+srv://nghia:Huong1505@cowl.fwmasrq.mongodb.net/?appName=cowl";
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase("cowl");
        _usersCollection = database.GetCollection<User>("users");

        // Tạo index cho username và email để đảm bảo không trùng lặp
        var indexKeysDefinition = Builders<User>.IndexKeys.Ascending(u => u.Username);
        var indexModel = new CreateIndexModel<User>(indexKeysDefinition, new CreateIndexOptions { Unique = true });
        _usersCollection.Indexes.CreateOne(indexModel);

        var emailIndexKeysDefinition = Builders<User>.IndexKeys.Ascending(u => u.Email);
        var emailIndexModel = new CreateIndexModel<User>(emailIndexKeysDefinition, new CreateIndexOptions { Unique = true });
        _usersCollection.Indexes.CreateOne(emailIndexModel);
    }

    public static AuthService Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new AuthService();
                    }
                }
            }
            return _instance;
        }
    }

    public async Task<(bool success, string message)> RegisterAsync(string username, string email, string password, string fullName)
    {
        try
        {
            // Kiểm tra username đã tồn tại chưa
            var existingUser = await _usersCollection.Find(u => u.Username == username).FirstOrDefaultAsync();
            if (existingUser != null)
            {
                return (false, "Tên đăng nhập đã tồn tại!");
            }

            // Kiểm tra email đã tồn tại chưa
            var existingEmail = await _usersCollection.Find(u => u.Email == email).FirstOrDefaultAsync();
            if (existingEmail != null)
            {
                return (false, "Email đã được sử dụng!");
            }

            // Kiểm tra độ mạnh của mật khẩu
            if (password.Length < 6)
            {
                return (false, "Mật khẩu phải có ít nhất 6 ký tự!");
            }

            // Hash mật khẩu
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            // Tạo user mới
            var newUser = new User
            {
                Username = username,
                Email = email,
                PasswordHash = passwordHash,
                FullName = fullName,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _usersCollection.InsertOneAsync(newUser);
            return (true, "Đăng ký thành công!");
        }
        catch (Exception ex)
        {
            return (false, $"Lỗi: {ex.Message}");
        }
    }

    public async Task<(bool success, string message, User? user)> LoginAsync(string username, string password)
    {
        try
        {
            // Tìm user theo username hoặc email
            var user = await _usersCollection.Find(u => u.Username == username || u.Email == username).FirstOrDefaultAsync();
            
            if (user == null)
            {
                return (false, "Tên đăng nhập hoặc mật khẩu không đúng!", null);
            }

            if (!user.IsActive)
            {
                return (false, "Tài khoản đã bị khóa!", null);
            }

            // Xác thực mật khẩu
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return (false, "Tên đăng nhập hoặc mật khẩu không đúng!", null);
            }

            // Cập nhật thời gian đăng nhập cuối
            var update = Builders<User>.Update.Set(u => u.LastLogin, DateTime.UtcNow);
            await _usersCollection.UpdateOneAsync(u => u.Id == user.Id, update);

            CurrentUser = user;
            return (true, "Đăng nhập thành công!", user);
        }
        catch (Exception ex)
        {
            return (false, $"Lỗi: {ex.Message}", null);
        }
    }

    public void Logout()
    {
        CurrentUser = null;
        // Xóa dữ liệu công ty khi logout
        CompanyDataService.Instance.ClearLocalData();
    }

    public async Task<bool> IsUsernameAvailableAsync(string username)
    {
        var user = await _usersCollection.Find(u => u.Username == username).FirstOrDefaultAsync();
        return user == null;
    }

    public async Task<bool> IsEmailAvailableAsync(string email)
    {
        var user = await _usersCollection.Find(u => u.Email == email).FirstOrDefaultAsync();
        return user == null;
    }

    public async Task<(bool success, string message)> ChangePasswordAsync(string userId, string oldPassword, string newPassword)
    {
        try
        {
            var user = await _usersCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null)
            {
                return (false, "Không tìm thấy người dùng!");
            }

            if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash))
            {
                return (false, "Mật khẩu cũ không đúng!");
            }

            if (newPassword.Length < 6)
            {
                return (false, "Mật khẩu mới phải có ít nhất 6 ký tự!");
            }

            var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            var update = Builders<User>.Update.Set(u => u.PasswordHash, newPasswordHash);
            await _usersCollection.UpdateOneAsync(u => u.Id == userId, update);

            return (true, "Đổi mật khẩu thành công!");
        }
        catch (Exception ex)
        {
            return (false, $"Lỗi: {ex.Message}");
        }
    }
}

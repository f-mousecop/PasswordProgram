using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PasswordProgram.data;
using PasswordProgram.PasswordPolicy;
using PasswordProgram.Services;

namespace PasswordProgram
{
    internal static class Program
    {
        const string ConnectionString = "Data Source=passwordpolicy.db";

        private static async Task Main(string[] args)
        {
            using IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.SetMinimumLevel(LogLevel.Warning);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseSqlite(ConnectionString));

                    services.AddSingleton(new PasswordPolicyOptions());
                    services.AddSingleton<IPasswordValidator, PasswordValidator>();
                    services.AddScoped<IPasswordService, PasswordService>();
                })
                .Build();

            // Ensure database exists and apply migration
            using (var scope = host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await dbContext.Database.MigrateAsync();
            }

            using (var scope = host.Services.CreateScope())
            {
                var passwordService = scope.ServiceProvider.GetRequiredService<IPasswordService>();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                await RunMainMenuAsync(passwordService, db);
            }
        }

        private static async Task RunMainMenuAsync(IPasswordService passwordService, AppDbContext db)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=========================================");
                Console.WriteLine("         Password Policy Console");
                Console.WriteLine("=========================================");
                Console.WriteLine();
                Console.WriteLine("                  Menu");
                Console.WriteLine("-----------------------------------------");
                Console.WriteLine("1. Register new user");
                Console.WriteLine("2. Change password");
                Console.WriteLine("3. List users");
                Console.WriteLine("4. Delete user");
                Console.WriteLine("0. Exit");
                Console.WriteLine("-----------------------------------------");
                Console.Write("Enter choice: ");

                var choice = Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1":
                        await RegisterUserAsync(passwordService);
                        break;

                    case "2":
                        await ChangePasswordAsync(passwordService, db);
                        break;

                    case "3":
                        await ListUsersAsync(db);
                        break;

                    case "4":
                        await DeleteUserAsync(db);
                        break;

                    case "0":
                        return;

                    default:
                        Console.Write("Invalid choice. Press any key to continue... ");
                        Console.ReadKey();
                        break;

                }
            }
        }


        /// <summary>
        /// Deletes a user from the database after prompting for confirmation and user selection.
        /// </summary>
        /// <remarks>This method interacts with the console to display all users, prompt for a user ID,
        /// and confirm deletion before removing the user. If no users exist or the operation is cancelled, no changes
        /// are made to the database. The method must be called from a context that supports console input and
        /// output.</remarks>
        /// <param name="db">The database context used to access and modify user records. Must not be null.</param>
        /// <returns>A task that represents the asynchronous delete operation.</returns>
        private static async Task DeleteUserAsync(AppDbContext db)
        {
            Console.Clear();
            Console.WriteLine("=== Delete User ===");

            Console.WriteLine("Press any key to list all users...");
            Console.ReadKey();

            var users = await db.Users.ToListAsync();

            if (users.Count == 0)
            {
                Console.WriteLine("No users found");
            }
            else
            {
                Console.WriteLine("--------------------------------------");
                foreach (var user in users)
                {
                    Console.WriteLine($"ID: {user.Id}\n" +
                        $"Username: {user.Username}\n" +
                        $"Account name: {user.AccountName}\n" +
                        $"Display name: {user.DisplayName}\n" +
                        $"First and last name: {user.FirstName} {user.LastName}");
                    Console.WriteLine("--------------------------------------");
                }
            }


            while (true)
            {
                Console.Write("Enter ther User ID to delete (or 'q' to cancel): ");
                var input = Console.ReadLine();

                if (string.Equals(input, "q", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Delete cancelled.");
                    Console.WriteLine("Press any key to return to menu...");
                    Console.ReadKey();
                    return;
                }

                if (!int.TryParse(input, out int userId))
                {
                    Console.WriteLine("Invalid ID. Please enter a numeric User ID.");
                    continue;
                }

                var userToDelete = await db.Users
                    .Include(u => u.PasswordHistories)
                    .SingleOrDefaultAsync(u => u.Id == userId);

                if (userToDelete == null)
                {
                    Console.WriteLine($"No user found with ID {userId}. Try again.");
                    continue;
                }

                Console.WriteLine();
                Console.WriteLine($"You are about to delete user:");
                Console.WriteLine($"  ID:       {userToDelete.Id}");
                Console.WriteLine($"  Username: {userToDelete.Username}");
                Console.Write("Are you sure? (y/n): ");
                var confirm = Console.ReadLine();

                if (!string.Equals(confirm, "y", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Delete cancelled.");
                    Console.WriteLine("Press any key to return to menu...");
                    Console.ReadKey();
                    return;
                }

                db.Users.Remove(userToDelete);
                await db.SaveChangesAsync();

                Console.WriteLine("User deleted successfully.");
                Console.WriteLine("Press any key to return to menu...");
                Console.ReadKey();
                return;
            }
        }


        /// <summary>
        /// Displays a list of all users in the database to the console asynchronously.
        /// </summary>
        /// <remarks>The method clears the console and outputs user details if any users are found;
        /// otherwise, it displays a message indicating that no users exist. The method pauses for user input before
        /// returning.</remarks>
        /// <param name="db">The database context used to retrieve user information. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private static async Task ListUsersAsync(AppDbContext db)
        {
            Console.Clear();
            Console.WriteLine("=== Users ===");

            var users = await db.Users.ToListAsync();

            if (users.Count == 0)
            {
                Console.WriteLine("No users found");
            }
            else
            {
                Console.WriteLine("--------------------------------------");
                foreach (var user in users)
                {
                    Console.WriteLine($"ID: {user.Id}\n" +
                        $"Username: {user.Username}\n" +
                        $"Account name: {user.AccountName}\n" +
                        $"Display name: {user.DisplayName}\n" +
                        $"First and last name: {user.FirstName} {user.LastName}");
                    Console.WriteLine("--------------------------------------");
                }
            }
            Console.WriteLine();
            Console.WriteLine("Press any key to return to menu...");
            Console.ReadKey();
        }


        /// <summary>
        /// Prompts the user to change their password by verifying credentials and updating the password using the
        /// specified password service.
        /// </summary>
        /// <remarks>This method interacts with the console to guide the user through the password change
        /// process. The operation can be cancelled by entering 'q' when prompted for the current password.</remarks>
        /// <param name="passwordService">The password service used to validate and update the user's password.</param>
        /// <param name="db">The database context used to retrieve user information.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private static async Task ChangePasswordAsync(IPasswordService passwordService, AppDbContext db)
        {
            Console.Clear();
            Console.WriteLine("=== Change Password ===");

            DisplayPolicy();

            string username;
            while (true)
            {
                Console.Write("Enter username: ");
                username = Console.ReadLine() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(username))
                {
                    Console.WriteLine("Username is required...");
                }
                else break;

            }

            var user = await db.Users.SingleOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                Console.WriteLine("User not found");
                Console.WriteLine("Press any key to return to menu...");
                Console.ReadKey();
                return;
            }

            while (true)
            {
                Console.Write("Current password (or 'q' to cancel): ");
                string? currentPassword = Console.ReadLine();

                if (string.Equals(currentPassword, "q", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                Console.WriteLine();

                Console.Write("New password: ");
                string? newPassword = Console.ReadLine();
                Console.WriteLine();

                var result = await passwordService.ChangePasswordAsync(
                    user.Id,
                    currentPassword ?? string.Empty,
                    newPassword ?? string.Empty);

                if (result.Success)
                {
                    Console.WriteLine("Password change successful");
                    break;
                }
                else
                {
                    Console.WriteLine("Password change failed:");
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine(" - " + error);
                    }
                }
                Console.WriteLine();
            }

            Console.WriteLine("Press any key to return to menu...");
            Console.ReadKey();
        }


        /// <summary>
        /// Guides the user through the process of registering a new account by prompting for required information and
        /// submitting it to the password service asynchronously.
        /// </summary>
        /// <remarks>This method interacts with the console to collect user input for account details and
        /// password confirmation. It displays the result of the registration process and any errors encountered. The
        /// method is intended for interactive console applications and blocks until the user presses a key to return to
        /// the menu.</remarks>
        /// <param name="passwordService">The password service used to register the new user account. Must not be null.</param>
        /// <returns>A task that represents the asynchronous registration operation.</returns>
        private static async Task RegisterUserAsync(IPasswordService passwordService)
        {
            Console.Clear();
            Console.WriteLine("=== Register New User ===");

            DisplayPolicy();

            Console.Write("Account name (e.g., John.Doe): ");
            string? accountName = Console.ReadLine();

            Console.Write("Username: ");
            string? username = Console.ReadLine();

            Console.Write("Display name: ");
            string? displayName = Console.ReadLine();

            Console.Write("First name: ");
            string? firstName = Console.ReadLine();

            Console.Write("Last name: ");
            string? lastName = Console.ReadLine();


            string password;
            while (true)
            {
                Console.Write("Password: ");
                string? first = Console.ReadLine();
                Console.WriteLine();

                Console.Write("Confirm password: ");
                string? second = Console.ReadLine();
                Console.WriteLine();

                if (first == second)
                {
                    password = first ?? string.Empty;
                    break;
                }

                Console.WriteLine("Passwords do not match");
            }

            Console.WriteLine();

            var result = await passwordService.RegisterUserAsync(
                accountName ?? string.Empty,
                username ?? string.Empty,
                displayName ?? string.Empty,
                firstName ?? string.Empty,
                lastName ?? string.Empty,
                password);

            if (result.Success)
            {
                Console.WriteLine("User registered successfully");
            }
            else
            {
                Console.WriteLine("Registration failed. Errors:");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine(" - " + error);
                }
            }

            Console.WriteLine("Press any key to return to menu...");
            Console.ReadKey();
        }



        private static void DisplayPolicy()
        {
            Console.WriteLine("=========================================================================");
            Console.WriteLine("    Password Policy");
            Console.WriteLine();
            Console.WriteLine("To ensure confidentiality and a strong defense, the password chosen by " +
                "you must meet the following requirements:\n" +
                "* Must be at least 14 characters in length.\n" +
                "* Must contain at least:\n  " +
                "\t* Two uppercase letters [A-Z]\n" +
                "\t* Two lowercase letters [a-z]\n" +
                "\t* Two digits [0-9]\n" +
                "\t* Two special characters [e.g.:!@#$%*]\n" +
                "\t* A new password should differ from the changed password by at least 4 characters.\n" +
                "\t* A password cannot contain your account name, username, display name, or any other information relating to personal identification.\n" +
                "\tExample: If account name = John.Doe " +
                "\t\tand username = jdoe\n" +
                "\t\tAnd display name = John Doe Systems Administrator NE Florida\n" +
                "\tThen regardless of case, the following cannot be any part of a password:\n" +
                "\tjohn, doe, jdoe, systems, administrator, ne, florida");
            Console.WriteLine("=========================================================================");
            Console.WriteLine();
        }
    }
}

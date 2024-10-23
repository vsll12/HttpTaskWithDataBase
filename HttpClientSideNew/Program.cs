using System.Net.Http.Json;
using System.Text.Json;
using HttpClientSideNew.DTOs;

internal class Program
{
    private static async Task Main(string[] args)
    {
        using var client = new HttpClient();

        while (true)
        {
            Console.WriteLine("1. All users");
            Console.WriteLine("2. Add user");
            Console.WriteLine("3. Delete user");
            Console.Write("Enter your choice: ");

            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                switch (choice)
                {
                    case 1:
                        try
                        {
                            var users = await client.GetFromJsonAsync<List<UserDTO>>("http://localhost:54751/users");

                            if (users != null && users.Any())
                            {
                                Console.WriteLine("Users: ");
                                foreach (var user in users)
                                {
                                    Console.WriteLine($"Username: {user.Name}, Surname: {user.Surname}");
                                }
                            }
                            else
                            {
                                Console.WriteLine("No users found.");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error : {ex.Message}");
                        }
                        break;

                    case 2:
                        
                        try
                        {
                            Console.Write("Enter the name: ");
                            string name = Console.ReadLine()!;
                            Console.Write("Enter the surname: ");
                            string surname = Console.ReadLine()!;

                            var newUser = new UserDTO { Name = name, Surname = surname };

                            var responseForPost = await client.PostAsJsonAsync("http://localhost:54751/Users", newUser);

                            if (responseForPost.IsSuccessStatusCode)
                            {
                                Console.WriteLine("User added successfully.");
                            }
                            else
                            {
                                Console.WriteLine($"Failed to add user. Status code: {responseForPost.StatusCode}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error adding user: " + ex.Message);
                        }
                        break;

                    case 3:

                        try
                        {
                            Console.Write("Enter the ID of the user to delete: ");
                            if (int.TryParse(Console.ReadLine(), out int id))
                            {
                                var responseForDelete = await client.DeleteAsync($"http://localhost:54751/User/{id}");

                                if (responseForDelete.IsSuccessStatusCode)
                                {
                                    Console.WriteLine("User deleted successfully");
                                }
                                else
                                {
                                    Console.WriteLine($"Failed to delete user Status code: {responseForDelete.StatusCode}");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Invalid ID format");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error deleting user: " + ex.Message);
                        }
                        break;

                    default:
                        Console.WriteLine("Invalid choice");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Please enter a valid choice");
            }
        }
    }
}

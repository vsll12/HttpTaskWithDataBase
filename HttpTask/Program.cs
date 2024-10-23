using HttpTask.Data;
using HttpTask.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text;
using System.Text.Json;

var dbContext = new AppDbContext();

var server = new HttpListener();
server.Prefixes.Add("http://localhost:54751/");
server.Start();

Console.WriteLine("Server started...");

while (true)
{
    try
    {

        var context = server.GetContext();
        var method = context.Request.HttpMethod.ToUpper();

        if (method == "GET")
        {

            var users = dbContext.Users.ToList();
            var json = JsonSerializer.Serialize(users);

            byte[] buffer = Encoding.UTF8.GetBytes(json);
            context.Response.ContentType = "application/json";
            context.Response.ContentLength64 = buffer.Length;

            await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.OutputStream.Close();
            Console.WriteLine("Processed GET request");
        }
        else if (method == "POST")
        {

            using var reader = new StreamReader(context.Request.InputStream, Encoding.UTF8);
            string body = await reader.ReadToEndAsync();

            Console.WriteLine(body);

            var newUser = JsonSerializer.Deserialize<User>(body);

            if (newUser != null)
            {

                dbContext.Users.Add(newUser);
                dbContext.SaveChanges();

                context.Response.StatusCode = (int)HttpStatusCode.Created; 
                var response = Encoding.UTF8.GetBytes("User created successfully");
                await context.Response.OutputStream.WriteAsync(response, 0, response.Length);
                context.Response.OutputStream.Close();
                Console.WriteLine("Processed POST request");
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest; 
            }
        }
        else if (method == "DELETE")
        {
     
            using var reader = new StreamReader(context.Request.InputStream, Encoding.UTF8);
            string body = await reader.ReadToEndAsync();


            if (int.TryParse(body, out int userId))
            {

                var user = dbContext.Users.FirstOrDefault(u => u.Id == userId);

                if (user != null)
                {
                    dbContext.Users.Remove(user);
                    dbContext.SaveChanges();

                    context.Response.StatusCode = (int)HttpStatusCode.NoContent;
                    Console.WriteLine($"Processed DELETE request for User ID {userId}");
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound; 
                }
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest; 
            }
        }
        else
        {
            context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed; // 405 Method Not Allowed
        }

        context.Response.OutputStream.Close();
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error: " + ex.Message);
    }
}

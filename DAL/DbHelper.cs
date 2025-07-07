using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;
using TaskManagementApp.Models;


namespace TaskManagementApp.DAL
{
    public class DbHelper
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();

        public bool RegisterUser(User user)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "INSERT INTO Users (Username, Password, Role) VALUES (@u, @p, @r)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@u", user.Username);
                cmd.Parameters.AddWithValue("@p", user.Password);
                cmd.Parameters.AddWithValue("@r", user.Role);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public User Login(string username, string password)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT * FROM Users WHERE Username=@u AND Password=@p";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@p", password);
                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    return new User
                    {
                        Id = Convert.ToInt32(dr["Id"]),
                        Username = dr["Username"].ToString(),
                        Role = dr["Role"].ToString()
                    };
                }
            }
            return null;
        }

        public bool AddTask(TaskModel task)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"INSERT INTO Tasks 
                        (Title, Description, Status, CreatedDate, UserId, Deadline, Priority)
                        VALUES
                        (@Title, @Description, @Status, @CreatedDate, @UserId, @Deadline, @Priority)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Title", task.Title);
                cmd.Parameters.AddWithValue("@Description", task.Description);
                cmd.Parameters.AddWithValue("@Status", task.Status ?? "Pending");
                cmd.Parameters.AddWithValue("@CreatedDate", task.CreatedDate);
                cmd.Parameters.AddWithValue("@UserId", task.UserId);

                cmd.Parameters.AddWithValue("@Deadline", (object)task.Deadline ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Priority", task.Priority ?? (object)DBNull.Value);

                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
        }


        public List<TaskModel> GetTasksByUserId(int userId)
        {
            List<TaskModel> list = new List<TaskModel>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT * FROM Tasks WHERE UserId = @uid";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@uid", userId);
                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new TaskModel
                    {
                        TaskId = Convert.ToInt32(dr["TaskId"]),
                        Title = dr["Title"].ToString(),
                        Description = dr["Description"].ToString(),
                        Status = dr["Status"].ToString(),
                        CreatedDate = Convert.ToDateTime(dr["CreatedDate"]),
                        UserId = Convert.ToInt32(dr["UserId"]),
                        Priority = dr["Priority"].ToString(),
                        Deadline = Convert.ToDateTime(dr["Deadline"])
                    });
                }
            }
            return list;
        }

        public List<TaskModel> GetTasksByUserIdWithSearch(int userId, string searchTerm)
        {
            List<TaskModel> list = new List<TaskModel>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"SELECT * FROM Tasks 
                         WHERE UserId = @uid 
                         AND (Title LIKE @search OR Description LIKE @search)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@search", "%" + searchTerm + "%");

                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new TaskModel
                    {
                        TaskId = Convert.ToInt32(dr["TaskId"]),
                        Title = dr["Title"].ToString(),
                        Description = dr["Description"].ToString(),
                        Status = dr["Status"].ToString(),
                        CreatedDate = Convert.ToDateTime(dr["CreatedDate"]),
                        UserId = Convert.ToInt32(dr["UserId"]),
                        Priority = dr["Priority"].ToString(),
                        Deadline = Convert.ToDateTime(dr["Deadline"])

                    });
                }
            }
            return list;
        }


        public TaskModel GetTaskById(int id)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT * FROM Tasks WHERE TaskId = @id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    return new TaskModel
                    {
                        TaskId = Convert.ToInt32(dr["TaskId"]),
                        Title = dr["Title"].ToString(),
                        Description = dr["Description"].ToString(),
                        Status = dr["Status"].ToString(),
                        CreatedDate = Convert.ToDateTime(dr["CreatedDate"]),
                        UserId = Convert.ToInt32(dr["UserId"]),
                        Priority = dr["Priority"]?.ToString(),
                        Deadline = Convert.ToDateTime(dr["Deadline"])
                    };
                }
            }
            return null;
        }

        public bool UpdateTask(TaskModel task)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"UPDATE Tasks 
                 SET Title = @Title,
                     Description = @Description,
                     Status = @Status,
                     Deadline = @Deadline,
                     Priority = @Priority
                 WHERE TaskId = @Id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Title", task.Title);
                cmd.Parameters.AddWithValue("@Description", task.Description);
                cmd.Parameters.AddWithValue("@Status", task.Status);
                cmd.Parameters.AddWithValue("@Deadline", (object)task.Deadline ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Priority", task.Priority ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Id", task.TaskId);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool DeleteTask(int id)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "DELETE FROM Tasks WHERE TaskId = @id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public List<TaskModel> GetAllTasks()
        {
            List<TaskModel> list = new List<TaskModel>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT * FROM Tasks";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new TaskModel
                    {
                        TaskId = Convert.ToInt32(dr["TaskId"]),
                        Title = dr["Title"].ToString(),
                        Description = dr["Description"].ToString(),
                        Status = dr["Status"].ToString(),
                        CreatedDate = Convert.ToDateTime(dr["CreatedDate"]),
                        UserId = Convert.ToInt32(dr["UserId"]),
                        Priority = dr["Priority"] != DBNull.Value ? dr["Priority"].ToString() : "—",
                        Deadline = dr["Deadline"] != DBNull.Value ? Convert.ToDateTime(dr["Deadline"]) : (DateTime?)null
                    });
                }

            }
            return list;
        }

        public bool IsUserNameExists(string userName)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT COUNT(*) FROM Users WHERE UserName = @userName";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@userName", userName);
                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }
    }
}
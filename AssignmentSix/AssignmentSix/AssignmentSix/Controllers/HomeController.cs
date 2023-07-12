using AssignmentSix.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace AssignmentSix.Controllers

{
    public class HomeController : Controller
    {
        public IConfiguration Configuration { get; }
        public HomeController(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IActionResult Index()
        {
            List<Inventory> inventoryList = new List<Inventory>();
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
  



            // must open the connection first


            // Obtain a data reader via ExecuteReader().
            using (SqlConnection connection = new SqlConnection(connectionString)) 
            {
                connection.Open();
                string sql = "SELECT * FROM Inventory";
                SqlCommand command = new SqlCommand(sql, connection);

                using (SqlDataReader dataReader = command.ExecuteReader())
                {


                    // **********************************************
                    // Hard coded. Don't do this!
                    // dataReader[""] gets the column name
                    //int inventoryId = Convert.ToInt32(dataReader["Id"]);
                    //string inventoryName = Convert.ToString(dataReader["Name"]);
                    //decimal inventoryPrice = Convert.ToDecimal(dataReader["Price"]);
                    //int inventoryQuantity = Convert.ToInt32(dataReader["Quantity"]);
                    //DateTime inventoryAddedOn = Convert.ToDateTime(dataReader["AddedOn"]);
                    // **********************************************



                    // the right way without inventory list yet
                    //while (dataReader.Read())
                    //{

                    //    for (int i = 0; i < dataReader.FieldCount; i++)
                    //    {
                    //        string currentColName = dataReader.GetName(i);
                    //        string currentColValue = Convert.ToString(dataReader.GetValue(i));

                    //    }

                    //}

                    while (dataReader.Read())
                    {
                        Inventory inventory = new Inventory();
                        inventory.Id = Convert.ToInt32(dataReader["Id"]);
                        inventory.Name = Convert.ToString(dataReader["Name"]);
                        inventory.Price = Convert.ToDecimal(dataReader["Price"]);
                        inventory.Quantity = Convert.ToInt32(dataReader["Quantity"]);
                        inventory.AddedOn = Convert.ToDateTime(dataReader["AddedOn"]);

                        inventoryList.Add(inventory);
                    }


                    // **********************************************
                    //  Returns multiple result sets from two tables
                    //string sql = "Select * From Inventory; Select * from Report";
                    //do
                    //{
                    //    while (dataReader.Read())
                    //    {
                    //        for (int i = 0; i < dataReader.FieldCount; i++)
                    //        {
                    //            string currentColName = dataReader.GetName(i);
                    //            string currentColValue = Convert.ToString(dataReader.GetValue(i));

                    //        }
                    //    }
                    //} while (dataReader.NextResult());

                    // **********************************************

                }
                connection.Close();
                


            }
            return View(inventoryList);




        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Inventory inventory)
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            using(SqlConnection connection = new SqlConnection(connectionString))
            {

                string sql = "INSERT INTO Inventory (Name, Price, Quantity) VALUES (@Name, @Price, @Quantity)";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    SqlParameter parameter = new SqlParameter
                    {
                        ParameterName = "@Name",
                        Value = inventory.Name,
                        SqlDbType = SqlDbType.VarChar,
                        Size = 50
                    };
                    // to add the parameters to our sql, use the SqlCommand 
                    command.Parameters.Add(parameter);

                    parameter = new SqlParameter
                    {
                        ParameterName = "@Price",
                        Value = inventory.Price,
                        SqlDbType = SqlDbType.Money

                    };
                    command.Parameters.Add(parameter);

                    parameter = new SqlParameter
                    {
                        ParameterName = "@Quantity",
                        Value = inventory.Quantity,
                        SqlDbType = SqlDbType.Int
                    };
                    command.Parameters.Add(parameter);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            ViewBag.Result = "Success";
            return View();
        }

        public IActionResult Update(int id)
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            Inventory inventory = new Inventory();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"Select * From Inventory Where Id='{id}'";
                SqlCommand command = new SqlCommand(sql, connection);
                connection.Open();
                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        inventory.Id = Convert.ToInt32(dataReader["Id"]);
                        inventory.Name = Convert.ToString(dataReader["Name"]);
                        inventory.Price = Convert.ToDecimal(dataReader["Price"]);
                        inventory.Quantity = Convert.ToInt32(dataReader["Quantity"]);
                        inventory.AddedOn = Convert.ToDateTime(dataReader["AddedOn"]);
                    }
                }

                connection.Close();
            }
            return View(inventory);
        }

        [HttpPost]
        public IActionResult Update(Inventory inventory, int id)
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"Update Inventory SET Name='{inventory.Name}', Price='{inventory.Price}', Quantity='{inventory.Quantity}' Where Id='{id}'";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"DELETE From Inventory Where Id='{id}'";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }

            return RedirectToAction("Index");
        }






    }
}



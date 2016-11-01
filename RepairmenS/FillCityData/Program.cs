using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FillCityData
{
    class Program
    {
        static void Main(string[] args)
        {
            FillCities();
           // FillCoordinates();
        }

        static void FillCities()
        {
            Console.WriteLine("Starting FillCityData...");
            StringBuilder builder = new StringBuilder();
            string query = "INSERT INTO Users (Id, RoleId, Email, LastName, FirstName, Username, Password) VALUES (@guid,@RoleId,@Email,@LastName,@FirstName,@UserName,@Password)";

            Dictionary<string, string> countries = new Dictionary<string, string>();
            
            Console.WriteLine("Countries picked up, total count of contries: {0}", countries.Count);
            Guid id3 = new Guid("56dc86b4-1229-e411-9417-a41f7255f9b5");

                        using (SqlConnection openCon = new SqlConnection("Data Source=DANIEL\\SQLEXPRESS;Initial Catalog=repairmenEntities;Persist Security Info=True;User ID=sa;Password=repairmen;"))
                        {
                            using (SqlCommand queryInsertCity = new SqlCommand(query))
                            {
                                queryInsertCity.Connection = openCon;
                                queryInsertCity.Parameters.Add("@guid", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
                                queryInsertCity.Parameters.Add("@RoleId", SqlDbType.UniqueIdentifier).Value = id3;
                                queryInsertCity.Parameters.Add("@Email", SqlDbType.NVarChar).Value = "dan@gmail.com";
                                queryInsertCity.Parameters.Add("@LastName", SqlDbType.NVarChar).Value = "Dan";
                                queryInsertCity.Parameters.Add("@FirstName", SqlDbType.NVarChar).Value = "Dan";
                                queryInsertCity.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = "Dan";
                                queryInsertCity.Parameters.Add("@Password", SqlDbType.VarChar).Value = "Dan";

                                openCon.Open();

                                queryInsertCity.ExecuteNonQuery();

                                openCon.Close();
                            }
                        }
                    
           
            
           
            Console.WriteLine("Ending FillCityData...");
        }

        static void FillCoordinates()
        {
            Console.WriteLine("Starting FillCoordinates...");
            StringBuilder builder = new StringBuilder();
            string query = "Update Cities set Longitude = @longitude, Latitude = @latitude WHERE CityName = @cityName";

            List<City> cities = new List<City>();
            using (StreamReader sr = new StreamReader("GeoLiteCity-Location.csv"))
            {
                string currentLine;
                while ((currentLine = sr.ReadLine()) != null)
                {
                    string[] parts = currentLine.Split(',');

                    City city = new City();
                    city.CityName = parts[1];
                    Console.WriteLine("City is  {0}", city.CityName);
                    Console.WriteLine("Latituede is {0}", parts[2]);
                    Console.WriteLine("Longiduted is {0}", parts[3]);

                    city.Latitude = float.Parse(parts[2]);
                    city.Longitude = float.Parse(parts[3]);
                    cities.Add(city);

                }
            }

            Console.WriteLine("Cities picked up, total count of cities: {0}", cities.Count);

            try
            {
                using (SqlConnection openCon = new SqlConnection("Data Source=DANIEL\\SQLEXPRESS;Initial Catalog=repairmenEntities;Persist Security Info=True;User ID=sa;Password=repairmen;"))
                {
                    openCon.Open();
                    foreach (City city in cities)
                    {
                        try
                        {
                            using (SqlCommand queryInsertCity = new SqlCommand(query))
                            {
                                queryInsertCity.Connection = openCon;
                                queryInsertCity.Parameters.Add("@longitude", SqlDbType.Float).Value = city.Longitude;
                                queryInsertCity.Parameters.Add("@latitude", SqlDbType.Float).Value = city.Latitude;
                                queryInsertCity.Parameters.Add("@cityName", SqlDbType.NVarChar).Value = city.CityName;
                                queryInsertCity.ExecuteNonQuery();
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Counldn't insert data for {0} error message: {1}", city.CityName, ex.Message);
                            builder.AppendLine(string.Format("Counldn't insert {0}, error message: {1}", city.CityName, ex.Message));
                        }
                    }
                    openCon.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Counldn't establish connection to DB");
                builder.AppendLine("Counldn't establish connection to DB");
            }

            File.WriteAllText("notInserted.txt", builder.ToString());
            Console.WriteLine("Ending FillCoordinates...");
            Console.ReadLine();
        }
    }

    class City
    {
        public string CityName { get; set; }
        public float Longitude { get; set; }
        public float Latitude { get; set; }
        public City() { }
    }
}

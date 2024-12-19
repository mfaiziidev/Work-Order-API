using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace WOAPI.Controllers
{
    public class MetalineController : ApiController
    {
        DBConnection db = new DBConnection();

        [ActionName("GetWorkOrderDateWise")]
        [Route("api/Metaline/GetWorkOrderDateWise/{pstgDate:datetime}")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetAllWorkOrder1(DateTime pstgDate)
        {
            try
            {
                // Fetch MO numbers from the database filtered by pstgDate
                var moListFromDB = GetAllWorkOrderFromDB1(pstgDate);

                // Fetch MO numbers from the external API
                var moListFromAPI = await FetchMOFromExternalAPI();

                // Extract MO numbers from the API response
                var moNumbersFromAPI = moListFromAPI.Select(item => item.mO_NUMBER).ToList();

                // Filter MO numbers from the database that are not present in the API response
                var filteredMOList = moListFromDB.Where(mo => !moNumbersFromAPI.Contains(mo.MO_Number)).ToList();

                return Request.CreateResponse(HttpStatusCode.OK, new { filteredMOList });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        public async Task<List<ReverseAPIResponse>> FetchMOFromExternalAPI()
        {
            using (var client = new HttpClient())
            {
                // Replace "api/endpoint" with the correct endpoint path
                string apiUrl = "http://110.39.11.6:8090/Piecerate/ProcessedOrders";

                client.BaseAddress = new Uri(apiUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<ReverseAPIResponse>>(responseData);
                }
                else
                {
                    throw new Exception($"Failed to fetch data from external API: {response.StatusCode}");
                }
            }
        }

        // Your existing code for GetAllWorkOrderFromDB1 method with date filter
        public List<clsMO> GetAllWorkOrderFromDB1(DateTime pstgDate)
        {
            try
            {
                List<clsMO> lst = new List<clsMO>();

                using (SqlConnection conn = db.GetDBConnection())
                {
                    using (SqlCommand cmd = new SqlCommand("SP_GetNewMODateWise", conn)) // Assuming you have a stored procedure for filtering by date
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@PSTGDATE", pstgDate);
                        conn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                clsMO MoInfo = new clsMO();
                                MoInfo.MO_Number = reader["MO_Number"].ToString();
                                MoInfo.MO_Status = Convert.ToInt32(reader["MO_Status"]);
                                MoInfo.Item_Num = reader["Item_Num"].ToString();
                                MoInfo.Item_Desc = reader["Item_Desc"].ToString();
                                MoInfo.Total_Quantity = Convert.ToDecimal(reader["Total_Quantity"]);
                                MoInfo.Start_Date = Convert.ToDateTime(reader["Start_Date"]);

                                lst.Add(MoInfo);
                            }
                        }
                    }
                }
                return lst;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting entries from the database: " + ex.Message);
                throw;
            }
        }



        //----------------------------------------------------------------------------------------------------------------------

        [ActionName("GetWorkOrderALL")]
        [Route("api/Metaline/GetWorkOrderALL")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetAllWorkOrder1()
        {
            try
            {
                // Fetch MO numbers from the database
                var moListFromDB = GetAllWorkOrderFromDB1();

                // Fetch MO numbers from the external API
                var moListFromAPI = await FetchMOFromExternalAPI1();

                // Extract MO numbers from the API response
                var moNumbersFromAPI = moListFromAPI.Select(item => item.mO_NUMBER).ToList();

                // Filter MO numbers from the database that are not present in the API response
                var filteredMOList = moListFromDB.Where(mo => !moNumbersFromAPI.Contains(mo.MO_Number)).ToList();

                return Request.CreateResponse(HttpStatusCode.OK, new { filteredMOList });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        // Method to fetch MO numbers from the external API
        public async Task<List<ReverseAPIResponse>> FetchMOFromExternalAPI1()
        {
            using (var client = new HttpClient())
            {
                // Replace "api/endpoint" with the correct endpoint path
                string apiUrl = "http://110.39.11.6:8090/Piecerate/ProcessedOrders";

                client.BaseAddress = new Uri(apiUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<ReverseAPIResponse>>(responseData);
                }
                else
                {
                    throw new Exception($"Failed to fetch data from external API: {response.StatusCode}");
                }
            }
        }

        // Your existing code for GetAllWorkOrderFromDB1 method
        public List<clsMO> GetAllWorkOrderFromDB1()
        {
            try
            {
                List<clsMO> lst = new List<clsMO>();

                using (SqlConnection conn = db.GetDBConnection())
                {
                    using (SqlCommand cmd = new SqlCommand("SP_GetNewMOALL", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        conn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                clsMO MoInfo = new clsMO();
                                MoInfo.MO_Number = reader["MO_Number"].ToString();
                                MoInfo.MO_Status = Convert.ToInt32(reader["MO_Status"]);
                                MoInfo.Item_Num = reader["Item_Num"].ToString();
                                MoInfo.Item_Desc = reader["Item_Desc"].ToString();
                                MoInfo.Total_Quantity = Convert.ToDecimal(reader["Total_Quantity"]);
                                MoInfo.Start_Date = Convert.ToDateTime(reader["Start_Date"]);

                                lst.Add(MoInfo);
                            }
                        }
                    }
                }
                return lst;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting entries from the database: " + ex.Message);
                throw;
            }
        }

        //----------------------------------------------------------------------------------------------------------------------




        [HttpPost]
        [ActionName("PostTransEntries")]
        [Route("api/Metaline/PostTransEntries")]
        public HttpResponseMessage PostEntries([FromBody] List<clsMIL> Metaline)
        {
            int st = 0;
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, new { });

            foreach (clsMIL info in Metaline)
            {
                using (SqlConnection conn = db.GetDBConnection())
                {
                    using (SqlCommand cmd = new SqlCommand("sp_APIGLEntries", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@TrasDate", info.TrasDate);
                        cmd.Parameters.AddWithValue("@RefNo", info.RefNo);
                        cmd.Parameters.AddWithValue("@Account", info.Account);
                        cmd.Parameters.AddWithValue("@Debit", info.Debit);
                        cmd.Parameters.AddWithValue("@Credit", info.Credit);

                        try
                        {
                            conn.Open();
                            st = cmd.ExecuteNonQuery();
                        }
                        catch (SqlException ex)
                        {
                            Console.WriteLine("Error executing stored procedure sp_APIGLEntries: " + ex.Message);
                            response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
                            return response;
                        }
                    }
                }
            }

            clsResponse resp = new clsResponse();
            resp.StatusCode = st;
            if (st == -1)
            {
                resp.Status = "Posted";
            }
            if (st == 0)
            {
                resp.Status = "Failed";
            }
            response = Request.CreateResponse(HttpStatusCode.OK, new { resp });
            return response;
        }

    }

    //----------------------------------------------------------------------------------------------------------------------

    //[ActionName("GetWorkOrderALL")]
    //[Route("api/Metaline/GetWorkOrderALL")]
    //[HttpGet]
    //public HttpResponseMessage GetAllWorkOrder1()
    //{
    //    try
    //    {
    //        var result = GetAllWorkOrderFromDB1();
    //        return Request.CreateResponse(HttpStatusCode.OK, new { result });
    //    }
    //    catch (Exception ex)
    //    {
    //        return Request.CreateResponse(HttpStatusCode.InternalServerError, $"An error occurred: {ex.Message}");
    //    }
    //}


    //public List<clsMO> GetAllWorkOrderFromDB1()
    //{
    //    try
    //    {
    //        List<clsMO> lst = new List<clsMO>();

    //        using (SqlConnection conn = db.GetDBConnection())
    //        {
    //            using (SqlCommand cmd = new SqlCommand("SP_GetNewMOALL", conn))
    //            {
    //                cmd.CommandType = CommandType.StoredProcedure;
    //                conn.Open();

    //                using (SqlDataReader reader = cmd.ExecuteReader())
    //                {
    //                    while (reader.Read())
    //                    {
    //                        clsMO MoInfo = new clsMO();
    //                        MoInfo.MO_Number = reader["MO_Number"].ToString();
    //                        MoInfo.MO_Status = Convert.ToInt32(reader["MO_Status"]);
    //                        MoInfo.Item_Num = reader["Item_Num"].ToString();
    //                        MoInfo.Item_Desc = reader["Item_Desc"].ToString();
    //                        MoInfo.Total_Quantity = Convert.ToDecimal(reader["Total_Quantity"]);
    //                        MoInfo.Start_Date = Convert.ToDateTime(reader["Start_Date"]);

    //                        lst.Add(MoInfo);
    //                    }
    //                }
    //            }
    //        }
    //        return lst;
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine("Error getting entries from the database: " + ex.Message);
    //        throw;
    //    }
    //}

    //----------------------------------------------------------------------------------------------------------------------
}

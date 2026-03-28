using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Round.Models;

namespace Round.Helpers
{
    public static class DbHelper
    {
        private static string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["ItemHierarchyDB"].ConnectionString;
            }
        }

        public static User ValidateUser(string email, string password)
        {
            User user = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_ValidateUser", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Password", password);

                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                user = new User
                                {
                                    UserId = Convert.ToInt32(reader["UserId"]),
                                    Email = reader["Email"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Database error while validating user: " + ex.Message);
            }
            return user;
        }

        public static List<Item> GetAllItems()
        {
            List<Item> items = new List<Item>();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_GetAllItems", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                items.Add(MapReaderToItem(reader));
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Database error while fetching items: " + ex.Message);
            }
            return items;
        }

        public static Item GetItemById(int itemId)
        {
            Item item = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_GetItemById", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ItemId", itemId);

                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                item = MapReaderToItem(reader);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Database error while fetching item: " + ex.Message);
            }
            return item;
        }

        public static int AddItem(string itemName, decimal weight, int? parentItemId = null)
        {
            int newItemId = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_AddItem", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ItemName", itemName);
                        cmd.Parameters.AddWithValue("@Weight", weight);
                        cmd.Parameters.AddWithValue("@ParentItemId",
                            parentItemId.HasValue ? (object)parentItemId.Value : DBNull.Value);

                        conn.Open();
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            newItemId = Convert.ToInt32(result);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Database error while adding item: " + ex.Message);
            }
            return newItemId;
        }

        public static void UpdateItem(int itemId, string itemName, decimal weight)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_UpdateItem", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ItemId", itemId);
                        cmd.Parameters.AddWithValue("@ItemName", itemName);
                        cmd.Parameters.AddWithValue("@Weight", weight);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Database error while updating item: " + ex.Message);
            }
        }

        public static void DeleteItem(int itemId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_DeleteItem", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ItemId", itemId);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Database error while deleting item: " + ex.Message);
            }
        }

        public static List<Item> SearchItems(string searchTerm)
        {
            List<Item> items = new List<Item>();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_SearchItems", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SearchTerm", searchTerm ?? "");

                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                items.Add(MapReaderToItem(reader));
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Database error while searching items: " + ex.Message);
            }
            return items;
        }

        public static List<Item> GetChildItems(int parentItemId)
        {
            List<Item> items = new List<Item>();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_GetChildItems", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ParentItemId", parentItemId);

                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                items.Add(MapReaderToItem(reader));
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Database error while fetching child items: " + ex.Message);
            }
            return items;
        }

        public static decimal GetChildWeightSum(int parentItemId)
        {
            decimal totalWeight = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_GetChildWeightSum", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ParentItemId", parentItemId);

                        conn.Open();
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            totalWeight = Convert.ToDecimal(result);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Database error while getting child weight sum: " + ex.Message);
            }
            return totalWeight;
        }

        public static void MarkAsProcessed(int itemId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_MarkAsProcessed", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ItemId", itemId);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Database error while marking item as processed: " + ex.Message);
            }
        }

        public static List<Item> GetProcessedItems()
        {
            List<Item> items = new List<Item>();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_GetProcessedItems", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                items.Add(MapReaderToItem(reader));
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Database error while fetching processed items: " + ex.Message);
            }
            return items;
        }

        public static List<Item> GetItemTree()
        {
            List<Item> allItems = new List<Item>();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_GetAllItemsForTree", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                allItems.Add(MapReaderToItem(reader));
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Database error while building item tree: " + ex.Message);
            }

            return BuildTree(allItems, null);
        }

        private static List<Item> BuildTree(List<Item> allItems, int? parentId)
        {
            List<Item> tree = new List<Item>();

            foreach (var item in allItems)
            {
                if (item.ParentItemId == parentId)
                {
                    item.Children = BuildTree(allItems, item.ItemId);
                    tree.Add(item);
                }
            }

            return tree;
        }

        private static Item MapReaderToItem(SqlDataReader reader)
        {
            return new Item
            {
                ItemId = Convert.ToInt32(reader["ItemId"]),
                ItemName = reader["ItemName"].ToString(),
                Weight = Convert.ToDecimal(reader["Weight"]),
                ParentItemId = reader["ParentItemId"] != DBNull.Value
                    ? (int?)Convert.ToInt32(reader["ParentItemId"])
                    : null,
                IsProcessed = Convert.ToBoolean(reader["IsProcessed"]),
                CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
            };
        }
    }
}

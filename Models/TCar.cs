using System;
using System.Data;
using System.Collections.Generic;

namespace CarApi
{
    public interface IDbItem 
    {
        public ItemStatus Status { get; set; }

        public IDbItem Get(IDbManager db);
        public void Update(IDbManager db);
        public void Add(IDbManager db);
        public void Delete(IDbManager db);
    }

    public class TCar : IDbItem
    {
        public ItemStatus Status { get; set; }

        public int Id { get; set; }
        public string Brand { get; set; }
        public string Type { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
        public string ImageUrl { get; set; }
        public double Price { get; set; }
        public int Yom { get; set; }

        // Default Constructor
        public TCar() {}

        // Constructor: Initialize empty TCar object with the given id and status
        public TCar(int id, ItemStatus status)
        {
            this.Id = id;
            this.Status = status;
        }

        // Constructor: Initialize this TCar object from DB on the given id
        public TCar(IDbManager db, int id)
        {
            IDbCommand cmd = db.GetDbCommand($"dbo.GetCar @id={id}");
            cmd.CommandType = CommandType.Text;
            using(IDataReader reader = cmd.ExecuteReader()) {

                if(reader.Read())
                {
                    this.readData(reader);
                }
                reader.Close();
            }
        }

        // Constructor: Create object instance form the given reader
        public TCar(IDataReader rd)
        {
            this.readData(rd);
        }

        // Static Factory: Returns new object instance by reading from the DB.
        public static TCar GetCar(IDbManager db, int id)
        {
            return new TCar(db, id);
        }

        // Re-reads this object from the DB.
        public IDbItem Get(IDbManager db)
        {
            TCar car = TCar.GetCar(db, this.Id);

            this.ImageUrl = car.ImageUrl;
            this.Model = car.Model;
            this.Price = car.Price;
            this.Status = ItemStatus.Valid;
            this.Type = car.Type;
            this.Yom = car.Yom;
            
            return this;
        }

        public void Update(IDbManager db)
        {
            this.Status = ItemStatus.Updated;
            this.dbSet(db);
        }

        // Inserts new item into DB. The ID contains newly created value.
        public void Add(IDbManager db)
        {
            this.Status = ItemStatus.New;
            this.dbSet(db);
        }

        // Deletes this item from DB
        public void Delete(IDbManager db)
        {
            this.Status = ItemStatus.Deleted;
            this.dbSet(db);
        }

        // Insert or update the TCar object in the DB based on the object Status
        private void dbSet(IDbManager db)
        {
            using(IDbConnection conn = db.GetDbConnection()) 
            using(IDbCommand cmd = conn.CreateCommand()) {

                cmd.CommandText = 
                    $"dbo.SetCar "+
                    $"@status={(int)Status}, @id={Id}, @brand='{Brand}', @type='{Type}'," +
                    $"@model='{Model}', @color='{Color}', @imageUrl='{ImageUrl}', @yom={Yom}, @price={Price}";

                cmd.CommandType = CommandType.Text;

                object result = cmd.ExecuteScalar();
                this.Id = Convert.IsDBNull(result) || (result == null) ? -1 : (int)result;
            }
        }

        private void readData(IDataReader reader)
        {
            Id = (int)reader.GetInt32(reader.GetOrdinal("id"));
            Brand = (string)reader.GetString(reader.GetOrdinal("brand"));
            Type = (string)reader.GetString(reader.GetOrdinal("type"));
            Model = (string)reader.GetString(reader.GetOrdinal("model"));
            Color = (string)reader.GetString(reader.GetOrdinal("color"));
            ImageUrl = (string)reader.GetString(reader.GetOrdinal("imageUrl"));
            Yom = (int)reader.GetInt32(reader.GetOrdinal("yom"));
            Price = (double)reader.GetDouble(reader.GetOrdinal("price"));
            Status = ItemStatus.Valid;
        }

        public static List<TCar> GetCarList(IDbManager db)
        {
            List<TCar> cars = new List<TCar>();

            IDbCommand cmd = db.GetDbCommand("getCars");
            cmd.CommandType = CommandType.StoredProcedure;
            
            using(IDataReader reader = cmd.ExecuteReader()) {

                while(reader.Read())
                {
                    cars.Add(new TCar(reader));
                }
                reader.Close();
            }
            return cars;
        }
    }
}

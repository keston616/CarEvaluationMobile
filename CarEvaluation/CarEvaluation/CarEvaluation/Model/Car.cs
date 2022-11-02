using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace CarEvaluation
{
    public class Car : ICloneable
    {
        public int idCar { get; set; }
        public int ModelId { get; set; }
        public Nullable<int> YearOfRealease { get; set; }
        public Nullable<int> EquipmentId { get; set; }
        public int StatusId { get; set; }
        public string VIN { get; set; }
        public string GosNumber { get; set; }
        public int BodyId { get; set; }
        public int Mileage { get; set; }
        public Nullable<decimal> EngineCapacity { get; set; }
        public Nullable<int> DriverId { get; set; }
        public int ColorId { get; set; }
        public int CountOwner { get; set; }
        public int TypeTransmissionId { get; set; }
        public Nullable<decimal> BuyPrice { get; set; }
        public Nullable<decimal> SellPrice { get; set; }
        public int GenerationId { get; set; }
        public int TypeEngineId { get; set; }
        public int FuelTypeId { get; set; }
        public bool Juridical { get; set; }
        public Nullable<int> PowerCapacity { get; set; }
        public bool IsOriginalPTC { get; set; }
        public Nullable<System.DateTime> DateBuy { get; set; }
        public Nullable<System.DateTime> DateSell { get; set; }
        public Nullable<int> InspectorId { get; set; }
        public Nullable<int> AniliticId { get; set; }
        public string Description { get; set; }
        public string CompressionMotor { get; set; }

        public virtual Body Body { get; set; }
        public virtual ColorCar Color { get; set; }
        public virtual Drive Drive { get; set; }
        public virtual Equipment Equipment { get; set; }
        public virtual FuelType FuelType { get; set; }
        public virtual Generation Generation { get; set; }
        public virtual Model Model { get; set; }
        public virtual Status Status { get; set; }
        public virtual TypeEngine TypeEngine { get; set; }
        public virtual TypeTransmission TypeTransmission { get; set; }
        public virtual User user { get; set; }
        public virtual User user1 { get; set; }

        public List<Booking> Booking { get; set; }

        public List<PhotoCar> PhotoCar { get; set; } = new List<PhotoCar>();
        public List<RepairedParts> Repairedparts { get; set; } = new List<RepairedParts>();

        [JsonIgnore]
        public object IsBuyPrice
        {
            get 
            {
                if (BuyPrice == null)
                {
                    return "Не установлена";
                }
                else return BuyPrice + " руб.";
            } 
        }

        [JsonIgnore]
        public object IsSellPrice
        {
            get
            {
                if (BuyPrice == null)
                {
                    return "Не установлена";
                }
                else return SellPrice + " руб.";
            }
        }
        [JsonIgnore]
        public string FirstImage
        {
            get
            {
                if (PhotoCar.Count != 0)
                {
                    return PhotoCar.First().Picture ;
                }
                else return "car.png";
            }
        }

        [JsonIgnore]
        public ObservableCollection<PhotoCar> PhotoObserCollection
        {
            get
            {
                return new ObservableCollection<PhotoCar>(PhotoCar);
            }
        }

        [JsonIgnore]
        public Nullable<DateTime> BookingDate
        {
            get
            {
                if (Booking != null &&  Booking.LastOrDefault() != null)
                {
                    DateTime dt = (DateTime)Booking.LastOrDefault().Date;
                    var dateValue = dt.AddDays(1);
                    DateTime yesterday = new DateTime(dateValue.Year, dateValue.Month, dateValue.Day, dt.Hour, dt.Minute, 0);
                    return yesterday;
                }
                else
                {
                    return new DateTime(); 
                }
            }
        }

        [JsonIgnore]
        public Booking BookingLast
        {
            get
            {
                    Booking bl = Booking.LastOrDefault();
                if (bl != null)
                {
                    bl.User = MainInfoClass.userList.Where(x => x.idUser == bl.UserId).FirstOrDefault();
                }
                    return bl;

            }
        }

        [JsonIgnore]
        public string DescriptionIsDefault
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Description))
                {
                    return "Без нареканий";
                }
                else
                {
                    return Description;
                }
           
            }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}

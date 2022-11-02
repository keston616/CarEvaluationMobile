using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace CarEvaluation
{
    public class User : INotifyPropertyChanged, ICloneable
    {
        public int idUser { get; set; }
        private string phone;
        public string Phone
        {
            get
            {
                return phone;
            }
            set
            {
                phone = value;
                NotifyPropertyChanged();
            }
        }
        public string Password { get; set; }
        private string surname;
        private string name;
        private string middleName;


        public int RoleId { get; set; }
        public Role Role { get; set; }
        private string photo;
        public string Photo
        {
            get
            {
                return photo;
            }
            set
            {
                photo = value;
                NotifyPropertyChanged();
            }
        }

       
        public string Surname
        {
            get
            {
                return surname;
            }
            set
            {
                surname = value;
                NotifyPropertyChanged();
            }
        }
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                NotifyPropertyChanged();
            }
        }
        public string MiddleName
        {
            get
            {
                return middleName;
            }
            set
            {
                middleName = value;
                NotifyPropertyChanged();
            }
        }

        public List<Booking> Booking { get; set; }
        public List<Car> car { get; set; } = new List<Car>();
        public List<Car> car1 { get; set; } = new List<Car>();


        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace CarEvaluation
{
    public  class Booking
    {
        public int IdBooking { get; set; }
        public Nullable<int> CarId { get; set; }
        public Nullable<int> UserId { get; set; }
        public string SurnameClient { get; set; }
        public string NameClient { get; set; }
        public string Phone { get; set; }
        public Nullable<System.DateTime> Date { get; set; }

        public virtual Car Car { get; set; }
        public virtual User User { get; set; }
    }
}

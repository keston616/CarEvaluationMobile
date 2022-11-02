using Android.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace CarEvaluation
{
    public class PhotoCar
    {
        public int idPhotoCar { get; set; }
        public int CarId { get; set; }
       // public Car Car { get; set; }
        public string Picture { get; set; }
        //public byte[] PicturByte
        //{
        //    get
        //    {
        //        Image imageIn = new Image { Source = Picture };
        //        MemoryStream ms = new MemoryStream();
        //        imageIn.Save(ms, ImageFormat);
        //        return ms.ToArray();
        //    }
        //}

    }
}

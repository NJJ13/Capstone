using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreshAir
{
    public static class MeterConverter
    {
        public static double ConvertMetersToMiles(int meters)
        {
            double metersInMile = 1609.344;
            double miles = Convert.ToDouble(meters) / metersInMile;
            return miles;
        }
    }
}

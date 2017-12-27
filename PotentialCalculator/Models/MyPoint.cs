using System;
using System.Linq;

namespace PotentialCalculator.Models {
    public struct MyPoint {
        public MyPoint(double x, double y) {
            X = x;
            Y = y;
        }
        public double X { get; set; }
        public double Y { get; set; }

    }
}

using System;

namespace NN{
    public static class Utils{

        public static Random random = new Random();
        public static double RandomRange(double min, double max){
            return random.NextDouble() * (max - min) + min;
        }
    }
}
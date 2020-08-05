using System;
using System.Linq;

namespace Chess.Model {
    public static class Constant {
        public const int SIZE = 8;
        private static Random random = new Random();
        public static string RngName(int length) {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
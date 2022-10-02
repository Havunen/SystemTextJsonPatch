using System;

namespace SystemTextJsonPatch.Tests.TestObjectModels
{
    public class MyCustomDateOnly
    {
        private int year;
        private int month;
        private int day;

        public MyCustomDateOnly(int year, int month, int day)
        {
            this.year = year;
            this.month = month;
            this.day = day;
        }

        internal static MyCustomDateOnly Parse(string v)
        {
            var dt = DateTime.Parse(v);

            return new MyCustomDateOnly(dt.Year, dt.Month, dt.Day);
        }

        public override string ToString()
        {
            var dt = new DateTime(year, month, day);

            return dt.ToString("yyyy-MM-dd");
        }

        public override int GetHashCode()
        {
            return year.GetHashCode() ^ month.GetHashCode() ^ day.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as MyCustomDateOnly);
        }

        public bool Equals(MyCustomDateOnly other)
        {
            if (other == null)
            {
                return false;
            }

            return year == other.year && month == other.month && day == other.day;
        }
    }
}

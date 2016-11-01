using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public enum SortBy { Date = 0, Rating, Name, Comment }
    public enum SortDirection { Desc = 0, Asc }

    public static class Util
    {

        public const string PAGENUMBER = "1";
        public const string ITEMNUMBER = "5";
        

        /// <summary>
        /// Parses the passed in string to "SortBy" enum, defaults to "Date" if the input is invalid
        /// </summary>
        /// <param name="sortBy"></param>
        /// <returns></returns>
        public static SortBy parseQstringSortBy(string sortBy)
        {
            SortBy retVal;
            Enum.TryParse(sortBy, true, out retVal);
            return retVal;
        }

        /// <summary>
        /// Parses the passed in string to "SortDirection" enum, defaults to "Desc" if the input is invalid
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static SortDirection parseQstringSortDirection(string dir)
        {
            SortDirection retVal;
            Enum.TryParse(dir, true, out retVal);
            return retVal;
        }

        /// <summary>
        /// Parses the passed in string to int, depending on the second parameter defaults to "1" or "5" if the input is invalid
        /// </summary>
        /// <param name="number"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public static int parseQStringNumber(string number, bool pageNumber = false)
        {
            int retVal;
            int.TryParse(number, out retVal);
            if (retVal == 0)
            {
                if (pageNumber) return int.Parse(PAGENUMBER);
                else return int.Parse(ITEMNUMBER);
            }
            return retVal;
        }

        /// <summary>
        /// Extension method for paging
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="pageNumber">The number of the item blocks</param>
        /// <param name="noOfItems">The number of items to be returned from pageNumber</param>
        /// <returns></returns>
        public static IEnumerable<TSource> Page<TSource>(this IEnumerable<TSource> source, int pageNumber, int noOfItems)
        {
            return source.Skip((pageNumber - 1) * noOfItems).Take(noOfItems);
        }

    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Helpers
{
    public class PagedList<T>
    {
        public PagedList(IEnumerable<T> source, int pageNumber, int pageSize, int totalItems)
        {
            this.TotalItems = totalItems;
            this.PageNumber = pageNumber;
            this.PageSize = pageSize;
            this.List = source;
        
            GetTotalPages();
        }

        public int TotalItems { get; }
        public int PageNumber { get; }
        public int PageSize { get; }
        public int[] ArrayTotalPages { get; set; }
        public IEnumerable<T> List { get; }
        public int TotalPages =>
              (int)Math.Ceiling(this.TotalItems / (double)this.PageSize);
        public bool HasPreviousPage => this.PageNumber > 1;
        public bool HasNextPage => this.PageNumber < this.TotalPages;
        public int NextPageNumber =>
               this.HasNextPage ? this.PageNumber + 1 : this.TotalPages;
        public int PreviousPageNumber =>
               this.HasPreviousPage ? this.PageNumber - 1 : 1;

        public PagingHeader GetHeader()
        { 
            return new PagingHeader(
                 this.TotalItems, this.PageNumber,
                 this.PageSize, this.TotalPages, ArrayTotalPages);
        }

        private void GetTotalPages()
        {
            ArrayTotalPages = new int[TotalPages];
            for (int i = 0; i < TotalPages; i++)
            {
                ArrayTotalPages[i] = i + 1;
            }
        }
    }
}

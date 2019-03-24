using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Helpers
{
    public class PagingHeader
    {
        public PagingHeader(
           int totalItems, int pageNumber, int pageSize, int totalPages,int[] totalPagesArray)
        {
            this.TotalItems = totalItems;
            this.PageNumber = pageNumber;
            this.ArrayTotalPages = totalPagesArray;
            this.PageSize = pageSize;
            this.TotalPages = totalPages;
        }

        public int TotalItems { get; }
        public int PageNumber { get; }
        public int PageSize { get; }
        public int TotalPages { get; }
        public int[] ArrayTotalPages { get; }
        public string ToJson() => JsonConvert.SerializeObject(this,
                                    new JsonSerializerSettings
                                    {
                                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                                    });

    }
}

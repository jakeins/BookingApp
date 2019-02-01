using BookingApp.Models;
using BookingApp.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Services
{
    public class TreeService
    {
        TreeRepository repository;

        public TreeService(TreeRepository r)
        {
            repository = r;
        }

        public List<TreeGroup> GetThree()
        {
            /*
            TreeGroup tr = new TreeGroup { TreeGroupId = 1, Title = "aas" };
            TreeGroup tr2 = new TreeGroup { TreeGroupId = 2, Title = "dddd", ParentTreeGroupId = 1 };
            List<TreeGroup> ll = new List<TreeGroup>();
            ll.Add(tr);
            ll.Add(tr2);

            return ll;
            */
            return (repository.GetList()).ToList();
        }

        public TreeGroup GetDetail(int id)
        {
            return repository.Get(id);
        }

        public void Delete(int id)
        {
            repository.Delete(id);
        }


    }
}

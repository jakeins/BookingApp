using BookingApp.Data;
using BookingApp.Models;
using BookingApp.Models.Dto;
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
        ApplicationDbContext context;

        public TreeService(TreeRepository r, ApplicationDbContext c)
        {
            repository = r;
            context = c;
        }

        public List<TreeGroup> GetThree()
        {
            return (repository.GetList()).ToList();
        }

        public TreeGroup GetDetail(int id)
        {
            return repository.Get(id);
        }

        public void Create(CreateTree t)
        {
            TreeGroup tree = new TreeGroup
            {
                Title = t.Title,
                ParentTreeGroupId = t.Parent,
                DefaultRuleId = t.Rule,
                CreatedUserId = t.UserCreate,
                UpdatedUserId = t.UserUpdate
            };
            repository.Create(tree);
        }

        public void Update(UpdateTree t)
        {
            TreeGroup tree = context.TreeGroups.FirstOrDefault(p => p.TreeGroupId == t.Id);
            if (tree != null)
            {
                tree.Title = t.Title;
                tree.ParentTreeGroupId = t.Parent;
                tree.DefaultRuleId = t.Rule;
                tree.CreatedUserId = t.UserCreate;
                tree.UpdatedUserId = t.UserUpdate;
                repository.Update(tree);
            }
        }

        public bool Delete(int id)
        {
            TreeGroup tree = context.TreeGroups.FirstOrDefault(p => p.TreeGroupId == id);
            if (tree != null)
            {
                repository.Delete(id);
                return true;
            } else
            {
                return false;
            }
        }


    }
}

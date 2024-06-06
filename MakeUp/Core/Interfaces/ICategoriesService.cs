using Infastructure.Entities;
using Core.Models.Categories;

namespace Core.Interfaces
{
    public interface ICategoriesService
    {
        IQueryable<CategoryEntity> Categories { get; }
        Task<int> Create(CategoryCreateViewModel entity);
        Task Update(CategoryEditViewModel entity);
        Task<bool> Delete(int id);
    }
}

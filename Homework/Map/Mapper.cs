using AutoMapper;
using Homework.Models;
using Homework.ViewModel;

namespace Homework.Map
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<SelectedEmployeeList, Employee>();
            CreateMap<Employee, SelectedEmployeeList>();
            CreateMap<Employee, EmployeeAdd>();
        }
    }
}

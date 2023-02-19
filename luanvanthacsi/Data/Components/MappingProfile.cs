using AutoMapper;
using luanvanthacsi.Data.Data;
using luanvanthacsi.Data.Entities;

namespace luanvanthacsi.Data.Components
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<StudentData, Student>();
            CreateMap<Student, StudentData>();
        }
    }
}

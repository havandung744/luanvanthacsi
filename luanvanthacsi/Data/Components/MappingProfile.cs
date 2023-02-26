using AutoMapper;
using luanvanthacsi.Data.Data;
using luanvanthacsi.Data.Edit;
using luanvanthacsi.Data.Entities;
using luanvanthacsi.Pages.AdminPages.ScientistPages;

namespace luanvanthacsi.Data.Components
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<StudentData, Student>().ReverseMap();
            CreateMap<ScientistData, Scientist>().ReverseMap()
                .ForMember(src => src.CreateDate, dest => dest.Ignore());
            CreateMap<ScientistEditModel, Scientist>().ReverseMap();
            CreateMap<EvaluationBoardData, EvaluationBoard>().ReverseMap();
            CreateMap<EvaluationBoardEditModel, EvaluationBoard>().ReverseMap();
        }
    }
}

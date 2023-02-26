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
            // Map học viên
            CreateMap<StudentData, Student>().ReverseMap();
            CreateMap<StudentEditModel, Student>().ReverseMap();

            // map nhà khoa học
            CreateMap<ScientistData, Scientist>().ReverseMap();
            CreateMap<ScientistEditModel, Scientist>().ReverseMap();

            // map hội đồng đánh giá
            CreateMap<EvaluationBoardData, EvaluationBoard>().ReverseMap();
            CreateMap<EvaluationBoardEditModel, EvaluationBoard>().ReverseMap();

            // map đợt bảo vệ
            CreateMap<ThesisDefenseData, ThesisDefense>().ReverseMap();
            CreateMap<ThesisDefenseEditModel, ThesisDefense>().ReverseMap();

        }
    }
}

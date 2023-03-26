using AutoMapper;
using luanvanthacsi.Data.Data;
using luanvanthacsi.Data.Edit;
using luanvanthacsi.Data.Entities;
using luanvanthacsi.Data.Extentions;
using luanvanthacsi.Excel.ClassExcel;
using luanvanthacsi.Models;
using luanvanthacsi.Pages.AdminPages.ScientistPages;
using luanvanthacsi.Ultils;

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
            //CreateMap<ScientistEditModel, Scientist>()
            //     .ForMember(src => src.Specialized.Id, dest => dest.MapFrom(c => c.SpecializedId)).ReverseMap();
            CreateMap<Scientist, ScientistEditModel>()
                .ForMember(src => src.SpecializedId, dest => dest.MapFrom(c => c.Specialized.Id)).ReverseMap();

            // map hội đồng đánh giá
            CreateMap<EvaluationBoardData, EvaluationBoard>().ReverseMap();
            CreateMap<EvaluationBoardEditModel, EvaluationBoard>().ReverseMap();
            CreateMap<EvaluationBoardEditModel, EvaluationBoardData>().ReverseMap();

            // map đợt bảo vệ
            CreateMap<ThesisDefenseData, ThesisDefense>().ReverseMap();
            CreateMap<ThesisDefenseEditModel, ThesisDefense>().ReverseMap();

            // map chủ tịch
            CreateMap<Lecturers, LecturersData>().ReverseMap();
            //CreateMap<ThesisDefenseEditModel, ThesisDefense>().ReverseMap();

            // map thư ký
            CreateMap<Secretary, SecretaryData>().ReverseMap();

            // map excel
            CreateMap<Student, StudentExportExcel>()
               .ForMember(src => src.DateOfBirth, dest => dest.MapFrom(c => c.DateOfBirth.ToShortDate())).ReverseMap();

            CreateMap<EvaluationBoardData, EvaluationBoardExcel>().ReverseMap();
            CreateMap<Scientist, EvaluationBoardDocx.EvaluationBoard>()
                .ForMember(dest => dest.Title, src => src.MapFrom(c => c.EvaluationRole.GetDescription()))
                .ForMember(dest => dest.Price, src => src.MapFrom(c => c.EvaluationRole.GetDegreeFee()))
                .ForMember(dest => dest.WorkUnit, src => src.MapFrom(c => c.WorkingAgency));

        }
    }
}

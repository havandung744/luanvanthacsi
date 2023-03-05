﻿using System.ComponentModel.DataAnnotations;

namespace luanvanthacsi.Data.Data
{
    public class EvaluationBoardData
    {
        [Display(Name = "STT")]
        public int stt;
        public string Id;
        [Display(Name = "Mã")]
        public string Code;
        [Display(Name = "Họ tên")]
        public string Name;
        [Display(Name = "Ngày tạo")]
        public DateTime CreateDate;
        [Display(Name = "Ngày Cập nhật")]
        public DateTime UpdateDate;
        [Display(Name = "Id học viên")]
        public string StudentId;
        [Display(Name = "Id khoa")]
        public string FacultyId;
        [Display(Name = "Id chủ tịch")]
        public string PresidentId;
        [Display(Name = "Id phản biện 1")]
        public string CounterattackerIdOne;
        [Display(Name = "Id phản biện 2")]
        public string CounterattackerIdTwo;
        [Display(Name = "Id phản biện 3")]
        public string CounterattackerIdThree;
        [Display(Name = "Id nhà khoa học 1")]
        public string ScientistIdOne;
        [Display(Name = "Id nhà khoa học 2")]
        public string ScientistIdTwo;
        [Display(Name = "Id thư ký")]
        public string SecretaryId;
    }
}

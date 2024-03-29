﻿using luanvanthacsi.Data.Data;
using System.ComponentModel.DataAnnotations;

namespace luanvanthacsi.Data.Edit
{
    public class ThesisDefenseEditModel
    {
        public string Id { get; set; }
        public string Code { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên đợt bảo vệ")]
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập năm đợt bảo vệ")]
        public DateTime YearOfProtection { get; set; }
    }

}

using System.ComponentModel.DataAnnotations;

namespace luanvanthacsi.Data.Data
{
    public class UserData
    {
        [Display(Name = "STT")]
        public int stt;
        public string Id;
        [Display(Name = "Tên Đăng nhập")]
        public string UserName;
    }
}

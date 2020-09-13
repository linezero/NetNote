using System.ComponentModel.DataAnnotations;

namespace NetNote.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "{0} 不能为空")]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "{0} 不能为空")]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Display(Name = "记住登录状态")]
        public bool RememberMe { get; set; }
    }
}

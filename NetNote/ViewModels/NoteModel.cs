using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace NetNote.ViewModels
{
    public class NoteModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "{0} 不能为空")]
        [Display(Name = "标题")]
        [MaxLength(100,ErrorMessage ="{0} 长度不能超过100")]
        public string Title { get; set; }
        [Required(ErrorMessage = "{0} 不能为空")]
        [Display(Name = "内容")]
        public string Content { get; set; }
        [Display(Name = "类型")]
        public int Type { get; set; }
        [Display(Name = "密码")]
        public string Password { get; set; }
        [Display(Name = "附件")]
        public IFormFile Attachment { get; set; }
    }
}

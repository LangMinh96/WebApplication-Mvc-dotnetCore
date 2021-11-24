using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models.Contact
{
    public class ContactModel
    {
        [Key]
        [Required]
        [Display(Name ="ID")]
        public int ID { get; set; }

        [Required(ErrorMessage ="Phải nhập họ tên")]
        [Column(TypeName ="nvarchar")]
        [StringLength(50)]
        [Display(Name ="Họ và tên")]
        public string Name { get; set; }

        [Display(Name ="Email")]
        [Required(ErrorMessage = "Phải nhập vào E-mail")]
        [EmailAddress(ErrorMessage = "Phải là địa chỉ E-mail")]
        public string Email { get; set; }

        [Display(Name = "Ngày gửi")]
        public DateTime DateSend { get; set; }

        [Display(Name ="Nội dung")]
        public string Message { get; set; }

        [StringLength(15)]
        [Display(Name ="Số điện thoại")]
        [Phone(ErrorMessage ="Phải là số điện thoại")]
        public string Phone { get; set; }
    }
}

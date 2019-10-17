using System.ComponentModel.DataAnnotations;

namespace MyCompany.MyProject.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}
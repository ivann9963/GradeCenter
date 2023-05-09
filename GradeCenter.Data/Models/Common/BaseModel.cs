using System.ComponentModel.DataAnnotations;

namespace GradeCenter.Data.Models.Common
{
    public class BaseModel<T>
    {
        [Key]
        public T Id { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;
    }
}

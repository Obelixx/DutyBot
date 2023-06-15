using System.ComponentModel.DataAnnotations;

namespace DutyBot.Models
{
    public class DutyModel: IComparable<DutyModel>, IComparer<DutyModel>
    {
        [DisplayFormat(DataFormatString = "yyyy-MM-dd")]
        public DateTime Date { get; set; }
        
        public string Name { get; set; }

        public int Compare(DutyModel x, DutyModel y)
        {
            return x.CompareTo(y);
        }

        public int CompareTo(DutyModel other)
        {
            return Date.Date.CompareTo(other.Date.Date);
        }
    }
}

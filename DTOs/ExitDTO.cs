namespace Parking.DTOs
{
    public class ExitDTO
    {
        public int EntryExitId { get; set; }
        public DateTime ExitTime { get; set; } = DateTime.Now;

    }
}

using DutyBot.Models;

namespace DutyBot.Common.Contracts
{
    public interface IDutyStorage
    {
        void CleanUp(string chatId);

        DutyModel DutyToday(string chatId);

        IEnumerable<DutyModel> GetAllDuties(string chatId);

        DutyModel GetDuty(string chatId, DateTime date);

        void SaveDuty(string chatId, DutyModel duty);
    }
}
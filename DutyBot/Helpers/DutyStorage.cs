using DutyBot.Common.Contracts;
using DutyBot.Models;

namespace DutyBot.Helpers
{
    public class DutyStorage : IDutyStorage
    {
        private static Dictionary<string, SortedSet<DutyModel>> inmemoryStorage = new Dictionary<string, SortedSet<DutyModel>>();

        /// <summary>
        /// Create and update
        /// </summary>
        /// <param name="duty"></param>
        public void SaveDuty(string chatId, DutyModel duty)
        {
            duty.Date = duty.Date.Date;
            if (inmemoryStorage.ContainsKey(chatId))
            {
                if (inmemoryStorage[chatId].Contains(duty))
                {
                    inmemoryStorage[chatId].First(d => d.Date == duty.Date).Name = duty.Name;
                }
                else
                {
                    inmemoryStorage[chatId].Add(duty);
                }
            }
            else
            {
                inmemoryStorage.Add(chatId, new SortedSet<DutyModel>(new DutyModel() as IComparer<DutyModel>));
                inmemoryStorage[chatId].Add(duty);
            }
        }

        /// <summary>
        /// Can return null.
        /// </summary>
        /// <param name="date">Date component is used only.</param>
        /// <returns></returns>
        public DutyModel GetDuty(string chatId, DateTime date)
        {
            if (inmemoryStorage.ContainsKey(chatId))
            {
                return inmemoryStorage[chatId].FirstOrDefault(d => d.Date.Date == date.Date);
            }

            return null;
        }

        public DutyModel DutyToday(string chatId)
        {
            // TODO: think about timezones
            return GetDuty(chatId, DateTime.UtcNow.Date);
        }

        public IEnumerable<DutyModel> GetAllDuties(string chatId)
        {
            if(inmemoryStorage.ContainsKey(chatId))
            {
                return inmemoryStorage[chatId];
            }

            return Enumerable.Empty<DutyModel>();
        }

        /// <summary>
        /// Delete old duties.
        /// </summary>
        public void CleanUp(string chatId)
        {
            if (inmemoryStorage.ContainsKey(chatId))
            {
                inmemoryStorage[chatId].RemoveWhere(x => x.Date <= DateTime.UtcNow.Date.AddDays(-1));
            }
        }
    }
}

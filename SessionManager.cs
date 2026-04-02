using AnyTeller.Models;
using System.Collections.Generic;

namespace AnyTeller.Services
{
    public class SessionManager
    {
        public List<UserRecord> Records { get; private set; }
        public int CurrentIndex { get; private set; }
        public int Score { get; private set; }
        public int TotalProcessed { get; private set; }

        public SessionManager(List<UserRecord> records)
        {
            Records = records;
            CurrentIndex = 0;
            Score = 0;
            TotalProcessed = 0;
        }

        public UserRecord GetCurrentRecord()
        {
            if (CurrentIndex < Records.Count)
                return Records[CurrentIndex];
            return null;
        }

        public void SubmitRound(bool isAccurate)
        {
            TotalProcessed++;
            if (isAccurate)
            {
                Score++;
            }
            CurrentIndex++;
        }

        public bool IsSessionComplete()
        {
            return CurrentIndex >= Records.Count;
        }
    }
}

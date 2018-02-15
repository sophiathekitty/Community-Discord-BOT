using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityBot.Helpers
{
    public class Result
    {
        public bool Success { get; private set; }
        public List<Alert> Alerts { get; }

        public Result()
        {
            Success = true;
            Alerts = new List<Alert>();
        }

        /// <summary>
        /// Fügt alle Alerts eines Results in das aktuelle Result ein und übernimmt den Status des Success
        /// </summary>
        /// <param name="r"></param>
        public void Merge(Result r)
        {
            if (r != null)
            {
                foreach (var error in r.Alerts)
                {
                    Alerts.Add(error);
                }
                if (r.Success == false)
                {
                    Success = false;
                }
            }
        }

        /// <summary>
        /// Fügt einen Alert in die AlertListe ein
        /// </summary>
        /// <param name="alert"></param>
        public void AddAlert(Alert alert)
        {
            switch (alert.Level)
            {
                case LevelEnum.Success:
                    break;
                case LevelEnum.Info:
                    break;
                case LevelEnum.Error:
                case LevelEnum.Exception:
                    Success = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            Alerts.Add(alert);
        }
    }

    public class Result<T> : Result
    {
        public T Value { get; set; }
    }

    public class Alert
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public LevelEnum Level { get; set; }

        public Alert(string name, string description, LevelEnum level)
        {
            Name = name;
            Description = description;
            Level = level;
        }
    }

    public enum LevelEnum
    {
        Success = 0,
        Info = 10,
        Error = 20,
        Exception = 30
    }
}
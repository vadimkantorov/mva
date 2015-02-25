using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Clik
{
    public class ClickLogReader
    {
        public Query ReadQueryClicks(int sessionId)
        {
            if (currentLine == null)
                return new Query { QueryId = -1 };
            var queryLineFields = currentLine.Split(new [] {"\t"}, StringSplitOptions.RemoveEmptyEntries);
            int sessionId_ = Convert.ToInt32(queryLineFields[0]);
            if (sessionId_ != sessionId)
                return new Query { QueryId = -1 };
            int timePassed = Convert.ToInt32(queryLineFields[1]);
            int queryID = Convert.ToInt32(queryLineFields[3]);
            int regionID = Convert.ToInt32(queryLineFields[4]);
            int[] listOfURLs = queryLineFields.Skip(5).Select(x => Convert.ToInt32(x)).ToArray();
            if (listOfURLs.Length != Constants.Ranks) throw new Exception("Not 10 URLs");

            bool[] isClicked = new bool[Constants.Ranks];
            int[] clickTimePassed = Enumerable.Repeat(-1, Constants.Ranks).ToArray();
            var query = new Query
            {
                ClickTimePassed = clickTimePassed,
                IsClicked = isClicked,
                QueryId = queryID,
                URLs = listOfURLs,
                QueryTimePassed = timePassed,
                RegionId = regionID
            };

            while (true)
            {
                currentLine = sr.ReadLine();
                if (currentLine == null)
                    break;

                var clickLineFields = currentLine.Split('\t');
                int sessionId__ = Convert.ToInt32(clickLineFields[0]);
                if (sessionId__ != sessionId)
                    break;

                if (clickLineFields[2].Single() != 'C')
                    break;

                int clickTimePassed_ = Convert.ToInt32(clickLineFields[1]);
                int urlId = Convert.ToInt32(clickLineFields[3]);
                for (int k = 0; k < listOfURLs.Length; k++)
                    if (listOfURLs[k] == urlId)
                    {
                        clickTimePassed[k] = clickTimePassed_;
                        isClicked[k] = true;
                        break;
                    }
            }
            return query;
        }

        public IEnumerable<Query> ReadQueries()
        {
            return ReadSessions().SelectMany(x => x.Queries);
        }

        public IEnumerable<Session> ReadSessions(int count = 52000000)
        {
            for (int i = 0; i < count; i++)
            {
                if (currentLine == null)
                    yield break;

                var session = new Session { SessionId = Convert.ToInt32(currentLine.Split('\t').First()) };

                var qs = new List<Query>();
                while (true)
                {
                    var q = ReadQueryClicks(session.SessionId);
                    if (q.QueryId == -1)
                        break;
                    qs.Add(q);
                }
                session.Queries = qs.ToArray();
                yield return session;
            }
        }

        public ClickLogReader(string filePath)
        {
            sr = new StreamReader(filePath);
            currentLine = sr.ReadLine();
        }

        readonly StreamReader sr;
        string currentLine;
    }
}
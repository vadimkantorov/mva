using System;
using System.Collections.Generic;
using System.IO;

namespace Clik
{
    public class ClickLogWriter : IDisposable
    {
        public void WriteSessions(IEnumerable<Session> sessions)
        {
            Action tab = () => sw.Write('\t');
            foreach (var session in sessions)
            {
                foreach (var q in session.Queries)
                {
                    sw.Write(session.SessionId);
                    tab();
                    
                    sw.Write(q.QueryTimePassed);
                    tab();
                    
                    sw.Write('Q');
                    tab();
                    
                    sw.Write(q.QueryId);
                    tab();

                    sw.Write(q.RegionId);
                    tab();

                    sw.WriteLine(string.Join("\t", q.URLs));
                    for(int i = 0; i < Constants.Ranks; i++)
                    {
                        if (q.IsClicked[i])
                        {
                            sw.Write(session.SessionId);
                            tab();

                            sw.Write(q.ClickTimePassed[i]);
                            tab();

                            sw.Write('C');
                            tab();

                            sw.WriteLine(q.URLs[i]);
                        }
                    }
                }
            }
        }
        
        public ClickLogWriter(string filePath)
        {
            sw = new StreamWriter(filePath);
        }

        readonly StreamWriter sw;
        public void Dispose()
        {
            sw.Flush();
            sw.Dispose();
        }
    }
}
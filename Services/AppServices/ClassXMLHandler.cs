using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices
{
    public class ClassXMLHandler
    {

        private static readonly object padlock = new object();
        private static ClassXMLHandler? _instance = null;
        public static ClassXMLHandler Instance
        {
            get
            {
                lock (padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new ClassXMLHandler();
                    }
                    return _instance;
                }
            }
        }




        public ClassXMLHandler() { }
    }
}

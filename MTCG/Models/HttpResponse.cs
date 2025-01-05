using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models
{
    public class HttpResponse
    {
        public string _status;
        public string _body;

        public HttpResponse(string status, string body)
        {
            this._status = status;
            this._body = body;
        }
    }
}

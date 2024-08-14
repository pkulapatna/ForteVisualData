using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices
{
    public class CheckedListItem
    {

        public int Id { get; set; }

        private bool _ischeck;
        public bool IsChecked
        {
            get { return _ischeck; }
            set { _ischeck = value; }
        }

        public string Name { get; set; }
        public string FieldType { get; set; }

        public CheckedListItem(int _id, string _name, bool _check, string _ftype) 
        {
            Id = _id;
            IsChecked = _check;
            Name = _name;
            FieldType = _ftype;

        }
    }
}

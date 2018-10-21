using System;

namespace rokhan.inc
{
    public class ProcessEventArgs : EventArgs
    {
        int _key1;
        int _key2;

        public int Key1
        {
            get
            {
                return _key1;
            }
            set
            {
                _key1 = value;
            }
        }

        public int Key2
        {
            get
            {
                return _key2;
            }
            set
            {
                _key2 = value;
            }
        }
    }
}

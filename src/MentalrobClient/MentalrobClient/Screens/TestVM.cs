using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Library;

namespace MentalrobClient.Screens
{
    class TestVM : ViewModel
    {
        private string _test = "test";
        [DataSourceProperty]
        public string Test
        {
            get => _test;
            set
            {
                _test = value;
                OnPropertyChanged();
            }
        }
    }
}

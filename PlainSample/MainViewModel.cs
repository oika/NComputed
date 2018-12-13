using NComputed;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlainSample
{
    /// <summary>
    /// ViewModel class of MainWindow.
    /// </summary>
    class MainViewModel : IPropertyChangedRaisable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int _num1;

        public int Num1
        {
            get
            {
                return _num1;
            }
            set
            {
                if (_num1 == value) return;
                _num1 = value;
                RaisePropertyChanged(nameof(Num1));
            }
        }

        private int _num2;

        public int Num2
        {
            get
            {
                return _num2;
            }
            set
            {
                if (_num2 == value) return;
                _num2 = value;
                RaisePropertyChanged(nameof(Num2));
            }
        }

        // Computed when the value of Num1 or Num2 changed.
        public Computed<int> Sum { get; }

        public MainViewModel()
        {
            // init
            this.Sum = Computed<int>.Of(this, nameof(Sum))
                                    .Observe(nameof(Num1), nameof(Num2))
                                    .ComputeAs(me => me.Num1 + me.Num2)
                                    .Build();
        }


        public void RaisePropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

using NComputed;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrismSample
{
    /// <summary>
    /// ViewModel class of MainWindow.
    /// </summary>
    class MainViewModel : BindableBase, IPropertyChangedRaisable
    {
        private int _num1;
        private int _num2;

        public int Num1
        {
            get
            {
                return _num1;
            }
            set
            {
                if (_num1 == value) return;
                SetProperty(ref _num1, value);
            }
        }
        public int Num2
        {
            get
            {
                return _num2;
            }
            set
            {
                if (_num2 == value) return;
                SetProperty(ref _num2, value);
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

        public new void RaisePropertyChanged(string propertyName)
        {
            base.RaisePropertyChanged(propertyName);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NComputed
{
    /// <summary>
    /// An interface which defines PropertyChanged caller.
    /// </summary>
    public interface IPropertyChangedRaisable : INotifyPropertyChanged
    {
        /// <summary>
        /// Raise PropertyChanged event.
        /// </summary>
        /// <param name="propertyName"></param>
        void RaisePropertyChanged(string propertyName);
    }

    /// <summary>
    /// Base abstract class of Computed.
    /// </summary>
    public abstract class ComputedBase
    {
        /// <summary>
        /// Gets the value contained.
        /// </summary>
        /// <returns></returns>
        public abstract object GetValue();
    }

    /// <summary>
    /// A wrapper of an auto computed value.
    /// </summary>
    /// <typeparam name="T">Type of the contained value.</typeparam>
    public class Computed<T> : ComputedBase, IDisposable
    {
        /// <summary>
        /// Creates a builder of Computed Object.
        /// </summary>
        /// <typeparam name="TObj">Type of viewModel.</typeparam>
        /// <param name="viewModel">The object which has this computed property.</param>
        /// <param name="name">The name of this computed property.</param>
        /// <returns></returns>
        public static Builder<TObj, T> Of<TObj>(TObj viewModel, string name) where TObj : IPropertyChangedRaisable
        {
            return new Builder<TObj, T>(viewModel, name);
        }
        /// <summary>
        /// The Builder of Computed Objects.
        /// </summary>
        /// <typeparam name="TObj">Type of viewModel.</typeparam>
        /// <typeparam name="TValue">Type of the value contained in the Computed Object.</typeparam>
        public class Builder<TObj, TValue> where TObj : IPropertyChangedRaisable
        {
            private Func<IPropertyChangedRaisable, TValue> compute;
            private readonly List<string> observedPropNameList = new List<string>();
            private readonly TObj viewModel;
            private string propertyName;

            internal protected Builder(TObj viewModel, string name)
            {
                if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));
                if (name == null) throw new ArgumentNullException(nameof(name));

                this.viewModel = viewModel;
                this.propertyName = name;
            }

            /// <summary>
            /// Sets how to compute the value and returns the object itself.
            /// </summary>
            /// <param name="func"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException"></exception>
            public Builder<TObj, TValue> ComputeAs(Func<TObj, TValue> func)
            {
                if (func == null) throw new ArgumentNullException(nameof(func));

                this.compute = o => func((TObj)o);
                return this;
            }

            /// <summary>
            /// Sets the names of the observed properties.
            /// </summary>
            /// <param name="propertyNames"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException"></exception>
            public Builder<TObj, TValue> Observe(params string[] propertyNames)
            {
                if (propertyNames == null || propertyNames.Any(n => n == null))
                {
                    throw new ArgumentNullException(nameof(propertyNames));
                }

                this.observedPropNameList.AddRange(propertyNames);
                return this;
            }

            /// <summary>
            /// Builds the Computed object.
            /// </summary>
            /// <returns></returns>
            /// <exception cref="InvalidOperationException">Thrown if <see cref="ComputeAs(Func{TObj, TValue})" />was not called.</exception>
            public Computed<TValue> Build()
            {
                if (this.viewModel == null || this.propertyName == null || this.compute == null)
                {
                    throw new InvalidOperationException();
                }
                return new Computed<TValue>(this.viewModel, this.propertyName, this.observedPropNameList, this.compute);
            }
        }


        private IPropertyChangedRaisable viewModel;
        private readonly IReadOnlyCollection<string> propertyNames;
        private Func<IPropertyChangedRaisable, T> compute;
        private bool disposedValue;

        /// <summary>
        /// Gets the name of the Computed property.
        /// </summary>
        public string Name { get; }

        private T _val;
        /// <summary>
        /// Gets the contained value.
        /// </summary>
        public T Value
        {
            get
            {
                return _val;
            }
            private set
            {
                if (_val == null && value == null) return;
                if (_val != null && _val.Equals(value)) return;
                _val = value;

                viewModel.RaisePropertyChanged(this.Name);
            }
        }

        public override object GetValue()
        {
            return Value;
        }


        protected Computed(IPropertyChangedRaisable viewModel, string name, IReadOnlyCollection<string> propertyNames, Func<IPropertyChangedRaisable, T> compute)
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (propertyNames == null) throw new ArgumentNullException(nameof(propertyNames));
            if (compute == null) throw new ArgumentNullException(nameof(compute));

            this.Name = name;
            this.viewModel = viewModel;
            this.propertyNames = propertyNames;
            this.compute = compute;

            this._val = computeVal();

            viewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!this.propertyNames.Contains(e.PropertyName)) return;

            this.Value = computeVal();
        }

        private T computeVal()
        {
            if (this.viewModel == null) throw new ObjectDisposedException(this.GetType().Name);
            return this.compute(this.viewModel);
        }

        public override string ToString()
        {
            return this.Value == null ? "" : this.Value.ToString();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.compute = null;
                    if (this.viewModel != null)
                    {
                        this.viewModel.PropertyChanged -= OnViewModelPropertyChanged;
                        this.viewModel = null;
                    }
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Disposes this instance.
        /// Note: You should call this if this instance is no longer used unused before the viewModel. 
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using Frost.GettextMarkupExtension.Annotations;

namespace Frost.GettextMarkupExtension {

    public class TBinding : MarkupExtension, IWeakEventListener, INotifyPropertyChanged, IDisposable {
        private static readonly Type LanguageChangedEventManagerType = typeof(LanguageChangedEventManager);
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly Binding _binding;

        public class DepObj : DependencyObject {
            private readonly TBinding _parent;

            public static readonly DependencyProperty FallbackValueProperty = DependencyProperty.Register("FallbackValue", typeof(object), typeof(DepObj),
                new PropertyMetadata(default(object), FallbackValuePropertyChanged));

            public static readonly DependencyProperty TargetNullValueProperty = DependencyProperty.Register("TargetNullValue", typeof(object), typeof(DepObj),
                new PropertyMetadata(default(object), TargetNullValuePropertyChanged));

            public DepObj(TBinding tb) {
                _parent = tb;
            }

            private static void FallbackValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args) {
                ((DepObj)d)._parent._binding.FallbackValue = args.NewValue;
            }

            /// <summary>Gets or sets the value to use when the binding is unable to return a value.</summary>
            /// <returns>The default value is <see cref="F:System.Windows.DependencyProperty.UnsetValue"/>.</returns>
            public object FallbackValue {
                get { return GetValue(FallbackValueProperty); }
                set { SetValue(FallbackValueProperty, value); }
            }

            private static void TargetNullValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args) {
                ((DepObj)d)._parent._binding.TargetNullValue = args.NewValue;
            }

            /// <summary>Gets or sets the value that is used in the target when the value of the source is null.</summary>
            /// <returns>The value that is used in the target when the value of the source is null.</returns>
            public object TargetNullValue {
                get { return GetValue(TargetNullValueProperty); }
                set { SetValue(TargetNullValueProperty, value); }
            }
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Windows.Data.Binding"/> class.</summary>
        public TBinding() : this(null) {
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Windows.Data.Binding"/> class with an initial path.</summary>
        /// <param name="path">The initial <see cref="P:System.Windows.Data.Binding.Path"/> for the binding.</param>
        public TBinding(string path) {
            _binding = new Binding(path);
            T = new DepObj(this);

            LanguageChangedEventManager.AddListener(TranslationManager.Instance, this);
        }

        public DepObj T { get; private set; }

        /// <summary>Gets or sets the value to use when the binding is unable to return a value.</summary>
        /// <returns>The default value is <see cref="F:System.Windows.DependencyProperty.UnsetValue"/>.</returns>
        public object FallbackValue {
            get { return T.FallbackValue; }
            set { T.FallbackValue = value; }
        }

        /// <summary>Gets or sets the value that is used in the target when the value of the source is null.</summary>
        /// <returns>The value that is used in the target when the value of the source is null.</returns>
        public object TargetNullValue {
            get { return T.TargetNullValue; }
            set { T.FallbackValue = value; }
        }

        #region Binding Forwarders

        /// <summary>Returns a value that indicates whether serialization processes should serialize the effective value of the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"/> property on instances of this class.</summary>
        /// <returns>true if the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"/> property value should be serialized; otherwise, false.</returns>
        public bool ShouldSerializeFallbackValue() {
            return _binding.ShouldSerializeFallbackValue();
        }

        /// <summary>Returns a value that indicates whether the <see cref="P:System.Windows.Data.BindingBase.TargetNullValue"/> property should be serialized.</summary>
        /// <returns>true if the <see cref="P:System.Windows.Data.BindingBase.TargetNullValue"/> property should be serialized; otherwise, false.</returns>
        public bool ShouldSerializeTargetNullValue() {
            return _binding.ShouldSerializeTargetNullValue();
        }

        /// <summary>Returns an object that should be set on the property where this binding and extension are applied.</summary>
        /// <returns>The value to set on the binding target property.</returns>
        /// <param name="serviceProvider">The object that can provide services for the markup extension. May be null; see the Remarks section for more information.</param>
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return _binding.ProvideValue(serviceProvider);
        }

        /// <summary>Gets or sets a string that specifies how to format the binding if it displays the bound value as a string.</summary>
        /// <returns>A string that specifies how to format the binding if it displays the bound value as a string.</returns>
        public string StringFormat {
            get { return _binding.StringFormat; }
            set { _binding.StringFormat = value; }
        }

        /// <summary>Gets or sets the name of the <see cref="T:System.Windows.Data.BindingGroup"/> to which this binding belongs.</summary>
        /// <returns>The name of the <see cref="T:System.Windows.Data.BindingGroup"/> to which this binding belongs.</returns>
        public string BindingGroupName {
            get { return _binding.BindingGroupName; }
            set { _binding.BindingGroupName = value; }
        }

        /// <summary>Indicates whether the <see cref="P:System.Windows.Data.Binding.ValidationRules"/> property should be persisted.</summary>
        /// <returns>true if the property value has changed from its default; otherwise, false.</returns>
        public bool ShouldSerializeValidationRules() {
            return _binding.ShouldSerializeValidationRules();
        }

        /// <summary>Indicates whether the <see cref="P:System.Windows.Data.Binding.Path"/> property should be persisted.</summary>
        /// <returns>true if the property value has changed from its default; otherwise, false.</returns>
        public bool ShouldSerializePath() {
            return _binding.ShouldSerializePath();
        }

        /// <summary>Indicates whether the <see cref="P:System.Windows.Data.Binding.Source"/> property should be persisted.</summary>
        /// <returns>true if the property value has changed from its default; otherwise, false.</returns>
        public bool ShouldSerializeSource() {
            return _binding.ShouldSerializeSource();
        }

        /// <summary>Gets a collection of rules that check the validity of the user input.</summary>
        /// <returns>A collection of <see cref="T:System.Windows.Controls.ValidationRule"/> objects.</returns>
        public Collection<ValidationRule> ValidationRules {
            get { return _binding.ValidationRules; }
        }

        /// <summary>Gets or sets a value that indicates whether to include the <see cref="T:System.Windows.Controls.ExceptionValidationRule"/>.</summary>
        /// <returns>true to include the <see cref="T:System.Windows.Controls.ExceptionValidationRule"/>; otherwise, false.</returns>
        public bool ValidatesOnExceptions {
            get { return _binding.ValidatesOnExceptions; }
            set { _binding.ValidatesOnExceptions = value; }
        }

        /// <summary>Gets or sets a value that indicates whether to include the <see cref="T:System.Windows.Controls.DataErrorValidationRule"/>.
        /// </summary>
        /// <returns>true to include the <see cref="T:System.Windows.Controls.DataErrorValidationRule"/>; otherwise, false.</returns>
        public bool ValidatesOnDataErrors {
            get { return _binding.ValidatesOnDataErrors; }
            set { _binding.ValidatesOnDataErrors = value; }
        }

        /// <summary>Gets or sets the path to the binding source property.</summary>
        /// <returns>The path to the binding source. The default is null.</returns>
        public PropertyPath Path {
            get { return _binding.Path; }
            set { _binding.Path = value; }
        }

        /// <summary>Gets or sets an XPath query that returns the value on the XML binding source to use.</summary>
        /// <returns>The XPath query. The default is null.</returns>
        public string XPath {
            get { return _binding.XPath; }
            set { _binding.XPath = value; }
        }

        /// <summary>Gets or sets a value that indicates the direction of the data flow in the binding.</summary>
        /// <returns>One of the <see cref="T:System.Windows.Data.BindingMode"/> values. The default is <see cref="F:System.Windows.Data.BindingMode.Default"/>, which returns the default binding mode value of the target dependency property. However, the default value varies for each dependency property. In general, user-editable control properties, such as those of text boxes and check boxes, default to two-way bindings, whereas most other properties default to one-way bindings.A programmatic way to determine whether a dependency property binds one-way or two-way by default is to get the property metadata of the property using <see cref="M:System.Windows.DependencyProperty.GetMetadata(System.Type)"/> and then check the Boolean value of the <see cref="P:System.Windows.FrameworkPropertyMetadata.BindsTwoWayByDefault"/> property.</returns>
        public BindingMode Mode {
            get { return _binding.Mode; }
            set { _binding.Mode = value; }
        }

        /// <summary>Gets or sets a value that determines the timing of binding source updates.</summary>
        /// <returns>One of the <see cref="T:System.Windows.Data.UpdateSourceTrigger"/> values. The default is <see cref="F:System.Windows.Data.UpdateSourceTrigger.Default"/>, which returns the default <see cref="T:System.Windows.Data.UpdateSourceTrigger"/> value of the target dependency property. However, the default value for most dependency properties is <see cref="F:System.Windows.Data.UpdateSourceTrigger.PropertyChanged"/>, while the <see cref="P:System.Windows.Controls.TextBox.Text"/> property has a default value of <see cref="F:System.Windows.Data.UpdateSourceTrigger.LostFocus"/>.A programmatic way to determine the default <see cref="P:System.Windows.Data.Binding.UpdateSourceTrigger"/> value of a dependency property is to get the property metadata of the property using <see cref="M:System.Windows.DependencyProperty.GetMetadata(System.Type)"/> and then check the value of the <see cref="P:System.Windows.FrameworkPropertyMetadata.DefaultUpdateSourceTrigger"/> property.</returns>
        public UpdateSourceTrigger UpdateSourceTrigger {
            get { return _binding.UpdateSourceTrigger; }
            set { _binding.UpdateSourceTrigger = value; }
        }

        /// <summary>Gets or sets a value that indicates whether to raise the <see cref="E:System.Windows.Data.Binding.SourceUpdated"/> event when a value is transferred from the binding target to the binding source.</summary>
        /// <returns>true if the <see cref="E:System.Windows.Data.Binding.SourceUpdated"/> event should be raised when the binding source value is updated; otherwise, false. The default is false.</returns>
        public bool NotifyOnSourceUpdated {
            get { return _binding.NotifyOnSourceUpdated; }
            set { _binding.NotifyOnSourceUpdated = value; }
        }

        /// <summary>Gets or sets a value that indicates whether to raise the <see cref="E:System.Windows.Data.Binding.TargetUpdated"/> event when a value is transferred from the binding source to the binding target.</summary>
        /// <returns>true if the <see cref="E:System.Windows.Data.Binding.TargetUpdated"/> event should be raised when the binding target value is updated; otherwise, false. The default is false.</returns>
        public bool NotifyOnTargetUpdated {
            get { return _binding.NotifyOnTargetUpdated; }
            set { _binding.NotifyOnTargetUpdated = value; }
        }

        /// <summary>Gets or sets a value that indicates whether to raise the <see cref="E:System.Windows.Controls.Validation.Error"/> attached event on the bound object.</summary>
        /// <returns>true if the <see cref="E:System.Windows.Controls.Validation.Error"/> attached event should be raised on the bound object when there is a validation error during source updates; otherwise, false. The default is false.</returns>
        public bool NotifyOnValidationError {
            get { return _binding.NotifyOnValidationError; }
            set { _binding.NotifyOnValidationError = value; }
        }

        /// <summary>Gets or sets the converter to use.</summary>
        /// <returns>A value of type <see cref="T:System.Windows.Data.IValueConverter"/>. The default is null.</returns>
        public IValueConverter Converter {
            get { return _binding.Converter; }
            set { _binding.Converter = value; }
        }

        /// <summary>Gets or sets the parameter to pass to the <see cref="P:System.Windows.Data.Binding.Converter"/>.</summary>
        /// <returns>The parameter to pass to the <see cref="P:System.Windows.Data.Binding.Converter"/>. The default is null.</returns>
        public object ConverterParameter {
            get { return _binding.ConverterParameter; }
            set { _binding.ConverterParameter = value; }
        }

        /// <summary>Gets or sets the culture in which to evaluate the converter.</summary>
        /// <returns>The default is null.</returns>
        public CultureInfo ConverterCulture {
            get { return _binding.ConverterCulture; }
            set { _binding.ConverterCulture = value; }
        }

        /// <summary>Gets or sets the object to use as the binding source.</summary>
        /// <returns>The object to use as the binding source.</returns>
        public object Source {
            get { return _binding.Source; }
            set { _binding.Source = value; }
        }

        /// <summary>Gets or sets the binding source by specifying its location relative to the position of the binding target.</summary>
        /// <returns>A <see cref="T:System.Windows.Data.RelativeSource"/> object specifying the relative location of the binding source to use. The default is null.</returns>
        public RelativeSource RelativeSource {
            get { return _binding.RelativeSource; }
            set { _binding.RelativeSource = value; }
        }

        /// <summary>Gets or sets the name of the element to use as the binding source object.</summary>
        /// <returns>The value of the Name property or x:Name Attribute of the element of interest. You can refer to elements in code only if they are registered to the appropriate <see cref="T:System.Windows.NameScope"/> through RegisterName. For more information, see WPF XAML Namescopes.The default is null.</returns>
        public string ElementName {
            get { return _binding.ElementName; }
            set { _binding.ElementName = value; }
        }

        /// <summary>Gets or sets a value that indicates whether the <see cref="T:System.Windows.Data.Binding"/> should get and set values asynchronously.</summary>
        /// <returns>The default is null.</returns>
        public bool IsAsync {
            get { return _binding.IsAsync; }
            set { _binding.IsAsync = value; }
        }

        /// <summary>Gets or sets opaque data passed to the asynchronous data dispatcher.</summary>
        /// <returns>Data passed to the asynchronous data dispatcher.</returns>
        public object AsyncState {
            get { return _binding.AsyncState; }
            set { _binding.AsyncState = value; }
        }

        /// <summary>Gets or sets a value that indicates whether to evaluate the <see cref="P:System.Windows.Data.Binding.Path"/> relative to the data item or the <see cref="T:System.Windows.Data.DataSourceProvider"/> object.</summary>
        /// <returns>false to evaluate the path relative to the data item itself; otherwise, true. The default is false.</returns>
        public bool BindsDirectlyToSource {
            get { return _binding.BindsDirectlyToSource; }
            set { _binding.BindsDirectlyToSource = value; }
        }

        /// <summary>Gets or sets a handler you can use to provide custom logic for handling exceptions that the binding engine encounters during the update of the binding source value. This is only applicable if you have associated an <see cref="T:System.Windows.Controls.ExceptionValidationRule"/> with your binding.</summary>
        /// <returns>A method that provides custom logic for handling exceptions that the binding engine encounters during the update of the binding source value.</returns>
        public UpdateSourceExceptionFilterCallback UpdateSourceExceptionFilter {
            get { return _binding.UpdateSourceExceptionFilter; }
            set { _binding.UpdateSourceExceptionFilter = value; }
        }

        #endregion

        /// <summary>Receives events from the centralized event manager.</summary>
        /// <returns>true if the listener handled the event. It is considered an error by the <see cref="T:System.Windows.WeakEventManager"/> handling in WPF to register a listener for an event that the listener does not handle. Regardless, the method should return false if it receives an event that it does not recognize or handle.</returns>
        /// <param name="managerType">The type of the <see cref="T:System.Windows.WeakEventManager"/> calling this method.</param><param name="sender">Object that originated the event.</param><param name="e">Event data.</param>
        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
            if (managerType != LanguageChangedEventManagerType) {
                return false;
            }

            OnPropertyChanged("TargetNullValue");
            OnPropertyChanged("FallbackValue");
            
            return true;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #region IDisposable

        public bool IsDisposed { get; private set; }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose() {
            Dispose(false);
        }

        /// <summary>Closes all files in the list and disposes all allocated resources.</summary>
        private void Dispose(bool finalizer) {
            if (IsDisposed) {
                return;
            }

            LanguageChangedEventManager.RemoveListener(TranslationManager.Instance, this);
            if (!finalizer) {
                GC.SuppressFinalize(this);
            }
            IsDisposed = true;
        }

        ~TBinding() {
            Dispose(true);
        }

        #endregion
    }

}
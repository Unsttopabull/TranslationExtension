using System;
using System.ComponentModel;
using System.Text;
using System.Windows;
using Frost.GettextMarkupExtension.Properties;

namespace Frost.GettextMarkupExtension {

    public class TranslationData : IWeakEventListener, INotifyPropertyChanged, IDisposable {
        private static readonly Type LanguageChangedEventManagerType = typeof(LanguageChangedEventManager);
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly string _key;
        private readonly string _prefix;
        private readonly string _postfix;
        private readonly string _context;
        private readonly object[] _args;
        private readonly string _pluralKey;
        private readonly long _number;

        public TranslationData(string key, string pluralKey, long number, string context, object[] args, string prefix, string postfix) {
            _key = key;
            _prefix = prefix;
            _postfix = postfix;
            _context = context;
            _args = args;
            _number = number;
            _pluralKey = pluralKey;
            LanguageChangedEventManager.AddListener(Gettext.Instance, this);
        }

        public TranslationData(string key, string context, object[] arguments, string prefix, string postfix) : this(key, null, 0, context, arguments, prefix, postfix) {
        }

        public string Value {
            get { return ProvideValue(_key, _pluralKey, _number, _context, _args, _prefix, _postfix); }
        }

        internal static string ProvideValue(string key, string pluralKey, long number, string context, object[] args, string prefix, string postfix) {
            StringBuilder sb;
            if (!string.IsNullOrEmpty(pluralKey)) {
                sb = new StringBuilder(
                    string.IsNullOrEmpty(context)
                        ? Gettext.Instance.TranslatePlural(key, pluralKey, number, args)
                        : Gettext.Instance.TranslatePluralContext(key, pluralKey, number, context, args)
                );
            }
            else {
                sb = new StringBuilder(
                    string.IsNullOrEmpty(context)
                        ? Gettext.Instance.Translate(key, args)
                        : Gettext.Instance.TranslateContext(key, context, args)
                );
            }

            if (!string.IsNullOrEmpty(prefix)) {
                sb.Insert(0, prefix);
            }

            if (!string.IsNullOrEmpty(postfix)) {
                sb.Append(postfix);
            }

            return sb.ToString();
        }

        internal static string ProvideValue(string key, string context, object[] args, string prefix, string postfix) {
            return ProvideValue(key, null, 0, context, args, prefix, postfix);
        }

        /// <summary>Receives events from the centralized event manager.</summary>
        /// <returns>true if the listener handled the event. It is considered an error by the <see cref="T:System.Windows.WeakEventManager"/> handling in WPF to register a listener for an event that the listener does not handle. Regardless, the method should return false if it receives an event that it does not recognize or handle.</returns>
        /// <param name="managerType">The type of the <see cref="T:System.Windows.WeakEventManager"/> calling this method.</param>
        /// <param name="sender">Object that originated the event.</param><param name="e">Event data.</param>
        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
            if (managerType != LanguageChangedEventManagerType) {
                return false;
            }

            OnPropertyChanged("Value");
            return true;
        }

        #region IDisposable

        public bool IsDisposed { get; private set; }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        void IDisposable.Dispose() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(false);
        }

        /// <summary>Closes all files in the list and disposes all allocated resources.</summary>
        private void Dispose(bool finalizer) {
            if (IsDisposed) {
                return;
            }

            LanguageChangedEventManager.RemoveListener(Gettext.Instance, this);
            if (!finalizer) {
                GC.SuppressFinalize(this);
            }
            IsDisposed = true;
        }

        ~TranslationData() {
            Dispose(true);
        }

        #endregion


        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged(string propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

}
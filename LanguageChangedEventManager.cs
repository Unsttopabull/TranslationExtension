using System;
using System.Windows;

namespace Frost.GettextMarkupExtension {

    internal class LanguageChangedEventManager : WeakEventManager {
        private static readonly Type ManagerType = typeof(LanguageChangedEventManager);

        public static void AddListener(Gettext source, IWeakEventListener listener) {
            CurrentManager.ProtectedAddListener(source, listener);
        }

        public static void RemoveListener(Gettext source, IWeakEventListener listener) {
            CurrentManager.ProtectedRemoveListener(source, listener);
        }

        public static LanguageChangedEventManager CurrentManager {
            get {
                LanguageChangedEventManager manager = (LanguageChangedEventManager) GetCurrentManager(ManagerType);
                if (manager != null) {
                    return manager;
                }

                manager = new LanguageChangedEventManager();
                SetCurrentManager(ManagerType, manager);
                return manager;                
            }
        }

        private void OnLanguageChanged(object sender, EventArgs e) {
            DeliverEvent(sender, e);
        }

        /// <summary>When overridden in a derived class, starts listening for the event being managed. After the <see cref="M:System.Windows.WeakEventManager.StartListening(System.Object)"/> method is first called, the manager should be in the state of calling <see cref="M:System.Windows.WeakEventManager.DeliverEvent(System.Object,System.EventArgs)"/> or <see cref="M:System.Windows.WeakEventManager.DeliverEventToList(System.Object,System.EventArgs,System.Windows.WeakEventManager.ListenerList)"/> whenever the relevant event from the provided source is handled.</summary>
        /// <param name="source">The source to begin listening on.</param>
        protected override void StartListening(object source) {
            ((Gettext) source).LanguageChanged += OnLanguageChanged;
        }

        /// <summary>When overridden in a derived class, stops listening on the provided source for the event being managed.</summary>
        /// <param name="source">The source to stop listening on.</param>
        protected override void StopListening(object source) {
            ((Gettext) source).LanguageChanged -= OnLanguageChanged;
        }
    }

}
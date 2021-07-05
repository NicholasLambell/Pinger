using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

// Sourced from https://tyrrrz.me/blog/wpf-listbox-selecteditems-twoway-binding

namespace Pinger.Behavior {
    public class ListBoxSelectionBehavior<T> : Behavior<ListBox> {
        #region Props

        private bool ViewHandled { get; set; }
        private bool ModelHandled { get; set; }

        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty
            .Register(
                nameof(SelectedItems),
                typeof(IList),
                typeof(ListBoxSelectionBehavior<T>),
                new FrameworkPropertyMetadata(
                    null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectedItemsChanged
                )
            );

        public IList SelectedItems {
            get => (IList)GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        #endregion

        private static void OnSelectedItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
            ListBoxSelectionBehavior<T> behavior = (ListBoxSelectionBehavior<T>)sender;

            if (
                behavior.ModelHandled ||
                behavior.AssociatedObject == null
            ) {
                return;
            }

            behavior.ModelHandled = true;
            behavior.SelectItems();
            behavior.ModelHandled = false;
        }

        // Propagate selected items from model to view
        private void SelectItems() {
            // Modification: For some reason this is sometimes called where selection mode doesn't match the actual selection mode
            //               This will trigger an exception when SelectedItems is modified
            if (AssociatedObject.SelectionMode == SelectionMode.Single) {
                return;
            }

            ViewHandled = true;
            AssociatedObject.SelectedItems.Clear();
            if (SelectedItems != null) {
                foreach (object item in SelectedItems) {
                    AssociatedObject.SelectedItems.Add(item);
                }
            }

            ViewHandled = false;
        }

        // Propagate selected items from view to model
        private void OnListBoxSelectionChanged(object sender, SelectionChangedEventArgs args) {
            if (
                ViewHandled ||
                AssociatedObject.Items.SourceCollection == null
            ) {
                return;
            }

            SelectedItems = AssociatedObject.SelectedItems.Cast<T>().ToArray();
        }

        // Re-select items when the set of items changes
        private void OnListBoxItemsChanged(object sender, NotifyCollectionChangedEventArgs args) {
            if (
                ViewHandled ||
                AssociatedObject.Items.SourceCollection == null
            ) {
                return;
            }

            SelectItems();
        }

        protected override void OnAttached() {
            base.OnAttached();

            AssociatedObject.SelectionChanged += OnListBoxSelectionChanged;
            ((INotifyCollectionChanged)AssociatedObject.Items).CollectionChanged += OnListBoxItemsChanged;
        }

        /// <inheritdoc />
        protected override void OnDetaching() {
            base.OnDetaching();

            if (AssociatedObject == null) {
                return;
            }

            AssociatedObject.SelectionChanged -= OnListBoxSelectionChanged;
            ((INotifyCollectionChanged)AssociatedObject.Items).CollectionChanged -= OnListBoxItemsChanged;
        }
    }
}
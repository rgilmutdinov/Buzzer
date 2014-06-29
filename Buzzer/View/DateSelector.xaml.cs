using System;
using System.Windows;
using System.Windows.Controls;

namespace Buzzer.View
{
   public partial class DateSelector : UserControl
   {
      private const int FirstYear = 1990;

      public static readonly DependencyProperty SelectedDateProperty =
         DependencyProperty.Register("SelectedDate", typeof (DateTime?), typeof (DateSelector),
                                     new PropertyMetadata(selectedDatePropertyChanged));

      public DateSelector()
      {
         InitializeComponent();

         initializeComboBoxes();
         signInSelectionChanged();
      }

      public DateTime? SelectedDate
      {
         get { return (DateTime?) GetValue(SelectedDateProperty); }
         set { SetValue(SelectedDateProperty, value); }
      }

      private static void selectedDatePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
      {
         var dateSelector = (DateSelector) dependencyObject;
         var value = (DateTime?) args.NewValue;
         dateSelector.setDate(value);
      }

      private void initializeComboBoxes()
      {
         DateTime today = DateTime.Today;

         for (int year = FirstYear; year <= today.Year; year++)
            _comboBoxYears.Items.Add(year);

         foreach (int month in new[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12})
            _comboBoxMonths.Items.Add(month);

         _comboBoxYears.SelectedItem = today.Year;
         _comboBoxMonths.SelectedItem = today.Month;

         fillComboBoxDays(today.Year, today.Month, null);
      }

      private void signInSelectionChanged()
      {
         _comboBoxYears.SelectionChanged += comboBoxSelectionChanged;
         _comboBoxMonths.SelectionChanged += comboBoxSelectionChanged;
         _comboBoxDays.SelectionChanged += comboBoxDaySelectionChanged;
      }

      private void signOutSelectionChanged()
      {
         _comboBoxYears.SelectionChanged -= comboBoxSelectionChanged;
         _comboBoxMonths.SelectionChanged -= comboBoxSelectionChanged;
         _comboBoxDays.SelectionChanged -= comboBoxDaySelectionChanged;
      }

      private void comboBoxDaySelectionChanged(object sender, SelectionChangedEventArgs e)
      {
         SelectedDate = new DateTime((int) _comboBoxYears.SelectedItem,
                                     (int) _comboBoxMonths.SelectedItem,
                                     (int) _comboBoxDays.SelectedItem);
      }

      private void comboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
      {
         var year = (int) _comboBoxYears.SelectedItem;
         var month = (int) _comboBoxMonths.SelectedItem;
         var temp = (int?) _comboBoxDays.SelectedItem;
         int daysInMonth = DateTime.DaysInMonth(year, month);

         if (temp.HasValue)
         {
            int day = daysInMonth < temp.Value ? daysInMonth : temp.Value;
            SelectedDate = new DateTime(year, month, day);
         }
         else
         {
            signOutSelectionChanged();
            fillComboBoxDays(year, month, null);
            signInSelectionChanged();
         }
      }

      private void setDate(DateTime? selectedDate)
      {
         if (selectedDate.HasValue && isInRange(selectedDate.Value))
         {
            DateTime date = selectedDate.Value;
            updateComboBoxes(date.Year, date.Month, date.Day);
         }
         else if (!selectedDate.HasValue || !isInRange(selectedDate.Value))
         {
            DateTime today = DateTime.Today;
            updateComboBoxes(today.Year, today.Month, null);
         }
      }

      private void updateComboBoxes(int year, int month, int? day)
      {
         signOutSelectionChanged();

         _comboBoxYears.SelectedItem = year;
         _comboBoxMonths.SelectedItem = month;

         fillComboBoxDays(year, month, day);

         signInSelectionChanged();
      }

      private bool isInRange(DateTime selectedDate)
      {
         const int january = 1;
         const int december = 12;

         var minDate = new DateTime(FirstYear, january, 1);
         int currentYear = DateTime.Today.Year;
         var maxDate = new DateTime(currentYear, december, 31);

         return minDate <= selectedDate && selectedDate <= maxDate;
      }

      private void fillComboBoxDays(int year, int month, int? day)
      {
         _comboBoxDays.Items.Clear();

         for (int i = 1; i <= DateTime.DaysInMonth(year, month); i++)
            _comboBoxDays.Items.Add(i);

         _comboBoxDays.SelectedItem = day;
      }
   }
}
